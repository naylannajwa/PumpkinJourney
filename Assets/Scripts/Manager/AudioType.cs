/// <summary>
/// Enum untuk semua jenis suara dalam game
/// Digunakan untuk mengidentifikasi audio clip yang akan dimainkan
/// </summary>
public enum AudioType
{
    // Background Music
    MainBGM,
    HomePageBGM,
    QuizBGM,

    // Player Sounds
    Jump,
    Death,
    Slide,
    Land,
    Footstep,

    // UI Sounds
    ButtonClick,
    ButtonHover,
    UIPopup,
    UIClose,

    // Game State Sounds
    Pause,
    Play,

    // Gameplay Sounds
    KeyCollect,
    DoorLocked,
    DoorOpen,
    LevelComplete,
    Score,
    CandyCollect,
    PowerUp,

    // Quiz Sounds
    CorrectAnswer,
    WrongAnswer,
    QuizComplete,
    QuizStart,
    TimeWarning,

    // Environment Sounds
    WindAmbient
}
