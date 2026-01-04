using System.Collections;
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1; // Nilai koin
    public float fadeSpeed = 2f; // Kecepatan fade
    public float moveUpSpeed = 3f; // Kecepatan naik ke atas
    public float fadeDuration = 1f; // Durasi efek fade
    
    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Coin needs SpriteRenderer component!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah yang menyentuh adalah Player
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectCoin();
        }
    }
    
    void CollectCoin()
    {
        isCollected = true;

        // Play score sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound();
        }

        // Tambahkan koin ke GameManager (kita akan tambahkan method ini)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(coinValue);
        }

        // Mulai efek fade dan naik ke atas
        StartCoroutine(FadeAndMoveUp());

        // Disable collider agar tidak bisa dikumpul lagi
        GetComponent<Collider2D>().enabled = false;
    }
    
    IEnumerator FadeAndMoveUp()
    {
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;
        Vector3 startPosition = transform.position;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            
            // Fade out (alpha dari 1 ke 0)
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = newColor;
            
            // Move up
            transform.position = startPosition + Vector3.up * (t * moveUpSpeed);
            
            yield return null;
        }
        
        // Destroy object setelah efek selesai
        Destroy(gameObject);
    }
}