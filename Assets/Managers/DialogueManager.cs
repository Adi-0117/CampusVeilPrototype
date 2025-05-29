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

    private Button panelButton;
    private Queue<DialogueLine> linesQueue;
    private System.Action<int> onChoiceSelected;

    void Awake() {
        linesQueue = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);

        // cache the Button on the panel for listener cleanup
        panelButton = dialoguePanel.GetComponent<Button>();
    }

    public void StartDialogue(DialogueData data, System.Action<int> choiceCallback = null) {
        onChoiceSelected = choiceCallback;
        dialoguePanel.SetActive(true);
        linesQueue.Clear();
        foreach (var line in data.lines) linesQueue.Enqueue(line);
        ShowNextLine();
    }

    public void ShowNextLine() {
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
                    onChoiceSelected?.Invoke(choiceIndex);
                    ShowNextLine();
                });
            }
        } else {
            // no choices → clicking the panel advances
            panelButton.onClick.RemoveAllListeners();
            panelButton.onClick.AddListener(ShowNextLine);
        }
    }

    void EndDialogue()
    {
        Debug.Log("[DialogueManager] EndDialogue() called — invoking onChoiceSelected");
        onChoiceSelected?.Invoke(0);
        onChoiceSelected = null;

        // only remove listeners if the panel really has a Button component
        var panelBtn = dialoguePanel.GetComponent<UnityEngine.UI.Button>();
        if (panelBtn != null)
            panelBtn.onClick.RemoveAllListeners();

        dialoguePanel.SetActive(false);
    }
}
