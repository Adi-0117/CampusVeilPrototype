// QuestionPuzzle.cs
// Multiple-choice puzzle that colors buttons and signals completion on correct answer.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPuzzle : MonoBehaviour
{
    public Button[] answerButtons;
    public int correctIndex = 0;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

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
