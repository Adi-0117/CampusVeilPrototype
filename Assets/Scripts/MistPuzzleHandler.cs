using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MistPuzzleHandler : MonoBehaviour, IPuzzleLevel, IPointerClickHandler
{
    public GameObject mistPanel;   // assign in prefab
    private int tapsNeeded = 5;
    private int tapsSoFar = 0;
    private int questID;
    private ItemData rewardItem;
    private int xpReward;

    public void Initialize(int questID, ItemData reward, int xp)
    {
        this.questID = questID;
        rewardItem    = reward;
        xpReward      = xp;

        mistPanel.SetActive(true);
        // ensure we can click the mist:
        var img = mistPanel.GetComponent<Image>();
        if (img != null) img.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData e)
    {
        // Each tap clears a bit of mist:
        tapsSoFar++;
        float remaining = 1f - ((float)tapsSoFar / tapsNeeded);
        var img = mistPanel.GetComponent<Image>();
        img.color = new Color(1, 1, 1, remaining);

        if (tapsSoFar >= tapsNeeded)
            CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        mistPanel.SetActive(false);

        // Award item & XP:
        InventoryManager.Instance.AddItem(rewardItem);
        QuestManager.Instance.AddXP(xpReward);

        // Signal quest finished:
        QuestManager.Instance.CompleteQuest(questID);

        // Clean up this puzzle object:
        Destroy(gameObject);
    }
}
