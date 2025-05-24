using UnityEngine;
using UnityEngine.EventSystems;

public class QuestTrigger : MonoBehaviour, IPointerClickHandler
{
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
        Debug.Log($"Player chose option #{choiceIndex} on quest \"{dialogueData.name}\"");
        // TODO: mark this quest as complete, reward XP, spawn next marker, etc.
    }
}
