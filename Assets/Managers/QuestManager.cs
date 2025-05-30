// QuestManager.cs
// Controls quest spawning via AR anchors/markers, handles completion, XP and rewards.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private GameObject _currentAnchorGO;
    private GameObject _currentMarkerGO;
    private CompassController cachedCompass;

    [Header("Story")]
    public DialogueData introDialogue;

    [Header("Editor Testing")]
    public bool useEditorMode = false;

    [Header("Quests Configuration")]
    public QuestData[] quests;

#if UNITY_EDITOR
    void OnValidate()
    {
        quests = UnityEditor
            .AssetDatabase.FindAssets("t:QuestData", new[] { "Assets/Data/Quests" })
            .Select(g =>
                UnityEditor.AssetDatabase.LoadAssetAtPath<QuestData>(
                    UnityEditor.AssetDatabase.GUIDToAssetPath(g)
                )
            )
            .OrderBy(q => q.levelIndex)
            .ToArray();
    }
#endif

    [Header("UI")]
    public GameObject markerPrefab;

    [Header("AR Components")]
    public ARAnchorManager anchorManager;

    private int currentQuestIndex = 0;
    private int playerXP = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        cachedCompass = FindObjectOfType<CompassController>();
    }

    void Start()
    {
        if (introDialogue != null)
            FindObjectOfType<DialogueManager>()
                .StartDialogue(introDialogue, _ => StartCurrentQuest());
        else
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

        StartCoroutine(SpawnMarkerAtLocation(quests[currentQuestIndex]));
    }

    IEnumerator SpawnMarkerAtLocation(QuestData data)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitUntil(() =>
                Permission.HasUserAuthorizedPermission(Permission.FineLocation)
                || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            );
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS not enabled on device");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
            yield return new WaitForSeconds(1);

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("Unable to start GPS");
            yield break;
        }

        double userLat = Input.location.lastData.latitude;
        double userLon = Input.location.lastData.longitude;
        const double earthRadius = 6378137.0;
        double dLat = (data.latitude - userLat) * Mathf.Deg2Rad;
        double dLon = (data.longitude - userLon) * Mathf.Deg2Rad;
        double latRad = userLat * Mathf.Deg2Rad;
        float northing = (float)(dLat * earthRadius);
        float easting = (float)(dLon * earthRadius * Mathf.Cos((float)latRad));

        var pose = new Pose(new Vector3(easting, 0, northing), Quaternion.identity);

        var anchorGO = new GameObject("ARAnchor");
        anchorGO.transform.SetPositionAndRotation(pose.position, pose.rotation);
        var anchor = anchorGO.AddComponent<ARAnchor>();
        GameObject marker;

        if (anchor != null)
        {
            _currentAnchorGO = anchorGO;
            marker = Instantiate(markerPrefab);
            marker.transform.SetParent(anchorGO.transform, false);
            _currentMarkerGO = marker;
        }
        else
        {
            Destroy(anchorGO);
            Vector3 worldPos = Camera.main.transform.TransformPoint(pose.position);
            marker = Instantiate(markerPrefab, worldPos, Quaternion.identity);
            _currentMarkerGO = marker;
            _currentAnchorGO = marker;
        }

        var qt = marker.GetComponent<QuestTrigger>();
        qt.questData = data;
        qt.questID = currentQuestIndex;

        if (cachedCompass != null)
            cachedCompass.target = marker.transform;

        if (marker.transform.Find("Label") is Transform lbl)
        {
            var tmpro = lbl.GetComponent<TextMeshProUGUI>();
            if (tmpro != null)
                tmpro.SetText(data.questName);

            var tm = lbl.GetComponent<TextMesh>();
            if (tm != null)
                tm.text = data.questName;
        }
    }

    public void CompleteQuest(int questID)
    {
        if (questID != currentQuestIndex)
            return;

        if (_currentMarkerGO != null)
        {
            Destroy(_currentMarkerGO);
            _currentMarkerGO = null;
        }
        else if (_currentAnchorGO != null)
        {
            Destroy(_currentAnchorGO);
            _currentAnchorGO = null;
        }

        playerXP += quests[questID].xpReward;
        var reward = quests[questID].rewardItem;
        if (reward != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(reward);
        }

        currentQuestIndex++;
        StartCurrentQuest();
    }

    public void AddXP(int amount)
    {
        playerXP += amount;
        Debug.Log($"+{amount} XP → Total: {playerXP}");
    }

    public void OnPuzzleCompleted(PuzzleData data)
    {
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
        _currentMarkerGO = marker;
        _currentAnchorGO = marker;
        var qt = marker.GetComponent<QuestTrigger>() ?? marker.AddComponent<QuestTrigger>();
        if (marker.GetComponent<Collider>() == null)
            marker.AddComponent<BoxCollider>();
        qt.questData = data;
        qt.questID = currentQuestIndex;
        if (cachedCompass != null)
            cachedCompass.target = marker.transform;
        Debug.Log(
            $"[QuestManager] [Mock] Spawned '{data.questName}' marker at {data.editorSpawnPosition}"
        );
    }
}
