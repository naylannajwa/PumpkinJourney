using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Key Collected Notification")]
    public GameObject keyCollectedPanel; // Panel "Kunci telah diambil!"
    public float notificationDuration = 2f;
    
    private bool hasKey = false;
    private int currentLevel = 1; // Default to level 1

    private int coinCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // PENTING: Agar level persistent across scenes
            Debug.Log("🎯 GameManager created and set to DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
            Debug.Log("✅ GameManager instance created!");
        }
    }
    
    void Start()
    {
        // Auto-detect level from scene name jika belum di-set
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentLevel == 1 && currentSceneName != "homePage") // Jika masih default level 1
        {
            DetectLevelFromScene(currentSceneName);
        }

        Debug.Log($"🎯 GameManager Start - Current level: {currentLevel}, Scene: {currentSceneName}");

        // Reset key for new level (kunci per level, bukan persistent)
        hasKey = false;
        Debug.Log($"🔑 GameManager Start - Key reset for new level");

        // Hide notification panel
        if (keyCollectedPanel != null)
        {
            keyCollectedPanel.SetActive(false);
        }

        // Update door visuals (pintu tertutup di awal level)
        UpdateAllDoors();
    }
    
    public void CollectKey()
    {
        hasKey = true;

        Debug.Log("✅ Key collected for this level!");

        // Show notification
        ShowKeyCollectedNotification();

        // Update all doors in scene (pintu akan terbuka)
        UpdateAllDoors();
    }
    
    public bool HasKey()
    {
        return hasKey;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    /// <summary>
    /// Auto-detect level dari nama scene
    /// </summary>
    private void DetectLevelFromScene(string sceneName)
    {
        if (sceneName.StartsWith("gameplay"))
        {
            if (sceneName == "gameplay")
            {
                currentLevel = 1;
            }
            else
            {
                // Extract number from "gameplay2" -> 2, "gameplay3" -> 3, etc.
                string numberStr = sceneName.Substring(8); // Remove "gameplay" prefix
                if (int.TryParse(numberStr, out int levelNum))
                {
                    currentLevel = Mathf.Clamp(levelNum, 1, 4);
                }
            }
            Debug.Log($"🎯 Auto-detected level {currentLevel} from scene '{sceneName}'");
        }
    }
    
    void ShowKeyCollectedNotification()
    {
        if (keyCollectedPanel != null)
        {
            keyCollectedPanel.SetActive(true);
            Debug.Log("📢 Key collected notification shown!");
            
            // Hide after duration
            Invoke("HideKeyCollectedNotification", notificationDuration);
        }
    }
    
    void HideKeyCollectedNotification()
    {
        if (keyCollectedPanel != null)
        {
            keyCollectedPanel.SetActive(false);
        }
    }

    // Tambahkan method baru
    public void AddCoins(int amount)
    {
        coinCount += amount;
        Debug.Log($"🪙 Coin collected! Total coins: {coinCount}");
        
        // Optional: Update UI jika ada
        UpdateCoinUI();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    // Method untuk update UI (implementasikan jika ada UI coin)
    private void UpdateCoinUI()
    {
        // TAMBAH: Panggil UIManager untuk update coin display
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoinDisplay(coinCount);
        }
        else
        {
            Debug.LogWarning("⚠️ UIManager not found - coin display not updated");
        }
    }

    // TAMBAH: Method untuk update health display
    public void UpdateHealthDisplay()
    {
        // TAMBAH: Update health display saat health berubah
        if (UIManager.Instance != null)
        {
            // Cari player dan dapatkan health
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PumpkinMovement playerScript = playerObj.GetComponent<PumpkinMovement>();
                if (playerScript != null)
                {
                    int currentHealth = playerScript.GetHealthPoints();
                    UIManager.Instance.UpdateHealthDisplay(currentHealth);
                }
            }
        }
    }

    void UpdateAllDoors()
    {
        // Find all doors in scene and update visual
        DoorExit[] doors = FindObjectsOfType<DoorExit>();
        
        foreach (DoorExit door in doors)
        {
            door.OnKeyCollected();
        }
        
        Debug.Log($"🚪 Updated {doors.Length} door(s)");
    }
    
    public void LoadLevel(string levelName)
    {
        // Extract level number from scene name
        int levelNum = 1; // Default level 1

        if (levelName.StartsWith("Level"))
        {
            // Handle "Level1", "Level2", etc.
            string levelStr = levelName.Substring(5); // Remove "Level" prefix
            if (int.TryParse(levelStr, out levelNum))
            {
                currentLevel = Mathf.Clamp(levelNum, 1, 4);
            }
        }
        else if (levelName.StartsWith("gameplay"))
        {
            // Handle "gameplay", "gameplay2", "gameplay3", "gameplay4"
            if (levelName == "gameplay")
            {
                levelNum = 1;
            }
            else
            {
                // Extract number from "gameplay2" -> 2, "gameplay3" -> 3, etc.
                string numberStr = levelName.Substring(8); // Remove "gameplay" prefix
                if (int.TryParse(numberStr, out levelNum))
                {
                    // levelNum sudah berisi angka yang benar
                }
                else
                {
                    levelNum = 1; // fallback
                }
            }
            currentLevel = Mathf.Clamp(levelNum, 1, 4);
        }

        Debug.Log($"🎯 GameManager level set to {currentLevel} from scene '{levelName}'");
        SceneManager.LoadScene(levelName);
    }
    
    public void RestartLevel()
    {
        // Reset semua game state
        hasKey = false;
        coinCount = 0;
        currentLevel = 1; // Reset level counter jika perlu
        
        Debug.Log("🔄 Restarting level - Resetting all game state...");
        Debug.Log($"🔑 Key reset: {hasKey}");
        Debug.Log($"🪙 Coins reset: {coinCount}");
        
        // Restart level - scene akan dimuat ulang
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // TAMBAH METHOD BARU: PlayerDied - dipanggil saat player health = 0
    public void PlayerDied()
    {
        Debug.Log("💀 Player died - resetting level...");
        
        // Reset game state
        hasKey = false;
        coinCount = 0;
        
        // Reload current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
        
        Debug.Log($"🔄 Level reset: {currentScene}");
    }
}