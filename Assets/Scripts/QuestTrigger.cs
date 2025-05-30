// QuestTrigger.cs
// Responds to marker clicks by running pre-dialogue, puzzles, post-dialogue, and quest completion.
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestTrigger : MonoBehaviour, IPointerClickHandler
{
    public QuestData questData;
    public int questID;
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
            Debug.LogError("[QuestTrigger] No DialogueManager found!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RunPreDialogue();
    }

    void RunPreDialogue()
    {
        if (questData.preDialogue != null)
            dialogueManager.StartDialogue(questData.preDialogue, _ => AfterPreDialogue());
        else
            AfterPreDialogue();
    }

    void AfterPreDialogue()
    {
        if (questData.puzzleData != null)
            PuzzleManager.Instance.StartPuzzle(questData.puzzleData, OnPuzzleSolved);
        else
            RunPostDialogue();
    }

    void OnPuzzleSolved()
    {
        RunPostDialogue();
    }

    void RunPostDialogue()
    {
        if (questData.postDialogue != null)
            dialogueManager.StartDialogue(questData.postDialogue, _ => CompleteThisQuest());
        else
            CompleteThisQuest();
    }

    void CompleteThisQuest()
    {
        QuestManager.Instance.CompleteQuest(questID);
    }
}
