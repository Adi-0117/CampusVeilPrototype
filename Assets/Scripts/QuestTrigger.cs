using UnityEngine;
using UnityEngine.EventSystems;

public class QuestTrigger : MonoBehaviour, IPointerClickHandler
{
    public int questID;
    [Tooltip("Drag your DialogueData asset here")]
    public DialogueData dialogueData;
    public QuestData questData;    // <- this holds the reference to your quest’s data
    
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
            Debug.LogError("No DialogueManager found in scene!");
    }

    // Called when the player taps/clicks this object
    public void OnPointerClick(PointerEventData e)
    {
        dialogueManager.StartDialogue(dialogueData, choiceIndex =>
        {
            // After dialogue closes, spawn the puzzle:
            if (questData.puzzlePrefab != null)
            {
                var puzzleObj = Instantiate(questData.puzzlePrefab,
                                        transform.position,
                                        Quaternion.identity);
                // Give the puzzle script this questID & reward info:
                var pd = questData.puzzleData;

                var handler = puzzleObj.GetComponent<IPuzzleLevel>();
                handler.Initialize(
                    questID,
                    pd.rewardItem,   // now comes from PuzzleData
                    pd.xpReward      // now the puzzle’s own XP reward
                );
            }
            else
            {
                // Fallback: immediately complete if no puzzle
                QuestManager.Instance.CompleteQuest(questID);
            }
        });
    }


    // Optional: handle which choice the player picked
    private void OnChoiceSelected(int choiceIndex)
    {
        QuestManager.Instance.CompleteQuest(questID);
    }
}
