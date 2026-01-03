using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject levelSelectPanel;

    void Start()
    {
        Debug.Log("[MainMenuManager] 🏠 MainMenuManager script started!");
        levelSelectPanel.SetActive(false);
    }

    public void PlayGame()
    {
        // Load level pertama - kunci akan direset otomatis di GameManager.Start()
        GameManager.EnsureInstance();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel("gameplay");
        }
        else
        {
            SceneManager.LoadScene("gameplay");
        }
    }

    // Di MainMenuManager - ubah OpenLevelSelect()
    public void OpenLevelSelect()
    {
        levelSelectPanel.SetActive(true);

        // 🔧 DEBUG: Cek semua PlayerPrefs unlock status
        Debug.Log("🏠 [MainMenu] === PLAYERPREFS DEBUG ===");
        for (int i = 1; i <= 4; i++)
        {
            int status = PlayerPrefs.GetInt($"Level{i}Unlocked", 0);
            Debug.Log($"🏠 [MainMenu] Level{i}Unlocked = {status}");
        }
        Debug.Log("🏠 [MainMenu] === END DEBUG ===");

        // TAMBAH: Force refresh level buttons
        LevelSelectManager levelManager = FindObjectOfType<LevelSelectManager>();
        if (levelManager != null)
        {
            levelManager.InitializeLevelButtons();
            Debug.Log("🔄 Level select refreshed!");
        }
    }

    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    // 🔧 DEBUG: Method untuk force unlock semua level (untuk testing)
    public void ForceUnlockAllLevels()
    {
        Debug.Log("🔓 [DEBUG] Force unlocking all levels...");

        for (int i = 1; i <= 4; i++)
        {
            PlayerPrefs.SetInt($"Level{i}Unlocked", 1);
            Debug.Log($"🔓 [DEBUG] Force unlocked Level{i}");
        }

        PlayerPrefs.Save();

        // Refresh level select
        LevelSelectManager levelManager = FindObjectOfType<LevelSelectManager>();
        if (levelManager != null)
        {
            levelManager.InitializeLevelButtons();
            Debug.Log("🔄 Level select refreshed after force unlock!");
        }
    }

    // TAMBAH: Method untuk reset game progress (untuk demo)
    // PENTING: Method ini digunakan untuk tombol reset di level select
    public void ResetGameProgress()
    {
        Debug.Log("🎮 DEMO RESET: Resetting all game progress for demo...");
        Debug.Log("🔄 Deleting all PlayerPrefs...");
        Debug.Log("🔄 Reloading scene...");

        // Reset semua PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Reload scene untuk apply reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Debug.Log("✅ Game progress reset - ready for fresh demo!");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}