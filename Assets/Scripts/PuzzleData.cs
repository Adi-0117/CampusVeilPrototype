using UnityEngine;

[CreateAssetMenu(
    fileName = "NewPuzzleData",
    menuName = "Puzzle/PuzzleData",
    order = 1)]
public class PuzzleData : ScriptableObject {
    public string puzzleName;
    [TextArea] public string instructions;
    public ItemData requiredItem;
    
    [Header("Prefab")]
    [Tooltip("Drag in the specific prefab for this puzzle")]
    public GameObject puzzlePrefab;

    [Header("Reward")]
    public ItemData rewardItem; 
    public int xpReward = 5;
}
