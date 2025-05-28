public interface IPuzzleLevel
{
    /// <summary>
    /// Called by QuestTrigger to start the puzzle.
    /// </summary>
    /// <param name="questID">Which quest to complete on success.</param>
    /// <param name="reward">Item to give.</param>
    /// <param name="xpReward">XP to award.</param>
    void Initialize(int questID, ItemData reward, int xpReward);
}
