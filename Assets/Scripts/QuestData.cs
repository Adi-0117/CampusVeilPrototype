// QuestData.cs
// ScriptableObject describing a quest’s dialogues, location, puzzle, and rewards.
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData_", menuName = "CampusVeil/QuestData", order = 1)]
public class QuestData : ScriptableObject
{
    public int levelIndex;
    public string questName;
    public DialogueData preDialogue;
    public DialogueData postDialogue;
    public double latitude;
    public double longitude;
    public Vector3 editorSpawnPosition = Vector3.zero;
    public GameObject puzzlePrefab;
    public PuzzleData puzzleData;
    public ItemData rewardItem;
    public int xpReward = 10;
}
