using UnityEngine;

[CreateAssetMenu(
    fileName = "NewPuzzleData",
    menuName = "Puzzle/PuzzleData",
    order = 1)]
public class PuzzleData : ScriptableObject {
    public string puzzleName;
    [TextArea] public string instructions;
    public ItemData requiredItem;
    public int xpReward = 5;
}
