using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject inventoryPanel;
    public GameObject settingsPanel;

    public void StartGame() {
        SceneManager.LoadScene("ARPlayground");
    }

    public void ContinueGame() {
        // TODO: load saved state here
        SceneManager.LoadScene("ARPlayground");
    }

    public void OpenInventory() {
        inventoryPanel.SetActive(true);
    }
    public void CloseInventory() {
        inventoryPanel.SetActive(false);
    }

    public void OpenSettings() {
        settingsPanel.SetActive(true);
    }
    public void CloseSettings() {
        settingsPanel.SetActive(false);
    }
}
