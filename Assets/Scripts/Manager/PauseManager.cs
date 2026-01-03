using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    
    [Header("UI References")]
    public GameObject pausePanel;
    public TextMeshProUGUI pauseText;
    public Button resumeButton;
    public Button restartButton;
    public Button homeButton;
    
    [Header("Settings")]
    public string homeSceneName = "homePage";
    private bool isPaused = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
            Debug.Log("✅ PauseManager initialized!");
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate PauseManager found! Destroying...");
            Destroy(gameObject);
        }
    }
    
    public static void EnsureInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<PauseManager>();
            
            if (Instance == null)
            {
                GameObject go = new GameObject("PauseManager");
                Instance = go.AddComponent<PauseManager>();
                Debug.Log("✅ PauseManager instance created!");
            }
        }
    }
    
    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);

            // Ensure canvas is hidden at start
            Canvas canvas = pausePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.transform.localScale = Vector3.zero;
                Debug.Log("📺 Canvas hidden at start!");
            }
        }

        if (pauseText != null)
        {
            pauseText.text = "GAME PAUSED";
        }
        
        Debug.Log("🎮 PauseManager ready!");
    }
    
    void Update()
    {
        // Pause dengan ESC atau P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f; // Pause game

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Debug.Log("⏸️ Game Paused!");
            
            // Animasi akan otomatis berjalan karena Animator component
            // JANGAN set canvas scale manual agar animasi terlihat
        }
    }
        
    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f; // Resume game

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Debug.Log("▶️ Game Resumed!");

            // Hide canvas
            Canvas canvas = pausePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.transform.localScale = Vector3.zero;
                Debug.Log("📺 Canvas scale set to hidden!");
            }
        }
    }
    
    public void OnResumeButtonClicked()
    {
        Debug.Log("▶️ Resume button clicked!");
        ResumeGame();
    }
    
    public void OnRestartButtonClicked()
    {
        Debug.Log("🔄 Restart button clicked!");
        ResumeGame(); // Resume dulu sebelum restart
        
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
        Debug.Log("🏠 Home button clicked!");
        ResumeGame(); // Resume dulu sebelum pindah scene
        
        SceneManager.LoadScene(homeSceneName);
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}