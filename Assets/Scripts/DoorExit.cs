using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorExit : MonoBehaviour
{
    [Header("Settings")]
    public string nextLevelScene = "Level2";
    public int levelToUnlock = 2;

    [Header("Door Sprites")]
    public GameObject doorClosedSprite;  // Sprite pintu tertutup
    public GameObject doorOpenSprite;    // Sprite pintu terbuka

    [Header("Glow Effect")]
    public SpriteRenderer doorGlowRenderer; // SpriteRenderer untuk efek cahaya pintu
    public Color glowColor = Color.yellow; // Warna cahaya saat mendekat
    public float glowIntensity = 0.5f; // Intensitas cahaya

    [Header("Auto Panels")]
    public GameObject doorLockedPanel;   // Panel "Cari kunci dulu"
    public GameObject doorUnlockedPanel; // Panel "Tekan E untuk masuk"

    private bool playerNearby = false;

    void Start()
    {
        // Set door state berdasarkan key status
        UpdateDoorVisual();
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[DoorExit] OnTriggerEnter2D: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            // Update door visual (untuk handle case player dapat kunci lalu balik ke door)
            UpdateDoorVisual();

            // Enable glow effect
            if (doorGlowRenderer != null)
            {
                doorGlowRenderer.color = glowColor * glowIntensity;
                doorGlowRenderer.gameObject.SetActive(true);
                Debug.Log("[DoorExit] ✨ Door glow activated!");
            }

            // Cek apakah punya kunci
            bool hasKey = GameManager.Instance != null && GameManager.Instance.HasKey();
            
            if (hasKey)
            {
                // SUDAH punya kunci - LANGSUNG SHOW LEVEL COMPLETE!
                Debug.Log("[DoorExit] 🎉 Player has key! Showing level complete screen...");
                
                // Hide semua panel lama
                if (doorLockedPanel != null)
                    doorLockedPanel.SetActive(false);
                if (doorUnlockedPanel != null)
                    doorUnlockedPanel.SetActive(false);
                
                // Stop player movement (optional)
                PumpkinMovement player = other.GetComponent<PumpkinMovement>();
                if (player != null)
                {
                    // Disable player input atau set velocity = 0
                    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                    }
                }
                
                // Show level complete screen
                LevelCompleteManager.EnsureInstance(); // Pastikan instance ada

                if (LevelCompleteManager.Instance != null)
                {
                    LevelCompleteManager.Instance.ShowLevelComplete();
                    Debug.Log("[DoorExit] ✅ Level complete screen shown!");
                }
                else
                {
                    // Fallback: cari di scene
                    LevelCompleteManager lcm = FindObjectOfType<LevelCompleteManager>();
                    if (lcm != null)
                    {
                        lcm.ShowLevelComplete();
                        Debug.Log("[DoorExit] ✅ Level complete found via FindObjectOfType!");
                    }
                    else
                    {
                        Debug.LogError("[DoorExit] ❌ LevelCompleteManager not found in scene!");
                        // Emergency fallback: load next level langsung
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.LoadLevel("gameplay2");
                        }
                    }
                }
            }
            else
            {
                // Belum punya kunci - show "Cari kunci dulu"
                if (doorLockedPanel != null)
                {
                    doorLockedPanel.SetActive(true);
                    Debug.Log("[DoorExit] 🔒 Door locked panel shown!");
                }

                if (doorUnlockedPanel != null)
                {
                    doorUnlockedPanel.SetActive(false);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"[DoorExit] OnTriggerExit2D: {other.gameObject.name}");

        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            // Disable glow effect
            if (doorGlowRenderer != null)
            {
                doorGlowRenderer.gameObject.SetActive(false);
                Debug.Log("[DoorExit] ✨ Door glow deactivated!");
            }

            // Hide all panels
            if (doorLockedPanel != null)
            {
                doorLockedPanel.SetActive(false);
            }

            if (doorUnlockedPanel != null)
            {
                doorUnlockedPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Update visual pintu berdasarkan key status
    /// </summary>
    void UpdateDoorVisual()
    {
        // Ensure GameManager exists
        GameManager.EnsureInstance();

        if (GameManager.Instance == null) return;

        bool hasKey = GameManager.Instance.HasKey();

        // Show/Hide door sprites
        if (doorClosedSprite != null)
        {
            doorClosedSprite.SetActive(!hasKey); // Tertutup jika BELUM punya kunci
        }

        if (doorOpenSprite != null)
        {
            doorOpenSprite.SetActive(hasKey); // Terbuka jika SUDAH punya kunci
        }

        Debug.Log($"[DoorExit] Door visual updated - HasKey: {hasKey}");
    }

    void TryOpenDoor()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[DoorExit] GameManager.Instance is NULL!");
            return;
        }

        Debug.Log($"[DoorExit] TryOpenDoor - HasKey: {GameManager.Instance.HasKey()}");

        if (GameManager.Instance.HasKey())
        {
            // Unlock next level
            PlayerPrefs.SetInt("Level" + levelToUnlock + "Unlocked", 1);
            PlayerPrefs.Save();

            Debug.Log($"[DoorExit] Loading {nextLevelScene}...");
            GameManager.Instance.LoadLevel(nextLevelScene);
        }
        else
        {
            Debug.Log("[DoorExit] Player doesn't have key!");
        }
    }

    /// <summary>
    /// Public method untuk update door dari luar (dipanggil setelah dapat kunci)
    /// </summary>
    public void OnKeyCollected()
    {
        UpdateDoorVisual();
    }
}