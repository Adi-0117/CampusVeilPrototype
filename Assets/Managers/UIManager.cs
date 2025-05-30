// UIManager.cs
// Handles UI panel toggles and starting/continuing the game scene.
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject inventoryPanel;
    public GameObject settingsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("ARPlayground");
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("ARPlayground");
    }

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
