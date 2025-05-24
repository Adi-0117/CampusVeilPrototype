using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestStoryManager : MonoBehaviour
{
    public static QuestStoryManager Instance { get; private set; }

    // Simple quest data structure
    [Serializable]
    public class Quest
    {
        public string questID;
        public string title;
        public string description;
        public bool isComplete;
    }

    // List of all quests/story beats
    [SerializeField]
    private List<Quest> quests = new List<Quest>();
    private int currentQuestIndex = 0;

    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (quests.Count > 0)
            StartQuest(quests[0].questID);
    }

    /// <summary>
    /// Begins the quest with the given ID.
    /// </summary>
    public void StartQuest(string questID)
    {
        Quest quest = quests.Find(q => q.questID == questID);
        if (quest == null)
        {
            Debug.LogWarning($"QuestStoryManager: Quest ID '{questID}' not found.");
            return;
        }

        currentQuestIndex = quests.IndexOf(quest);
        quest.isComplete = false;
        OnQuestStarted?.Invoke(quest);
        Debug.Log($"Quest Started: {quest.title}");
    }

    /// <summary>
    /// Marks the current quest complete and advances to the next.
    /// </summary>
    public void CompleteCurrentQuest()
    {
        if (currentQuestIndex < 0 || currentQuestIndex >= quests.Count)
            return;

        Quest quest = quests[currentQuestIndex];
        quest.isComplete = true;
        OnQuestCompleted?.Invoke(quest);
        Debug.Log($"Quest Completed: {quest.title}");

        int nextIndex = currentQuestIndex + 1;
        if (nextIndex < quests.Count)
        {
            StartQuest(quests[nextIndex].questID);
        }
    }

    /// <summary>
    /// Returns the currently active quest.
    /// </summary>
    public Quest GetCurrentQuest()
    {
        if (currentQuestIndex < 0 || currentQuestIndex >= quests.Count)
            return null;
        return quests[currentQuestIndex];
    }
}
