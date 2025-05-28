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
    public Text debugText;    // assign in Inspector
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
    }

    void Start()
    {
        StartCurrentQuest();
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

        // Show status right away
        if (debugText != null)
            debugText.text = $"GPS enabled? {Input.location.isEnabledByUser}\n" +
                            $"Status: {Input.location.status}";

        // --- NOW CHECK IF GPS IS ENABLED ---
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS not enabled on device");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
        {
            if (debugText != null)
                debugText.text = $"Waiting for GPS… ({maxWait}s)\nStatus: {Input.location.status}";
            yield return new WaitForSeconds(1);
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            debugText.text = $"GPS failed to start: {Input.location.status}";
            Debug.LogError("Unable to start GPS");
            yield break;
        }

        // We have a lock!
        double userLat = Input.location.lastData.latitude;
        double userLon = Input.location.lastData.longitude;
        if (debugText != null)
            debugText.text = $"GPS Locked\nLat: {userLat:F6}\nLon: {userLon:F6}";

        // 3) Calculate meter offsets
        const double earthRadius = 6378137.0;
        double dLat = (data.latitude - userLat) * Mathf.Deg2Rad;
        double dLon = (data.longitude - userLon) * Mathf.Deg2Rad;
        double latRad = userLat * Mathf.Deg2Rad;
        float northing = (float)(dLat * earthRadius);
        float easting = (float)(dLon * earthRadius * Mathf.Cos((float)latRad));

        // Before creating the anchor:
        if (debugText != null)
            debugText.text += $"\nOffset E: {easting:F1}m, N: {northing:F1}m";

        // 4) Create AR anchor at that pose
        var pose = new Pose(new Vector3(easting, 0, northing), Quaternion.identity);
        debugText.text += $"\nAttempting to anchor…";

        ARAnchor anchor = anchorManager.AddAnchor(pose);
        GameObject marker;

        if (anchor != null)
        {
            debugText.text += "\n✅ Anchor created";
            marker = Instantiate(markerPrefab, anchor.transform);
        }
        else
        {
            debugText.text += "\n❌ Anchor failed — using fallback";
            // Place marker relative to the camera instead:
            Vector3 worldPos = Camera.main.transform.TransformPoint(pose.position);
            marker = Instantiate(markerPrefab, worldPos, Quaternion.identity);
        }

        // Now we have `marker` guaranteed:
        var qt = marker.GetComponent<QuestTrigger>();
        qt.dialogueData = data.dialogue;
        qt.questID     = currentQuestIndex;

        // Retarget compass:
        var compass = FindObjectOfType<CompassController>();
        if (compass != null)
            compass.target = marker.transform;

        // Optional: label it so you can see it in-world
        if (marker.transform.Find("Label") is Transform lbl)
        {
            var tmpro = lbl.GetComponent<TextMeshProUGUI>();
            if (tmpro != null) tmpro.SetText(data.questName);
            
            var tm = lbl.GetComponent<TextMesh>();
            if (tm != null) tm.text = data.questName;
        }

        debugText.text += $"\nMarker at {marker.transform.position}";
    }

    public void CompleteQuest(int questID)
    {
        if (questID != currentQuestIndex) return;

        // Award XP
        playerXP += quests[questID].xpReward;
        // Debug.Log($"Quest "{quests[questID].questName}" complete! +{quests[questID].xpReward} XP (Total: {playerXP})");

        // Advance to the next quest
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
        var qt = marker.GetComponent<QuestTrigger>();
        if (qt == null) qt = marker.AddComponent<QuestTrigger>();
        if (marker.GetComponent<Collider>() == null) marker.AddComponent<BoxCollider>();

        qt.questData    = data;
        qt.dialogueData = data.dialogue;
        qt.questID     = currentQuestIndex;

        if (FindObjectOfType<CompassController>() is CompassController comp)
            comp.target = marker.transform;
        debugText.text = $"[Mock] Spawned '{data.questName}' at {data.editorSpawnPosition}";
    }
}
