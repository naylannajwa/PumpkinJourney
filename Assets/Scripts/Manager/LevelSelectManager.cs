using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;
        public GameObject lockIcon;
        public string sceneName;
        public int levelNumber;
    }
    
    public LevelButton[] levels;
    
    void Start()
    {
        // CEK APAKAH INI GAME BARU
        CheckForNewGame();

        InitializeLevelButtons();
    }

    // TAMBAH: Method untuk mendeteksi game baru dan reset level locks
    void CheckForNewGame()
    {
        // Cek apakah game sudah pernah dimainkan sebelumnya
        // Gunakan flag "GameStarted" sebagai indikator
        bool hasPlayedBefore = PlayerPrefs.GetInt("GameStarted", 0) == 1;

        if (!hasPlayedBefore)
        {
            // INI GAME BARU! Reset semua level locks
            Debug.Log("🎮 GAME BARU DETECTED! Resetting all level locks...");

            // Reset semua level unlock status
            ResetAllLevelLocks();

            // Mark bahwa game sudah pernah dimainkan
            PlayerPrefs.SetInt("GameStarted", 1);
            PlayerPrefs.Save();

            Debug.Log("✅ Game baru setup selesai - Level 1 unlocked, others locked");
        }
        else
        {
            Debug.Log("🎮 Game sudah pernah dimainkan - menggunakan save data existing");
        }
    }

    // TAMBAH: Method untuk reset semua level locks
    void ResetAllLevelLocks()
    {
        // Reset Level 1 (selalu unlocked)
        PlayerPrefs.SetInt("Level1Unlocked", 1);

        // Reset Level 2, 3, 4 (locked)
        PlayerPrefs.SetInt("Level2Unlocked", 0);
        PlayerPrefs.SetInt("Level3Unlocked", 0);
        PlayerPrefs.SetInt("Level4Unlocked", 0);

        // Reset progress lainnya jika ada
        PlayerPrefs.SetInt("TotalCoins", 0);

        PlayerPrefs.Save();

        Debug.Log("🔒 All level locks reset - Only Level 1 unlocked");
    }

    void InitializeLevelButtons()
    {
        // Level 1 always unlocked
        PlayerPrefs.SetInt("Level1Unlocked", 1);
        
        foreach (LevelButton level in levels)
        {
            bool isUnlocked = PlayerPrefs.GetInt("Level" + level.levelNumber + "Unlocked", 0) == 1;
            
            level.button.interactable = isUnlocked;
            level.lockIcon.SetActive(!isUnlocked);
             
            if (isUnlocked)
            {
                string sceneName = level.sceneName;
                level.button.onClick.AddListener(() => LoadLevel(sceneName));
            }
        }
    }
    
    void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // TAMBAH: Method untuk force reset game (untuk testing/debugging)
    public void ForceResetGame()
    {
        Debug.Log("🔄 FORCE RESET GAME - Resetting all progress...");

        // Reset flag game started
        PlayerPrefs.SetInt("GameStarted", 0);

        // Reset semua level locks
        ResetAllLevelLocks();

        // Reinitialize buttons
        InitializeLevelButtons();

        Debug.Log("✅ Game force reset completed!");
    }
}