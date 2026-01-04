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
    }
}
