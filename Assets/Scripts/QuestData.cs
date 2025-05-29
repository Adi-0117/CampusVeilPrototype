using UnityEngine;

[CreateAssetMenu(fileName = "QuestData_", menuName = "CampusVeil/QuestData", order = 1)]
[System.Serializable]
public class QuestData : ScriptableObject
{
    [Tooltip("Order in which this quest appears (lower numbers first)")]
    public int levelIndex;
    public string questName;

    [Header("Dialogues")]
    public DialogueData preDialogue;   // before puzzle
    public DialogueData postDialogue;  // after puzzle

    [Header("Real-World Location")]
    public double latitude;
    public double longitude;

    [Header("Mock-Spawn (all platforms)")]
    public Vector3 editorSpawnPosition = Vector3.zero;

    [Header("Puzzle (Level)")]
    public GameObject puzzlePrefab;
    public PuzzleData puzzleData;

    [Header("Rewards")]
    [Tooltip("Optional: prefab of the item to give when this quest completes")]
    public ItemData rewardItem;
    public int xpReward = 10;
}
