using UnityEngine;
[CreateAssetMenu(fileName = "QuestData_", menuName = "CampusVeil/QuestData", order = 1)]

[System.Serializable]
public class QuestData: ScriptableObject {
    [Tooltip("Order in which this quest appears (lower numbers first)")]
    public int levelIndex;
    public string questName;
    public DialogueData dialogue;       
    public int xpReward = 10;          
    [Header("Real-World Location")]
    [Tooltip("Latitude of this building")]
    public double latitude;
    [Tooltip("Longitude of this building")]
    public double longitude;

    [Header("Mock-Spawn (all platforms)")]
    [Tooltip("When Use Editor Mode is checked, marker will appear here")]
    public Vector3 editorSpawnPosition = Vector3.zero;

    [Header("Puzzle (Level)")]
    [Tooltip("Prefab that implements this level's AR puzzle")]
    public GameObject puzzlePrefab;
    public PuzzleData puzzleData;    
      // the PuzzleData asset to trigger after dialogue
}
    