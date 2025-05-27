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

    private PuzzleData currentPuzzle;

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // hook up cancel
        cancelButton.onClick.AddListener(() => puzzlePanel.SetActive(false));
    }

    public void StartPuzzle(PuzzleData data) {
        currentPuzzle = data;
        puzzleTitle.text = data.puzzleName;
        puzzleInstructions.text = data.instructions;

        // check item requirement
        if (data.requiredItem != null &&
            !InventoryManager.Instance.HasItem(data.requiredItem)) {
            solveButton.interactable = false;
        } else {
            solveButton.interactable = true;
        }

        // hook up solve
        solveButton.onClick.RemoveAllListeners();
        solveButton.onClick.AddListener(CompletePuzzle);

        puzzlePanel.SetActive(true);
    }

    void CompletePuzzle() {
        // award XP
        QuestManager.Instance.AddXP(currentPuzzle.xpReward);
        Debug.Log($"Puzzle “{currentPuzzle.puzzleName}” solved! +{currentPuzzle.xpReward} XP");
        puzzlePanel.SetActive(false);

        // signal QuestManager or next step
        QuestManager.Instance.OnPuzzleCompleted(currentPuzzle);
    }
}
