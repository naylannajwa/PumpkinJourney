using UnityEngine;

public class TestQuizSystem : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🧪 TESTING QUIZ SYSTEM...");

        // Test 1: Load QuizData
        QuizData quizData = Resources.Load<QuizData>("QuizData");
        if (quizData == null)
        {
            Debug.LogError("❌ QuizData tidak ditemukan di Resources!");
            return;
        }
        Debug.Log("✅ QuizData loaded successfully");

        // Test 2: Load questions from text file
        quizData.LoadFromTextFile();

        // Test 3: Check questions count
        if (quizData.questions == null)
        {
            Debug.LogError("❌ Questions array is null!");
            return;
        }

        Debug.Log($"✅ Total questions loaded: {quizData.questions.Length}");

        // Test 4: Test level filtering
        for (int level = 1; level <= 4; level++)
        {
            quizData.SetLevel(level);
            // LoadFromTextFile() sudah dipanggil di SetLevel()

            int expectedStart = (level - 1) * quizData.questionsPerLevel;
            int expectedCount = Mathf.Min(quizData.questionsPerLevel, quizData.questions.Length);

            Debug.Log($"🎯 Level {level}: Expected {expectedCount} questions, Got {quizData.questions.Length} questions");

            if (quizData.questions.Length > 0)
            {
                string[] answers = {quizData.questions[0].answerA, quizData.questions[0].answerB,
                                   quizData.questions[0].answerC, quizData.questions[0].answerD};
                string correctAnswer = answers[quizData.questions[0].correctAnswerIndex];

                Debug.Log($"   📝 Sample question: {quizData.questions[0].question}");
                Debug.Log($"   🔍 Correct answer index: {quizData.questions[0].correctAnswerIndex} ({correctAnswer})");

                // Show all options
                Debug.Log($"   📋 Options: A:{quizData.questions[0].answerA}, B:{quizData.questions[0].answerB}, C:{quizData.questions[0].answerC}, D:{quizData.questions[0].answerD}");
            }
        }

        Debug.Log("✅ QUIZ SYSTEM TEST COMPLETED!");

        // Test 5: Test Quiz System Only
        // Audio testing moved to separate script
    }

    private void TestAudioSystem()
    {
        Debug.Log("🔊 TESTING AUDIO SYSTEM ON WINDOWS...");

        // Check if AudioManager exists
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager Instance tidak ditemukan!");
            return;
        }
        Debug.Log("✅ AudioManager Instance ditemukan");

        // Check if AudioData is assigned
        if (AudioManager.Instance.audioData == null)
        {
            Debug.LogError("❌ AudioData belum di-assign ke AudioManager!");
            return;
        }
        Debug.Log("✅ AudioData di-assign ke AudioManager");

        // Check basic Unity audio settings
        CheckUnityAudioSettings();

        // Test direct AudioSource playback
        TestDirectAudioPlayback();

        Debug.Log("✅ AUDIO SYSTEM TEST COMPLETED!");
    }

    private void CheckUnityAudioSettings()
    {
        Debug.Log("🔧 CHECKING UNITY AUDIO SETTINGS...");

        // Check if audio is globally muted
        Debug.Log($"AudioListener.pause: {AudioListener.pause}");
        Debug.Log($"AudioListener.volume: {AudioListener.volume}");

        // Check platform specific settings
        Debug.Log($"Application.platform: {Application.platform}");
        Debug.Log($"SystemInfo.operatingSystem: {SystemInfo.operatingSystem}");
    }

    private void TestDirectAudioPlayback()
    {
        Debug.Log("🎵 TESTING DIRECT AUDIO PLAYBACK...");

        var audioData = AudioManager.Instance.audioData;

        // Test MainBGM directly
        if (audioData.mainBGM != null)
        {
            Debug.Log("🎵 Playing MainBGM directly...");
            AudioSource.PlayClipAtPoint(audioData.mainBGM, Vector3.zero, 1.0f);
        }
        else
        {
            Debug.LogError("❌ MainBGM clip is NULL!");
        }

        // Test Pause sound directly
        if (audioData.pauseSound != null)
        {
            Debug.Log("⏸️ Playing Pause sound directly...");
            StartCoroutine(PlayPauseSoundDelayed(audioData.pauseSound));
        }
        else
        {
            Debug.LogError("❌ Pause sound clip is NULL!");
        }
    }

    private System.Collections.IEnumerator PlayPauseSoundDelayed(AudioClip clip)
    {
        yield return new WaitForSeconds(2f);
        AudioSource.PlayClipAtPoint(clip, Vector3.zero, 1.0f);
        Debug.Log("✅ Pause sound played directly!");
    }
}
