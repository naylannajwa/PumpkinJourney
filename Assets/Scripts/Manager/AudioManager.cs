using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Audio Manager untuk mengelola semua suara dalam game PumpkinJourney
/// Mengatur musik latar, efek suara, dan audio feedback
/// Sekarang menggunakan sistem ScriptableObject untuk kemudahan assign audio
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Configuration")]
    [Tooltip("Audio Mixer untuk kontrol volume")]
    public AudioMixer audioMixer;
    [Tooltip("ScriptableObject yang berisi semua audio clips")]
    public AudioData audioData;

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
    private string lastSceneName;

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

        // Validasi AudioData
        if (audioData == null)
        {
            Debug.LogError("❌ AudioData belum di-assign! Silakan assign AudioData di Inspector.");
            return;
        }

        // Setup audio sources
        SetupAudioSources();

        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Check current scene and play appropriate BGM
        UpdateBGMForCurrentScene();
        Debug.Log("🎵 AudioManager Start complete!");
    }

    /// <summary>
    /// Called when a scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🎵 Scene loaded: {scene.name}");
        UpdateBGMForCurrentScene();
    }

    /// <summary>
    /// Update BGM based on current scene
    /// </summary>
    private void UpdateBGMForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if this is the same scene (level restart)
        bool isSceneRestart = (lastSceneName == currentSceneName);
        lastSceneName = currentSceneName;

        switch (currentSceneName)
        {
            case "homePage":
                if (audioData != null && audioData.homePageBGM != null)
                {
                    PlayHomePageBGM();
                    Debug.Log("🎵 Playing HomePageBGM for homePage scene");
                }
                break;

            case "gameplay":
            case "gameplay2":
            case "gameplay3":
            case "gameplay4":
                if (audioData != null && audioData.mainBGM != null)
                {
                    // If restarting the same scene, don't restart BGM
                    if (!isSceneRestart)
                    {
                        PlayMainBGM();
                        Debug.Log("🎵 Playing MainBGM for gameplay scene");
                    }
                    else
                    {
                        Debug.Log("🎵 Keeping current MainBGM position (scene restart)");
                    }
                }
                break;

            default:
                Debug.Log($"🎵 No specific BGM for scene: {currentSceneName}");
                break;
        }
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

    /// <summary>
    /// Mendapatkan AudioClip berdasarkan AudioType
    /// </summary>
    private AudioClip GetAudioClip(AudioType audioType)
    {
        if (audioData == null)
        {
            Debug.LogError($"❌ AudioData belum di-assign untuk {audioType}");
            return null;
        }

        AudioClip clip = null;

        switch (audioType)
        {
            case AudioType.MainBGM:
                clip = audioData.mainBGM;
                break;
            case AudioType.HomePageBGM:
                clip = audioData.homePageBGM;
                break;
            case AudioType.QuizBGM:
                clip = audioData.quizBGM;
                break;
            case AudioType.Jump:
                clip = audioData.jumpSound;
                break;
            case AudioType.Death:
                clip = audioData.deathSound;
                break;
            case AudioType.Slide:
                clip = audioData.slideSound;
                break;
            case AudioType.Land:
                clip = audioData.landSound;
                break;
            case AudioType.Footstep:
                clip = audioData.footstepSound;
                break;
            case AudioType.ButtonClick:
                clip = audioData.buttonClickSound;
                break;
            case AudioType.ButtonHover:
                clip = audioData.buttonHoverSound;
                break;
            case AudioType.UIPopup:
                clip = audioData.uiPopupSound;
                break;
            case AudioType.UIClose:
                clip = audioData.uiCloseSound;
                break;
            case AudioType.Pause:
                clip = audioData.pauseSound;
                break;
            case AudioType.Play:
                clip = audioData.playSound;
                break;
            case AudioType.KeyCollect:
                clip = audioData.keyCollectSound;
                break;
            case AudioType.DoorLocked:
                clip = audioData.doorLockedSound;
                break;
            case AudioType.DoorOpen:
                clip = audioData.doorOpenSound;
                break;
            case AudioType.LevelComplete:
                clip = audioData.levelCompleteSound;
                break;
            case AudioType.Score:
                clip = audioData.scoreSound;
                break;
            case AudioType.CandyCollect:
                clip = audioData.candyCollectSound;
                break;
            case AudioType.PowerUp:
                clip = audioData.powerUpSound;
                break;
            case AudioType.CorrectAnswer:
                clip = audioData.correctAnswerSound;
                break;
            case AudioType.WrongAnswer:
                clip = audioData.wrongAnswerSound;
                break;
            case AudioType.QuizComplete:
                clip = audioData.quizCompleteSound;
                break;
            case AudioType.QuizStart:
                clip = audioData.quizStartSound;
                break;
            case AudioType.TimeWarning:
                clip = audioData.timeWarningSound;
                break;
            case AudioType.WindAmbient:
                clip = audioData.windAmbientSound;
                break;
            case AudioType.GameOverSound:
                clip = audioData.gameOverSound;
                break;
            case AudioType.EnemyHit:
                clip = audioData.enemyHitSound;
                break;
            case AudioType.EnemyDeath:
                clip = audioData.enemyDeathSound;
                break;
            case AudioType.PlayerHurt:
                clip = audioData.playerHurtSound;
                break;
            default:
                Debug.LogWarning($"⚠️ AudioType {audioType} tidak dikenali");
                break;
        }

        if (clip == null)
        {
            Debug.LogWarning($"⚠️ AudioClip untuk {audioType} belum di-assign di AudioData");
        }

        return clip;
    }

    /// <summary>
    /// Mainkan suara berdasarkan AudioType
    /// Metode utama untuk memainkan semua jenis suara
    /// </summary>
    public void PlaySound(AudioType audioType)
    {
        AudioClip clip = GetAudioClip(audioType);

        if (clip != null)
        {
            // Cek apakah ini BGM atau Ambient
            if (audioType == AudioType.MainBGM || audioType == AudioType.HomePageBGM || audioType == AudioType.QuizBGM)
            {
                PlayBGM(clip, audioType.ToString());
            }
            else if (audioType == AudioType.WindAmbient)
            {
                PlayAmbient(clip, audioType.ToString());
            }
            else
            {
                PlaySFX(clip, audioType.ToString());
            }
        }
    }

    /// <summary>
    /// Mainkan musik latar (BGM)
    /// </summary>
    private void PlayBGM(AudioClip clip, string bgmName)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
            Debug.Log($"🎵 BGM: {bgmName} started");
        }
    }

    /// <summary>
    /// Mainkan suara ambient
    /// </summary>
    private void PlayAmbient(AudioClip clip, string ambientName)
    {
        if (ambientSource != null && clip != null)
        {
            ambientSource.clip = clip;
            ambientSource.Play();
            Debug.Log($"🌬️ Ambient: {ambientName} started");
        }
    }

    // =============================================
    // BACKGROUND MUSIC
    // =============================================

    /// <summary>
    /// Mainkan musik latar utama
    /// </summary>
    public void PlayMainBGM()
    {
        PlaySound(AudioType.MainBGM);
    }

    /// <summary>
    /// Mainkan musik kuis
    /// </summary>
    public void PlayQuizBGM()
    {
        PlaySound(AudioType.QuizBGM);
    }

    /// <summary>
    /// Mainkan musik home page
    /// </summary>
    public void PlayHomePageBGM()
    {
        PlaySound(AudioType.HomePageBGM);
    }

    /// <summary>
    /// Mainkan suara pause
    /// </summary>
    public void PlayPauseSound()
    {
        PlaySound(AudioType.Pause);
    }

    /// <summary>
    /// Mainkan suara play/start level
    /// </summary>
    public void PlayStartSound()
    {
        PlaySound(AudioType.Play);
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
        PlaySound(AudioType.Jump);
    }

    /// <summary>
    /// Suara mati
    /// </summary>
    public void PlayDeathSound()
    {
        PlaySound(AudioType.Death);
    }

    /// <summary>
    /// Suara slide
    /// </summary>
    public void PlaySlideSound()
    {
        PlaySound(AudioType.Slide);
    }

    /// <summary>
    /// Suara landing
    /// </summary>
    public void PlayLandSound()
    {
        PlaySound(AudioType.Land);
    }

    /// <summary>
    /// Suara langkah kaki
    /// </summary>
    public void PlayFootstepSound()
    {
        PlaySound(AudioType.Footstep);
    }

    // =============================================
    // UI SOUNDS
    // =============================================

    /// <summary>
    /// Suara klik tombol
    /// </summary>
    public void PlayButtonClickSound()
    {
        PlaySound(AudioType.ButtonClick);
    }

    /// <summary>
    /// Suara hover tombol
    /// </summary>
    public void PlayButtonHoverSound()
    {
        PlaySound(AudioType.ButtonHover);
    }

    /// <summary>
    /// Suara UI popup
    /// </summary>
    public void PlayUIPopupSound()
    {
        PlaySound(AudioType.UIPopup);
    }

    /// <summary>
    /// Suara UI close
    /// </summary>
    public void PlayUICloseSound()
    {
        PlaySound(AudioType.UIClose);
    }

    // =============================================
    // GAMEPLAY SOUNDS
    // =============================================

    /// <summary>
    /// Suara ambil kunci
    /// </summary>
    public void PlayKeyCollectSound()
    {
        PlaySound(AudioType.KeyCollect);
    }

    /// <summary>
    /// Suara pintu terkunci
    /// </summary>
    public void PlayDoorLockedSound()
    {
        PlaySound(AudioType.DoorLocked);
    }

    /// <summary>
    /// Suara pintu terbuka
    /// </summary>
    public void PlayDoorOpenSound()
    {
        PlaySound(AudioType.DoorOpen);
    }

    /// <summary>
    /// Suara level complete
    /// </summary>
    public void PlayLevelCompleteSound()
    {
        PlaySound(AudioType.LevelComplete);
    }

    /// <summary>
    /// Suara dapat poin
    /// </summary>
    public void PlayScoreSound()
    {
        PlaySound(AudioType.Score);
    }

    /// <summary>
    /// Suara collect candy
    /// </summary>
    public void PlayCandyCollectSound()
    {
        PlaySound(AudioType.CandyCollect);
    }

    /// <summary>
    /// Suara power up
    /// </summary>
    public void PlayPowerUpSound()
    {
        PlaySound(AudioType.PowerUp);
    }

    // =============================================
    // QUIZ SOUNDS
    // =============================================

    /// <summary>
    /// Suara jawaban benar
    /// </summary>
    public void PlayCorrectAnswerSound()
    {
        PlaySound(AudioType.CorrectAnswer);
    }

    /// <summary>
    /// Suara jawaban salah
    /// </summary>
    public void PlayWrongAnswerSound()
    {
        PlaySound(AudioType.WrongAnswer);
    }

    /// <summary>
    /// Suara kuis selesai
    /// </summary>
    public void PlayQuizCompleteSound()
    {
        PlaySound(AudioType.QuizComplete);
    }

    /// <summary>
    /// Suara mulai kuis
    /// </summary>
    public void PlayQuizStartSound()
    {
        PlaySound(AudioType.QuizStart);
    }

    /// <summary>
    /// Suara peringatan waktu
    /// </summary>
    public void PlayTimeWarningSound()
    {
        PlaySound(AudioType.TimeWarning);
    }

    // =============================================
    // ENVIRONMENT SOUNDS
    // =============================================

    /// <summary>
    /// Mainkan suara ambient angin
    /// </summary>
    public void PlayWindAmbient()
    {
        PlaySound(AudioType.WindAmbient);
    }

    // =============================================
    // GAME OVER SOUNDS
    // =============================================

    /// <summary>
    /// Mainkan suara game over
    /// </summary>
    public void PlayGameOverSound()
    {
        PlaySound(AudioType.GameOverSound);
    }

    // =============================================
    // ENEMY SOUNDS
    // =============================================

    /// <summary>
    /// Mainkan suara enemy terkena damage
    /// </summary>
    public void PlayEnemyHitSound()
    {
        PlaySound(AudioType.EnemyHit);
    }

    /// <summary>
    /// Mainkan suara enemy mati
    /// </summary>
    public void PlayEnemyDeathSound()
    {
        PlaySound(AudioType.EnemyDeath);
    }

    /// <summary>
    /// Mainkan suara player terkena damage
    /// </summary>
    public void PlayPlayerHurtSound()
    {
        PlaySound(AudioType.PlayerHurt);
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

    void OnDestroy()
    {
        // Unsubscribe from scene change events
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    /// <summary>
    /// Stop all SFX
    /// </summary>
    public void StopSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
            Debug.Log("🔊 SFX stopped");
        }
    }

    /// <summary>
    /// Play pause sound (can be stopped)
    /// </summary>
    public void PlayPauseSoundInterruptible()
    {
        AudioClip clip = GetAudioClip(AudioType.Pause);
        if (sfxSource != null && clip != null)
        {
            sfxSource.clip = clip;
            sfxSource.loop = false; // Ensure not looping
            sfxSource.volume = sfxVolume; // Set volume
            sfxSource.Play();
            Debug.Log("⏸️ Pause sound started (interruptible)");
        }
    }
}
