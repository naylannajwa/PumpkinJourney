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

    [Header("Countdown UI")]
    [Tooltip("Text untuk menampilkan countdown 3, 2, 1")]
    public TextMeshProUGUI countdownText;
    [Tooltip("Panel untuk countdown background")]
    public GameObject countdownPanel;

    [Header("Countdown Settings")]
    [Tooltip("Ukuran maksimal animasi countdown (1 = normal size)")]
    public float countdownMaxScale = 2.0f;
    [Tooltip("Durasi animasi countdown dalam detik")]
    public float countdownAnimationDuration = 0.8f;

    [Header("Settings")]
    public string homeSceneName = "homePage";
    private bool isPaused = false;
    private bool isCountingDown = false;
    
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

        // Initialize countdown UI
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        Debug.Log("🎮 PauseManager ready!");
    }
    
    void Update()
    {
        // Pause dengan ESC atau P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused && !isCountingDown) // Don't allow resume if counting down
            {
                ResumeGame();
            }
            else if (!isPaused)
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

            // Pause BGM and play pause sound (interruptible)
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseBGM();
                AudioManager.Instance.PlayPauseSoundInterruptible();
            }

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

            // Stop pause sound, resume BGM and play start sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopSFX(); // Stop pause sound if still playing
                AudioManager.Instance.ResumeBGM();
                AudioManager.Instance.PlayStartSound();
            }

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

        // Play button click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        StartCoroutine(ResumeWithCountdown());
    }
    
    public void OnRestartButtonClicked()
    {
        Debug.Log("🔄 Restart button clicked!");

        // Play button click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

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

        // Note: AudioManager will automatically switch to HomePageBGM when homePage scene loads
        SceneManager.LoadScene(homeSceneName);
    }

    /// <summary>
    /// Coroutine untuk resume game dengan countdown 3, 2, 1
    /// </summary>
    private System.Collections.IEnumerator ResumeWithCountdown()
    {
        if (isCountingDown) yield break; // Prevent multiple countdowns

        isCountingDown = true;

        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Show countdown panel
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        // Countdown dari 3 ke 1
        for (int i = 3; i > 0; i--)
        {
            // Check if user pressed ESC/P to skip countdown
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("⏭️ Countdown skipped by user input!");
                break;
            }

            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                countdownText.gameObject.SetActive(true);

                // Animate scale effect
                StartCoroutine(AnimateCountdownNumber(countdownText.transform));
            }

            // Play countdown sound
            if (AudioManager.Instance != null)
            {
                // You can add a specific countdown sound here
                AudioManager.Instance.PlayButtonClickSound();
            }

            Debug.Log($"⏱️ Countdown: {i}");

            yield return new WaitForSecondsRealtime(1f); // Wait 1 second (unscaled time)
        }

        // Hide countdown
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        // Resume game
        ResumeGame();
        isCountingDown = false;

        Debug.Log("✅ Countdown complete, game resumed!");
    }

    /// <summary>
    /// Coroutine untuk animasi countdown number
    /// </summary>
    private System.Collections.IEnumerator AnimateCountdownNumber(Transform numberTransform)
    {
        if (numberTransform == null) yield break;

        float duration = countdownAnimationDuration;
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * countdownMaxScale;

        numberTransform.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Scale up then down
            if (t < 0.5f)
            {
                numberTransform.localScale = Vector3.Lerp(startScale, endScale, t * 2);
            }
            else
            {
                numberTransform.localScale = Vector3.Lerp(endScale, Vector3.one, (t - 0.5f) * 2);
            }

            yield return null;
        }

        numberTransform.localScale = Vector3.one;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}