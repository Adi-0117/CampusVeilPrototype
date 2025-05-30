// PuzzleManager.cs
// Controls puzzle UI, instantiates puzzle prefabs, and handles completion.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    public GameObject puzzlePanel;
    public TMP_Text puzzleTitle;
    public TMP_Text puzzleInstructions;
    public Button solveButton;
    public Button cancelButton;
    public Transform puzzleContent;

    private PuzzleData currentPuzzle;
    private System.Action onPuzzleSolvedCallback;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        cancelButton.onClick.AddListener(ClosePuzzle);
    }

    public void StartPuzzle(PuzzleData data, System.Action onSolved)
    {
        currentPuzzle = data;
        onPuzzleSolvedCallback = onSolved;
        puzzleTitle.text = data.puzzleName;
        puzzleInstructions.text = data.instructions;

        foreach (Transform t in puzzleContent)
            Destroy(t.gameObject);

        if (data.puzzlePrefab != null)
            Instantiate(data.puzzlePrefab, puzzleContent);

        solveButton.gameObject.SetActive(false);
        solveButton.onClick.RemoveAllListeners();
        solveButton.onClick.AddListener(CompletePuzzle);

        puzzlePanel.SetActive(true);
    }

    public void OnPuzzleSolved()
    {
        solveButton.gameObject.SetActive(true);
    }

    void CompletePuzzle()
    {
        if (currentPuzzle != null)
            QuestManager.Instance.AddXP(currentPuzzle.xpReward);

        onPuzzleSolvedCallback?.Invoke();
        ClosePuzzle();
    }

    void ClosePuzzle()
    {
        foreach (Transform t in puzzleContent)
            Destroy(t.gameObject);
        puzzlePanel.SetActive(false);
    }
}
