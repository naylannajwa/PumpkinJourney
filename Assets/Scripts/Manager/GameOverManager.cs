using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button homeButton;

    [Header("Settings")]
    public string homeSceneName = "homePage";
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Note: GameOverManager is per-scene, not DontDestroyOnLoad
            // Each level should have its own GameOverManager
            Debug.Log("💀 GameOverManager initialized!");
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate GameOverManager found! Destroying...");
            Destroy(gameObject);
            return;
        }
    }

    public static void EnsureInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<GameOverManager>();

            if (Instance == null)
            {
                GameObject go = new GameObject("GameOverManager");
                Instance = go.AddComponent<GameOverManager>();
                Debug.Log("💀 GameOverManager instance created!");
            }
            else
            {
                Debug.Log("✅ GameOverManager instance found in scene");
            }
        }
    }

    void Start()
    {
        // Reset state for fresh scene
        isGameOver = false;
        Time.timeScale = 1f; // Ensure game is running

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);

            // Ensure canvas is hidden at start
            Canvas canvas = gameOverPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.transform.localScale = Vector3.zero;
                Debug.Log("📺 GameOver canvas hidden at start!");
            }
        }

        if (gameOverText != null)
        {
            gameOverText.text = "GAME OVER";
        }

        Debug.Log("💀 GameOverManager ready!");
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f; // Pause game

        // Play game over sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverSound();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("💀 Game Over!");

            // Animasi akan otomatis berjalan karena Animator component
        }
    }

    public void HideGameOver()
    {
        if (!isGameOver) return;

        isGameOver = false;
        Time.timeScale = 1f; // Resume game

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("▶️ Game Over hidden!");

            // Hide canvas
            Canvas canvas = gameOverPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.transform.localScale = Vector3.zero;
                Debug.Log("📺 GameOver canvas scale set to hidden!");
            }
        }
    }

    public void OnRestartButtonClicked()
    {
        Debug.Log("🔄 GameOver Restart button clicked!");

        // Play button click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        // Force resume game state first
        Time.timeScale = 1f;
        isGameOver = false;

        // Hide panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Debug.Log("✅ Game resumed, panel hidden, now restarting level...");

        // Use coroutine for consistent timing
        StartCoroutine(RestartLevelDelayed());
    }

    private System.Collections.IEnumerator RestartLevelDelayed()
    {
        // Wait one frame to ensure state changes are applied
        yield return null;

        // Restart level
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
        else
        {
            // Fallback
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }

    public void OnHomeButtonClicked()
    {
        Debug.Log("🏠 GameOver Home button clicked!");
        Debug.Log($"🏠 Loading scene: {homeSceneName}");

        // Force resume game state first
        Time.timeScale = 1f;
        isGameOver = false;

        // Hide panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Debug.Log("✅ Game resumed, panel hidden, now loading scene...");

        // Use coroutine to ensure proper timing
        StartCoroutine(LoadHomeSceneDelayed());
    }

    private System.Collections.IEnumerator LoadHomeSceneDelayed()
    {
        // Wait one frame to ensure state changes are applied
        yield return null;

        Debug.Log("⏳ Loading home scene...");

        // Check if scene exists in build settings
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == homeSceneName)
            {
                sceneExists = true;
                break;
            }
        }

        if (sceneExists)
        {
            try
            {
                SceneManager.LoadScene(homeSceneName);
                Debug.Log("✅ Scene load initiated");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to load scene {homeSceneName}: {e.Message}");
                // Fallback to reload current scene
                string currentScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentScene);
            }
        }
        else
        {
            Debug.LogError($"❌ Scene '{homeSceneName}' not found in Build Settings!");
            Debug.Log("📋 Available scenes in Build Settings:");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"  - {sceneName}");
            }

            // Fallback to reload current scene
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
