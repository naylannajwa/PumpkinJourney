using UnityEngine;
using UnityEngine.UI; // Tambahkan untuk Button
using TMPro;

/// <summary>
/// UIManager untuk game platformer Labu
/// Mengatur semua UI dalam game
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Interact Prompt (Press E)")]
    [Tooltip("GameObject panel yang berisi icon dan text")]
    public GameObject pressEPanel;
    
    [Tooltip("TextMeshPro untuk text 'Tekan E untuk...' (OPTIONAL)")]
    public TextMeshProUGUI pressEText;

    [Header("Pause Button")] // TAMBAH INI
    [Tooltip("Tombol pause di UI gameplay")]
    public Button pauseButton;

    [Header("Key Collected Notification")]
    [Tooltip("Panel untuk notifikasi kunci dikumpulkan")]
    public GameObject keyCollectedPanel;

    [Tooltip("Durasi notifikasi muncul (detik)")]
    public float notificationDuration = 2f;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
            Debug.Log("✅ UIManager initialized!");
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate UIManager found! Destroying...");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Hide Press E panel at start
        HidePressEIcon();
        
        // Setup pause button - TAMBAH INI
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
            Debug.Log("🔘 Pause button setup complete!");
        }
        
        Debug.Log("✅ UIManager Start complete!");
    }
    
    // =============================================
    // PRESS E ICON - Show/Hide dengan optional text
    // =============================================
    
    /// <summary>
    /// Tampilkan Press E panel (gambar + optional text)
    /// </summary>
    public void ShowPressEIcon()
    {
        if (pressEPanel != null)
        {
            pressEPanel.SetActive(true);
            Debug.Log("⌨️ Press E panel shown!");
        }
        else
        {
            Debug.LogError("❌ Press E Panel is NULL! Assign di Inspector!");
        }
    }
    
    /// <summary>
    /// Tampilkan Press E panel dengan custom text
    /// </summary>
    /// <param name="message">Text yang ditampilkan (jika ada TextMeshPro)</param>
    public void ShowPressEIcon(string message)
    {
        if (pressEPanel != null)
        {
            pressEPanel.SetActive(true);
            
            // Update text jika ada
            if (pressEText != null)
            {
                pressEText.text = message;
            }
            
            Debug.Log($"⌨️ Press E panel shown with message: {message}");
        }
        else
        {
            Debug.LogError("❌ Press E Panel is NULL! Assign di Inspector!");
        }
    }
    
    /// <summary>
    /// Sembunyikan Press E panel
    /// </summary>
    public void HidePressEIcon()
    {
        if (pressEPanel != null)
        {
            pressEPanel.SetActive(false);
            Debug.Log("⌨️ Press E panel hidden!");
        }
    }
    
    // =============================================
    // PAUSE BUTTON - TAMBAH INI
    // =============================================
    
    /// <summary>
    /// Handler untuk tombol pause di UI
    /// </summary>
    public void OnPauseButtonClicked()
    {
        Debug.Log("🔘 Pause button clicked!");
        
        // Panggil PauseManager untuk pause game
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.PauseGame();
        }
        else
        {
            Debug.LogError("❌ PauseManager not found!");
        }
    }
    
    // =============================================
    // MESSAGE - Optional feedback messages
    // =============================================
    
    /// <summary>
    /// Tampilkan message/feedback ke player
    /// Untuk sementara hanya log ke Console
    /// Bisa dikembangkan dengan panel message terpisah nanti
    /// </summary>
    /// <param name="message">Message yang ingin ditampilkan</param>
    public void ShowMessage(string message)
    {
        Debug.Log($"💬 Message: {message}");

        // TODO: Jika nanti mau bikin panel message terpisah, bisa ditambahkan di sini
        // Contoh:
        // if (messagePanel != null)
        // {
        //     messagePanel.SetActive(true);
        //     messageText.text = message;
        //     StartCoroutine(HideMessageAfterDelay(2f));
        // }
    }

    // =============================================
    // KEY COLLECTED NOTIFICATION
    // =============================================

    /// <summary>
    /// Tampilkan notifikasi kunci dikumpulkan dengan animasi
    /// </summary>
    public void ShowKeyCollectedNotification()
    {
        if (keyCollectedPanel != null)
        {
            keyCollectedPanel.SetActive(true);

            // Animate scale
            StartCoroutine(AnimateNotification());

            // Hide after duration
            Invoke("HideKeyCollectedNotification", notificationDuration);
        }
    }

    /// <summary>
    /// Coroutine untuk animasi notifikasi muncul
    /// </summary>
    private System.Collections.IEnumerator AnimateNotification()
    {
        Transform panel = keyCollectedPanel.transform;
        float duration = 0.3f;
        float elapsed = 0;

        panel.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            panel.localScale = Vector3.one * Mathf.SmoothStep(0, 1, t);
            yield return null;
        }

        panel.localScale = Vector3.one;
    }

    /// <summary>
    /// Sembunyikan notifikasi kunci dikumpulkan
    /// </summary>
    private void HideKeyCollectedNotification()
    {
        if (keyCollectedPanel != null)
        {
            keyCollectedPanel.SetActive(false);
        }
    }
}