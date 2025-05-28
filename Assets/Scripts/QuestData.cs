using UnityEngine;

[System.Serializable]
public class QuestData {
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

    // ← Add this line:
    public PuzzleData puzzleData;    
      // the PuzzleData asset to trigger after dialogue
}
    