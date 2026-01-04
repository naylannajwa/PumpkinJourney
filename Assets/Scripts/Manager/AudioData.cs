using UnityEngine;

/// <summary>
/// ScriptableObject untuk menyimpan semua data audio dalam satu tempat
/// Mudah untuk assign audio clips langsung di Inspector
/// </summary>
[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/Audio Data", order = 1)]
public class AudioData : ScriptableObject
{
    [Header("Background Music")]
    [Tooltip("Musik latar utama game")]
    public AudioClip mainBGM;
    [Tooltip("Musik latar saat di Home Page")]
    public AudioClip homePageBGM;
    [Tooltip("Musik saat mengerjakan kuis")]
    public AudioClip quizBGM;

    [Header("Player Sounds")]
    [Tooltip("Suara karakter loncat")]
    public AudioClip jumpSound;
    [Tooltip("Suara karakter mati")]
    public AudioClip deathSound;
    [Tooltip("Suara karakter slide")]
    public AudioClip slideSound;
    [Tooltip("Suara karakter landing setelah loncat")]
    public AudioClip landSound;

    [Header("UI Sounds")]
    [Tooltip("Suara saat menekan tombol")]
    public AudioClip buttonClickSound;
    [Tooltip("Suara saat hover tombol")]
    public AudioClip buttonHoverSound;
    [Tooltip("Suara saat UI muncul")]
    public AudioClip uiPopupSound;
    [Tooltip("Suara saat UI hilang")]
    public AudioClip uiCloseSound;

    [Header("Game State Sounds")]
    [Tooltip("Suara saat game di-pause")]
    public AudioClip pauseSound;
    [Tooltip("Suara saat mulai bermain level")]
    public AudioClip playSound;

    [Header("Gameplay Sounds")]
    [Tooltip("Suara saat mengambil kunci")]
    public AudioClip keyCollectSound;
    [Tooltip("Suara saat pintu terkunci")]
    public AudioClip doorLockedSound;
    [Tooltip("Suara saat pintu terbuka")]
    public AudioClip doorOpenSound;
    [Tooltip("Suara saat mencapai pintu level selanjutnya")]
    public AudioClip levelCompleteSound;
    [Tooltip("Suara saat mendapat poin")]
    public AudioClip scoreSound;

    [Header("Quiz Sounds")]
    [Tooltip("Suara saat jawaban benar")]
    public AudioClip correctAnswerSound;
    [Tooltip("Suara saat jawaban salah")]
    public AudioClip wrongAnswerSound;
    [Tooltip("Suara saat berhasil menyelesaikan kuis")]
    public AudioClip quizCompleteSound;
    [Tooltip("Suara saat memulai kuis")]
    public AudioClip quizStartSound;
    [Tooltip("Suara saat time running out")]
    public AudioClip timeWarningSound;

    [Header("Environment Sounds")]
    [Tooltip("Suara angin atau ambient")]
    public AudioClip windAmbientSound;

    [Header("Game Over Sounds")]
    [Tooltip("Suara saat game over")]
    public AudioClip gameOverSound;
    [Tooltip("Suara langkah kaki")]
    public AudioClip footstepSound;
    [Tooltip("Suara collect candy")]
    public AudioClip candyCollectSound;
    [Tooltip("Suara power up")]
    public AudioClip powerUpSound;
}
