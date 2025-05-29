using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject puzzlePanel;
    public TMP_Text puzzleTitle;
    public TMP_Text puzzleInstructions;
    public Button solveButton;
    public Button cancelButton;

    // Where we'll parent the puzzle prefab
    [Tooltip("Container under the panel to host the instantiated puzzle")]
    public Transform puzzleContent;

    private PuzzleData currentPuzzle;
    private System.Action onPuzzleSolvedCallback;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // clear & hide
        cancelButton.onClick.AddListener(() => ClosePuzzle());
    }

    public void StartPuzzle(PuzzleData data, System.Action onSolved)
    {
        Debug.Log($"[PuzzleManager] Starting puzzle \"{data.puzzleName}\" → prefab = {data.puzzlePrefab?.name ?? "null"}");
        Debug.Log($"[PuzzleManager] puzzleContent = {puzzleContent?.name ?? "null"} (childCount={puzzleContent?.childCount ?? 0})");

        currentPuzzle = data;
        onPuzzleSolvedCallback = onSolved;
        puzzleTitle.text = data.puzzleName;
        puzzleInstructions.text = data.instructions;

        // clear old content
        foreach (Transform t in puzzleContent)
            Destroy(t.gameObject);

        // spawn the right puzzle prefab
        if (data.puzzlePrefab != null)
        {
            Instantiate(data.puzzlePrefab, puzzleContent);
        }

        // set Solve button (we'll enable it only when the puzzle script tells us it's solved)
        solveButton.gameObject.SetActive(false);
        solveButton.onClick.RemoveAllListeners();
        solveButton.onClick.AddListener(CompletePuzzle);

        puzzlePanel.SetActive(true);
    }

    // Called by your puzzle script when the puzzle is solved
    public void OnPuzzleSolved()
    {
        solveButton.gameObject.SetActive(true);
    }

    void CompletePuzzle()
    {
        // award puzzle XP
        if (currentPuzzle != null)
        {
            QuestManager.Instance.AddXP(currentPuzzle.xpReward);
            Debug.Log($"[PuzzleManager] \"{currentPuzzle.puzzleName}\" solved! +{currentPuzzle.xpReward} XP");
        }

        onPuzzleSolvedCallback?.Invoke();
        ClosePuzzle();
    }

    void ClosePuzzle()
    {
        // clear content
        foreach (Transform t in puzzleContent)
            Destroy(t.gameObject);

        puzzlePanel.SetActive(false);
    }
}
