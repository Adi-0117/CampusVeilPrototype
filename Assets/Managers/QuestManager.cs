using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // Track both the anchor and the marker separately
    private GameObject _currentAnchorGO;
    private GameObject _currentMarkerGO;

    // Cache compass reference
    private CompassController cachedCompass;

    [Header("Story")]
    [Tooltip("Runs before any quest spawns")]
    public DialogueData introDialogue;

    [Header("Editor Testing")]
    [Tooltip("When checked and running in the Editor, markers will spawn at mock positions instead of using GPS")]
    public bool useEditorMode = false;

    [Header("Quests Configuration")]
    [Tooltip("All QuestData assets in Assets/Data/Quests")]
    public QuestData[] quests;

#if UNITY_EDITOR
    void OnValidate() {
        // whenever you edit, auto-populate from the Quests folder
        quests = UnityEditor.AssetDatabase
            .FindAssets("t:QuestData", new[]{"Assets/Data/Quests"})
            .Select(g => UnityEditor.AssetDatabase.LoadAssetAtPath<QuestData>(
                UnityEditor.AssetDatabase.GUIDToAssetPath(g)))
            .OrderBy(q => q.levelIndex)  // if you add a "levelIndex" field
            .ToArray();
    }
#endif

    [Header("UI")]
    public GameObject markerPrefab;        // your portal prefab

    [Header("AR Components")]
    public ARAnchorManager anchorManager;  // drag in your AR Session Origin's ARAnchorManager

    private int currentQuestIndex = 0;
    private int playerXP = 0;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cache the compass so we don't call FindObjectOfType each spawn
        cachedCompass = FindObjectOfType<CompassController>();
    }

    void Start()
    {
        // 1) show the intro, then kick off the first quest
        if (introDialogue != null)
        {
            FindObjectOfType<DialogueManager>()
                .StartDialogue(introDialogue, _ => StartCurrentQuest());
        }
        else
        {
            StartCurrentQuest();
        }
    }

    void StartCurrentQuest()
    {
        if (currentQuestIndex >= quests.Length)
        {
            Debug.Log($"All quests done! Total XP: {playerXP}");
            return;
        }

        if (useEditorMode)
        {
            SpawnMockMarker(quests[currentQuestIndex]);
            return;
        }

        // normal GPS flow on device:
        StartCoroutine(SpawnMarkerAtLocation(quests[currentQuestIndex]));
    }

    IEnumerator SpawnMarkerAtLocation(QuestData data)
    {
        // --- ASK FOR PERMISSION ON ANDROID ---
        #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            // wait until the user grants or denies
            yield return new WaitUntil(() =>
                Permission.HasUserAuthorizedPermission(Permission.FineLocation)
                || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            );
        }
        #endif

        // --- CHECK IF GPS IS ENABLED ---
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS not enabled on device");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
        {
            yield return new WaitForSeconds(1);
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("Unable to start GPS");
            yield break;
        }

        // We have a lock!
        double userLat = Input.location.lastData.latitude;
        double userLon = Input.location.lastData.longitude;

        // Calculate meter offsets
        const double earthRadius = 6378137.0;
        double dLat = (data.latitude - userLat) * Mathf.Deg2Rad;
        double dLon = (data.longitude - userLon) * Mathf.Deg2Rad;
        double latRad = userLat * Mathf.Deg2Rad;
        float northing = (float)(dLat * earthRadius);
        float easting = (float)(dLon * earthRadius * Mathf.Cos((float)latRad));

        // Create AR anchor at that pose
        var pose = new Pose(new Vector3(easting, 0, northing), Quaternion.identity);

        // Create anchor using newer API
        var anchorGO = new GameObject("ARAnchor");
        anchorGO.transform.SetPositionAndRotation(pose.position, pose.rotation);
        var anchor = anchorGO.AddComponent<ARAnchor>();
        GameObject marker;

        if (anchor != null)
        {
            _currentAnchorGO = anchorGO;
            marker = Instantiate(markerPrefab);
            marker.transform.SetParent(anchorGO.transform, worldPositionStays: false);
            _currentMarkerGO = marker;
        }
        else
        {
            Destroy(anchorGO);  // Clean up failed anchor GameObject
            // Place marker relative to the camera instead:
            Vector3 worldPos = Camera.main.transform.TransformPoint(pose.position);
            marker = Instantiate(markerPrefab, worldPos, Quaternion.identity);
            _currentMarkerGO = marker;  // In fallback, marker is its own anchor
            _currentAnchorGO = marker;
        }

        // Now we have `marker` guaranteed:
        var qt = marker.GetComponent<QuestTrigger>();
        qt.questData = data;  // Assign the full QuestData asset
        qt.questID = currentQuestIndex;

        // Retarget compass using cached reference
        if (cachedCompass != null)
            cachedCompass.target = marker.transform;

        // Optional: label it so you can see it in-world
        if (marker.transform.Find("Label") is Transform lbl)
        {
            var tmpro = lbl.GetComponent<TextMeshProUGUI>();
            if (tmpro != null) tmpro.SetText(data.questName);
            
            var tm = lbl.GetComponent<TextMesh>();
            if (tm != null) tm.text = data.questName;
        }
    }

    public void CompleteQuest(int questID)
    {
        if (questID != currentQuestIndex)
        {
            Debug.Log($"[QuestManager] Ignoring CompleteQuest({questID}), currentQuestIndex={currentQuestIndex}");
            return;
        }

        Debug.Log($"[QuestManager] Completing quest {questID} (\"{quests[questID].questName}\")");

        // 1) Destroy the marker first (if it exists)
        if (_currentMarkerGO != null)
        {
            Debug.Log($"[QuestManager] Destroying marker: {_currentMarkerGO.name}");
            Destroy(_currentMarkerGO);
            _currentMarkerGO = null;
        }
        // Then destroy the anchor if it's separate from the marker
        else if (_currentAnchorGO != null)
        {
            Debug.Log($"[QuestManager] Destroying anchor: {_currentAnchorGO.name}");
            Destroy(_currentAnchorGO);
            _currentAnchorGO = null;
        }
        else
        {
            Debug.LogWarning("[QuestManager] No marker or anchor to destroy!");
        }

        // 2) Award XP
        playerXP += quests[questID].xpReward;
        Debug.Log($"[QuestManager] +{quests[questID].xpReward} XP → total {playerXP}");

        // 3) Award the item (if you have one)
        var reward = quests[questID].rewardItem;
        Debug.Log($"[QuestManager] Quest {questID} rewardItem = {reward?.name ?? "null"}");

        if (reward != null)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(reward);
                Debug.Log($"[QuestManager] Awarded item: {reward.itemName}");
            }
            else
            {
                Debug.LogWarning("[QuestManager] No InventoryManager found in scene!");
            }
        }

        // 4) Advance to next quest
        currentQuestIndex++;
        StartCurrentQuest();
    }

    // Optional helper if you want to add XP from puzzles too
    public void AddXP(int amount)
    {
        playerXP += amount;
        Debug.Log($"+{amount} XP → Total: {playerXP}");
    }

    public void OnPuzzleCompleted(PuzzleData data)
    {
        // Find the quest with this puzzle
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].puzzleData == data)
            {
                CompleteQuest(i);
                break;
            }
        }
    }

    private void SpawnMockMarker(QuestData data)
    {
        var marker = Instantiate(markerPrefab, data.editorSpawnPosition, Quaternion.identity);

        // Track the marker (which is its own anchor in mock mode)
        _currentMarkerGO = marker;
        _currentAnchorGO = marker;

        var qt = marker.GetComponent<QuestTrigger>();
        if (qt == null) qt = marker.AddComponent<QuestTrigger>();
        if (marker.GetComponent<Collider>() == null) marker.AddComponent<BoxCollider>();

        qt.questData = data;  // Assign the full QuestData asset
        qt.questID = currentQuestIndex;

        // Use cached compass reference
        if (cachedCompass != null)
            cachedCompass.target = marker.transform;

        Debug.Log($"[QuestManager] [Mock] Spawned '{data.questName}' marker at {data.editorSpawnPosition}");
    }
}
