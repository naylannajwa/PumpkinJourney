using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Auto Panel")]
    public GameObject keyPromptPanel; // Panel gambar "Tekan E untuk ambil kunci"
    
    private bool playerNearby = false;
    
    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ShowQuiz();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[KeyItem] Trigger Enter: {other.name}, Tag: {other.tag}");
        
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            
            // Show panel otomatis
            if (keyPromptPanel != null)
            {
                keyPromptPanel.SetActive(true);
                Debug.Log("[KeyItem] 📢 Key prompt panel shown!");
            }
            else
            {
                Debug.LogError("[KeyItem] ❌ Key Prompt Panel is NULL!");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            
            // Hide panel
            if (keyPromptPanel != null)
            {
                keyPromptPanel.SetActive(false);
            }
        }
    }
    
    void ShowQuiz()
    {
        // Ensure instances exist
        QuizManager.EnsureInstance();
        GameManager.EnsureInstance();

        if (QuizManager.Instance == null)
        {
            Debug.LogError("[KeyItem] ❌ QuizManager.Instance is NULL!");
            return;
        }

        // Hide panel
        if (keyPromptPanel != null)
        {
            keyPromptPanel.SetActive(false);
        }

        // Show quiz
        QuizManager.Instance.ShowQuiz(OnQuizComplete);

        Debug.Log("[KeyItem] 🎯 Quiz started!");
    }
    
    void OnQuizComplete(bool success)
    {
        Debug.Log($"[KeyItem] Quiz complete! Success: {success}");
        
        if (success)
        {
            // Give key to player
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectKey();
            }

            // Play key collect sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayKeyCollectSound();
            }

            // Destroy key object
            Destroy(gameObject);

            Debug.Log("[KeyItem] ✅ Key collected!");
        }
        else
        {
            Debug.Log("[KeyItem] ❌ Quiz failed!");
        }
    }
}