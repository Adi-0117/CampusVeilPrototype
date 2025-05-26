using UnityEngine;

[System.Serializable]
public class QuestData {
    public string questName;
    public DialogueData dialogue;       // your DialogueData asset
    public int xpReward = 10;           // default XP
    public Vector3 spawnPosition;       // where to place the next marker
}
