using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Audio Manager untuk mengelola semua suara dalam game PumpkinJourney
/// Mengatur musik latar, efek suara, dan audio feedback
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    [Tooltip("Audio Mixer untuk kontrol volume")]
    public AudioMixer audioMixer;

    [Header("Background Music")]
    [Tooltip("Musik latar utama game")]
    public AudioClip mainBGM;
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
    [Tooltip("Suara langkah kaki")]
    public AudioClip footstepSound;
    [Tooltip("Suara collect candy")]
    public AudioClip candyCollectSound;
    [Tooltip("Suara power up")]
    public AudioClip powerUpSound;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    [Tooltip("Volume musik utama")]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    [Tooltip("Volume efek suara")]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    [Tooltip("Volume musik latar")]
    public float bgmVolume = 0.5f;

    // Private variables
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource ambientSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("🎵 AudioManager initialized!");
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate AudioManager found! Destroying...");
            Destroy(gameObject);
            return;
        }

        // Setup audio sources 
        SetupAudioSources();
    }

    void Start()
    {
        // Start main BGM
        PlayMainBGM();
        Debug.Log("🎵 AudioManager Start complete!");
    }

    /// <summary>
    /// Setup semua AudioSource yang dibutuhkan
    /// </summary>
    private void SetupAudioSources()
    {
        // BGM Source
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        bgmSource.outputAudioMixerGroup = audioMixer?.FindMatchingGroups("BGM")[0];

        // SFX Source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
        sfxSource.outputAudioMixerGroup = audioMixer?.FindMatchingGroups("SFX")[0];

        // Ambient Source
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.volume = sfxVolume * 0.3f;
        ambientSource.outputAudioMixerGroup = audioMixer?.FindMatchingGroups("Ambient")[0];
    }

    // =============================================
    // BACKGROUND MUSIC
    // =============================================

    /// <summary>
    /// Mainkan musik latar utama
    /// </summary>
    public void PlayMainBGM()
    {
        if (bgmSource != null && mainBGM != null)
        {
            bgmSource.clip = mainBGM;
            bgmSource.Play();
            Debug.Log("🎵 Main BGM started");
        }
    }

    /// <summary>
    /// Mainkan musik kuis
    /// </summary>
    public void PlayQuizBGM()
    {
        if (bgmSource != null && quizBGM != null)
        {
            bgmSource.clip = quizBGM;
            bgmSource.Play();
            Debug.Log("🎵 Quiz BGM started");
        }
    }

    /// <summary>
    /// Stop musik latar
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
            Debug.Log("🎵 BGM stopped");
        }
    }

    /// <summary>
    /// Pause musik latar
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Pause();
            Debug.Log("🎵 BGM paused");
        }
    }

    /// <summary>
    /// Resume musik latar
    /// </summary>
    public void ResumeBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.UnPause();
            Debug.Log("🎵 BGM resumed");
        }
    }

    // =============================================
    // PLAYER SOUNDS
    // =============================================

    /// <summary>
    /// Suara loncat
    /// </summary>
    public void PlayJumpSound()
    {
        PlaySFX(jumpSound, "Jump");
    }

    /// <summary>
    /// Suara mati
    /// </summary>
    public void PlayDeathSound()
    {
        PlaySFX(deathSound, "Death");
    }

    /// <summary>
    /// Suara slide
    /// </summary>
    public void PlaySlideSound()
    {
        PlaySFX(slideSound, "Slide");
    }

    /// <summary>
    /// Suara landing
    /// </summary>
    public void PlayLandSound()
    {
        PlaySFX(landSound, "Land");
    }

    /// <summary>
    /// Suara langkah kaki
    /// </summary>
    public void PlayFootstepSound()
    {
        PlaySFX(footstepSound, "Footstep");
    }

    // =============================================
    // UI SOUNDS
    // =============================================

    /// <summary>
    /// Suara klik tombol
    /// </summary>
    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound, "Button Click");
    }

    /// <summary>
    /// Suara hover tombol
    /// </summary>
    public void PlayButtonHoverSound()
    {
        PlaySFX(buttonHoverSound, "Button Hover");
    }

    /// <summary>
    /// Suara UI popup
    /// </summary>
    public void PlayUIPopupSound()
    {
        PlaySFX(uiPopupSound, "UI Popup");
    }

    /// <summary>
    /// Suara UI close
    /// </summary>
    public void PlayUICloseSound()
    {
        PlaySFX(uiCloseSound, "UI Close");
    }

    // =============================================
    // GAMEPLAY SOUNDS
    // =============================================

    /// <summary>
    /// Suara ambil kunci
    /// </summary>
    public void PlayKeyCollectSound()
    {
        PlaySFX(keyCollectSound, "Key Collect");
    }

    /// <summary>
    /// Suara pintu terkunci
    /// </summary>
    public void PlayDoorLockedSound()
    {
        PlaySFX(doorLockedSound, "Door Locked");
    }

    /// <summary>
    /// Suara pintu terbuka
    /// </summary>
    public void PlayDoorOpenSound()
    {
        PlaySFX(doorOpenSound, "Door Open");
    }

    /// <summary>
    /// Suara level complete
    /// </summary>
    public void PlayLevelCompleteSound()
    {
        PlaySFX(levelCompleteSound, "Level Complete");
    }

    /// <summary>
    /// Suara dapat poin
    /// </summary>
    public void PlayScoreSound()
    {
        PlaySFX(scoreSound, "Score");
    }

    /// <summary>
    /// Suara collect candy
    /// </summary>
    public void PlayCandyCollectSound()
    {
        PlaySFX(candyCollectSound, "Candy Collect");
    }

    /// <summary>
    /// Suara power up
    /// </summary>
    public void PlayPowerUpSound()
    {
        PlaySFX(powerUpSound, "Power Up");
    }

    // =============================================
    // QUIZ SOUNDS
    // =============================================

    /// <summary>
    /// Suara jawaban benar
    /// </summary>
    public void PlayCorrectAnswerSound()
    {
        PlaySFX(correctAnswerSound, "Correct Answer");
    }

    /// <summary>
    /// Suara jawaban salah
    /// </summary>
    public void PlayWrongAnswerSound()
    {
        PlaySFX(wrongAnswerSound, "Wrong Answer");
    }

    /// <summary>
    /// Suara kuis selesai
    /// </summary>
    public void PlayQuizCompleteSound()
    {
        PlaySFX(quizCompleteSound, "Quiz Complete");
    }

    /// <summary>
    /// Suara mulai kuis
    /// </summary>
    public void PlayQuizStartSound()
    {
        PlaySFX(quizStartSound, "Quiz Start");
    }

    /// <summary>
    /// Suara peringatan waktu
    /// </summary>
    public void PlayTimeWarningSound()
    {
        PlaySFX(timeWarningSound, "Time Warning");
    }

    // =============================================
    // ENVIRONMENT SOUNDS
    // =============================================

    /// <summary>
    /// Mainkan suara ambient angin
    /// </summary>
    public void PlayWindAmbient()
    {
        if (ambientSource != null && windAmbientSound != null)
        {
            ambientSource.clip = windAmbientSound;
            ambientSource.Play();
            Debug.Log("🌬️ Wind ambient started");
        }
    }

    /// <summary>
    /// Stop suara ambient
    /// </summary>
    public void StopAmbient()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
            Debug.Log("🌬️ Ambient stopped");
        }
    }

    // =============================================
    // UTILITY METHODS
    // =============================================

    /// <summary>
    /// Mainkan efek suara
    /// </summary>
    private void PlaySFX(AudioClip clip, string soundName)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
            Debug.Log($"🔊 SFX: {soundName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ SFX not played: {soundName} - Source or clip is null");
        }
    }

    /// <summary>
    /// Set master volume
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        }
        Debug.Log($"🔊 Master volume set to {masterVolume}");
    }

    /// <summary>
    /// Set SFX volume
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        if (ambientSource != null)
        {
            ambientSource.volume = sfxVolume * 0.3f;
        }
        Debug.Log($"🔊 SFX volume set to {sfxVolume}");
    }

    /// <summary>
    /// Set BGM volume
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
        Debug.Log($"🔊 BGM volume set to {bgmVolume}");
    }
}
