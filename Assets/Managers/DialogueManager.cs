// DialogueManager.cs
// Handles displaying dialogue lines, portraits, names and spawning choice buttons.
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        linesQueue = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
        panelButton = dialoguePanel.GetComponent<Button>();
    }

    public void StartDialogue(DialogueData data, System.Action<int> choiceCallback = null)
    {
        onChoiceSelected = choiceCallback;
        dialoguePanel.SetActive(true);
        linesQueue.Clear();
        foreach (var line in data.lines)
            linesQueue.Enqueue(line);
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        var line = linesQueue.Dequeue();
        portraitImage.sprite = line.portrait;
        nameText.text = line.speakerName;
        dialogueText.text = line.text;

        foreach (Transform t in choicesContainer)
            Destroy(t.gameObject);

        if (line.choices != null && line.choices.Length > 0)
        {
            for (int i = 0; i < line.choices.Length; i++)
            {
                int choiceIndex = i;
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<Text>().text = line.choices[i].text;
                btn.onClick.AddListener(() =>
                {
                    onChoiceSelected?.Invoke(choiceIndex);
                    ShowNextLine();
                });
            }
        }
        else
        {
            panelButton.onClick.RemoveAllListeners();
            panelButton.onClick.AddListener(ShowNextLine);
        }
    }

    void EndDialogue()
    {
        onChoiceSelected?.Invoke(0);
        onChoiceSelected = null;

        var panelBtn = dialoguePanel.GetComponent<Button>();
        if (panelBtn != null)
            panelBtn.onClick.RemoveAllListeners();

        dialoguePanel.SetActive(false);
    }
}
