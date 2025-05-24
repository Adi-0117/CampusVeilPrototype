using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Canvases & Panels")]
    public Canvas mainMenuCanvas;
    public Canvas gameplayCanvas;
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Button dialogueNextButton;
    public Text xpText;
    public Text questText;
    public GameObject inventoryPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ShowMainMenu();
        // Subscribe to quest events to update HUD
        QuestStoryManager.Instance.OnQuestStarted += UpdateQuestDisplay;
    }

    #region MainMenu
    public void ShowMainMenu()
    {
        mainMenuCanvas.enabled = true;
        gameplayCanvas.enabled = false;
    }

    public void StartGame()
    {
        mainMenuCanvas.enabled = false;
        gameplayCanvas.enabled = true;
        SceneController.Instance.LoadScene("Prototype"); // your gameplay scene name
    }
    #endregion

    #region HUD Updates
    private void UpdateQuestDisplay(QuestStoryManager.Quest quest)
    {
        questText.text = $"Quest: {quest.title}";
    }

    public void UpdateXP(int xp)
    {
        xpText.text = $"XP: {xp}";
    }
    #endregion

    #region Dialogue
    public void ShowDialogue(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = message;
        dialogueNextButton.onClick.RemoveAllListeners();
        dialogueNextButton.onClick.AddListener(() => HideDialogue());
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
    #endregion

    #region Inventory
    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
    #endregion
}
