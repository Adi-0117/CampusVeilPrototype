using UnityEngine;
using UnityEngine.EventSystems;

public class QuestTrigger : MonoBehaviour, IPointerClickHandler
{
    public int questID;
    [Tooltip("Drag your DialogueData asset here")]
    public DialogueData dialogueData;

    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
            Debug.LogError("No DialogueManager found in scene!");
    }

    // Called when the player taps/clicks this object
    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialogueData != null)
            dialogueManager.StartDialogue(dialogueData, OnChoiceSelected);
        else
            Debug.LogWarning("No DialogueData assigned to QuestTrigger on " + gameObject.name);
    }

    // Optional: handle which choice the player picked
    private void OnChoiceSelected(int choiceIndex)
    {
        QuestManager.Instance.CompleteQuest(questID);
    }
}
