// IPuzzleLevel.cs
// Interface for initializing puzzle levels with quest ID, reward item, and XP.
public interface IPuzzleLevel
{
    void Initialize(int questID, ItemData reward, int xpReward);
}
