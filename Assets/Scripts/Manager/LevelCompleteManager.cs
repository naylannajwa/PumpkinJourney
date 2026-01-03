using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    public static LevelCompleteManager Instance;
    
    [Header("UI References")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI coinText;
    public Button nextButton;
    public Button restartButton;
    public Button menuButton;
    
    [Header("Settings")]
    public string nextLevelScene = "gameplay2"; // Update ke gameplay2
    public string menuSceneName = "homePage";   // Update ke homePage
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ LevelCompleteManager initialized!");
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate LevelCompleteManager found! Destroying...");
            Destroy(gameObject);
        }
    }
    
    public static void EnsureInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<LevelCompleteManager>();
            
            if (Instance == null)
            {
                GameObject go = new GameObject("LevelCompleteManager");
                Instance = go.AddComponent<LevelCompleteManager>();
                Debug.Log("✅ LevelCompleteManager instance created!");
            }
        }
    }
    
    void Start()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
        
        Debug.Log("🎮 LevelCompleteManager ready - setup buttons via Inspector!");
    }
    
    public void ShowLevelComplete()
    {
        Debug.Log("🎯 ShowLevelComplete() called");

        // PAUSE GAME saat panel muncul
        Time.timeScale = 0f;
        Debug.Log("⏸️ Game paused for level complete screen");

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            Debug.Log("✅ Level complete panel activated");

            if (coinText != null && GameManager.Instance != null)
            {
                coinText.text = $"{GameManager.Instance.GetCoinCount()}";
                Debug.Log($"🪙 Coin display updated: {coinText.text}");
            }
            else
            {
                Debug.LogError("❌ Coin text or GameManager is null!");
            }

            DebugButtonState(nextButton, "Next");
            DebugButtonState(restartButton, "Restart");
            DebugButtonState(menuButton, "Menu");

            Debug.Log("🎉 Level Complete Screen Shown!");
        }
        else
        {
            Debug.LogError("❌ Level complete panel is null!");
        }
    }
    
    void DebugButtonState(Button button, string name)
    {
        if (button != null)
        {
            Debug.Log($"🔘 {name} Button - Interactable: {button.interactable}, Active: {button.gameObject.activeInHierarchy}, Listeners: {button.onClick.GetPersistentEventCount()}");
        }
        else
        {
            Debug.LogError($"❌ {name} Button is NULL!");
        }
    }
    
    public void HideLevelComplete()
    {
        Time.timeScale = 1f;
        
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
            Debug.Log("✅ Level complete panel hidden");
        }
    }
    
    public void OnNextButtonClicked()
    {
        Debug.Log("🎯 OnNextButtonClicked() - BUTTON CLICKED SUCCESSFULLY!");
        
        HideLevelComplete();
        
        // Load gameplay2 langsung (sesuai nama scene user)
        Debug.Log($"🎯 Loading next level: {nextLevelScene}");
        SceneManager.LoadScene(nextLevelScene);
    }
    
    public void OnRestartButtonClicked()
    {
        Debug.Log("🔄 OnRestartButtonClicked() - BUTTON CLICKED SUCCESSFULLY!");
        
        HideLevelComplete();
        
        // Restart current level
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"🔄 Restarting current level: {currentScene}");
        SceneManager.LoadScene(currentScene);
    }
    
    public void OnMenuButtonClicked()
    {
        Debug.Log("🏠 OnMenuButtonClicked() - BUTTON CLICKED SUCCESSFULLY!");
        
        HideLevelComplete();
        
        // Load homePage (sesuai nama scene user)
        Debug.Log($"🏠 Loading menu: {menuSceneName}");
        SceneManager.LoadScene(menuSceneName);
    }
}