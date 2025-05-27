using System.Collections;
using System.Collections.Generic;
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

    [Header("Quests Configuration")]
    public List<QuestData> quests;         // fill in Inspector
    public TMP_Text debugText;    // assign in Inspector
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
        if (currentQuestIndex >= quests.Count)
        {
            Debug.Log($"All quests done! Total XP: {playerXP}");
            return;
        }

        // Spawn the next marker based on GPS coordinates
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

        // --- NOW CHECK IF GPS IS ENABLED ---
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

        // 2) Get user GPS
        double userLat = Input.location.lastData.latitude;
        double userLon = Input.location.lastData.longitude;

        // 3) Calculate meter offsets
        const double earthRadius = 6378137.0;
        double dLat = (data.latitude - userLat) * Mathf.Deg2Rad;
        double dLon = (data.longitude - userLon) * Mathf.Deg2Rad;
        double latRad = userLat * Mathf.Deg2Rad;
        float northing = (float)(dLat * earthRadius);
        float easting = (float)(dLon * earthRadius * Mathf.Cos((float)latRad));

        // 4) Create AR anchor at that pose
        var pose = new Pose(new Vector3(easting, 0, northing), Quaternion.identity);
        var anchor = anchorManager.AddAnchor(pose);
        if (anchor == null)
        {
            Debug.LogError("Failed to create ARAnchor");
            yield break;
        }

        // 5) Instantiate your marker prefab under that anchor
        var marker = Instantiate(markerPrefab, anchor.transform);
        
        // find the Label inside the prefab and update its text
        var label = marker.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
        if (label != null) label.text = data.questName;
        // or if using legacy TextMesh:
        var tm = marker.transform.Find("Label")?.GetComponent<TextMesh>();
        if (tm != null) tm.text = data.questName;

        var qt = marker.GetComponent<QuestTrigger>();
        qt.dialogueData = data.dialogue;
        qt.questID     = currentQuestIndex;

        // 6) Retarget the compass
        var compass = FindObjectOfType<CompassController>();
        if (compass != null) compass.target = marker.transform;

        if (debugText != null)
            debugText.text = $"Spawned {data.questName}\nPos: {marker.transform.position.x:F1}, {marker.transform.position.y:F1}, {marker.transform.position.z:F1}";
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
        for (int i = 0; i < quests.Count; i++)
        {
            if (quests[i].puzzleData == data)
            {
                CompleteQuest(i);
                break;
            }
        }
    }
}
