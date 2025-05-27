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
    // ← Add this line:
    public PuzzleData puzzleData;    
      // the PuzzleData asset to trigger after dialogue
}
    