// PuzzleData.cs
// ScriptableObject defining puzzle metadata, prefab, and rewards.
using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Puzzle/PuzzleData", order = 1)]
public class PuzzleData : ScriptableObject
{
    public string puzzleName;

    [TextArea]
    public string instructions;
    public ItemData requiredItem;
    public GameObject puzzlePrefab;
    public ItemData rewardItem;
    public int xpReward = 5;
}
