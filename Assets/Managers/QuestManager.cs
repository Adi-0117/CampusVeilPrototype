using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quests Configuration")]
    public List<QuestData> quests;         // fill in Inspector
    public GameObject markerPrefab;        // your cube or custom AR prefab

    private int currentQuestIndex = 0;
    private int playerXP = 0;

    void Awake() {
        // Singleton pattern
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        StartCurrentQuest();
    }

    void StartCurrentQuest() {
        if (currentQuestIndex >= quests.Count) {
            Debug.Log("All quests completed! Total XP: " + playerXP);
            return;
        }

        // Spawn the next marker
        var data = quests[currentQuestIndex];
        var marker = Instantiate(markerPrefab, data.spawnPosition, Quaternion.identity);
        var compass = FindObjectOfType<CompassController>();
        if (compass != null)
            compass.target = marker.transform;
        // Assign the proper DialogueData to that marker’s QuestTrigger
        var qt = marker.GetComponent<QuestTrigger>();
        qt.dialogueData = data.dialogue;
        qt.questID     = currentQuestIndex;  // add this field to QuestTrigger
    }

    public void CompleteQuest(int questID) {
        if (questID != currentQuestIndex) return;  // ignore out-of-order
        // Award XP
        playerXP += quests[questID].xpReward;
        Debug.Log($"Quest “{quests[questID].questName}” done! +{quests[questID].xpReward} XP → total {playerXP}");
        // Clean up the old marker (QuestTrigger can Destroy itself after click)
        // Move on
        currentQuestIndex++;
        StartCurrentQuest();
    }
}
