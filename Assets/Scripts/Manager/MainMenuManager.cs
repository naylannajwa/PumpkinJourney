using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject levelSelectPanel;

    void Start()
    {
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

    public void OpenLevelSelect()
    {
        levelSelectPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
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