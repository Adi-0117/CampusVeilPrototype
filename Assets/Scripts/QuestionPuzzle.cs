using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionPuzzle : MonoBehaviour
{
    [Tooltip("Buttons for each answer")]
    public Button[] answerButtons;
    [Tooltip("Index of the correct button")]
    public int correctIndex = 0;
    public Color correctColor = Color.green;
    public Color wrongColor   = Color.red;

    void Start()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;
            answerButtons[i].onClick.AddListener(() => OnAnswer(idx));
        }
    }

    void OnAnswer(int idx)
    {
        var text = answerButtons[idx].GetComponentInChildren<TMP_Text>();
        if (idx == correctIndex)
        {
            text.color = correctColor;
            PuzzleManager.Instance.OnPuzzleSolved();
        }
        else
        {
            text.color = wrongColor;
            answerButtons[idx].interactable = false;
        }
    }
}
