using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;

    private Queue<DialogueLine> linesQueue;
    private System.Action<int> onChoiceSelected;

    void Awake() {
        linesQueue = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData data, System.Action<int> choiceCallback = null) {
        onChoiceSelected = choiceCallback;
        dialoguePanel.SetActive(true);
        linesQueue.Clear();
        foreach (var line in data.lines) linesQueue.Enqueue(line);
        ShowNextLine();
    }

    public void ShowNextLine() {
        // Debug logging to check null references
        Debug.LogFormat("[DialogueManager] ShowNextLine() called. " +
            "dialoguePanel={0}, portraitImage={1}, nameText={2}, dialogueText={3}, " +
            "choicesContainer={4}, choiceButtonPrefab={5}",
            dialoguePanel==null,
            portraitImage==null,
            nameText==null,
            dialogueText==null,
            choicesContainer==null,
            choiceButtonPrefab==null
        );

        if (linesQueue.Count == 0) {
            EndDialogue();
            return;
        }

        var line = linesQueue.Dequeue();
        portraitImage.sprite = line.portrait;
        nameText.text = line.speakerName;
        dialogueText.text = line.text;

        // clear old choices
        foreach (Transform t in choicesContainer) Destroy(t.gameObject);

        if (line.choices != null && line.choices.Length > 0) {
            // spawn choice buttons
            for (int i = 0; i < line.choices.Length; i++) {
                int choiceIndex = i;
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<Text>().text = line.choices[i].text;
                btn.onClick.AddListener(() => {
                    EndDialogue();     // close UI or proceed as desired
                    onChoiceSelected?.Invoke(choiceIndex);
                });
            }
        } else {
            // no choices → clicking the panel advances
            dialoguePanel.GetComponent<Button>().onClick.RemoveAllListeners();
            dialoguePanel.GetComponent<Button>()
                         .onClick.AddListener(ShowNextLine);
        }
    }

    void EndDialogue() {
        dialoguePanel.SetActive(false);
    }
}
