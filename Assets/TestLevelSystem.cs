using UnityEngine;

public class TestLevelSystem : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🧪 TESTING LEVEL SYSTEM...");

        // Test 1: Check GameManager level
        if (GameManager.Instance != null)
        {
            int gmLevel = GameManager.Instance.GetCurrentLevel();
            Debug.Log($"✅ GameManager level: {gmLevel}");
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null!");
        }

        // Test 2: Check QuizManager and QuizData
        if (QuizManager.Instance != null)
        {
            Debug.Log("✅ QuizManager exists");

            if (QuizManager.Instance.quizData != null)
            {
                int quizLevel = QuizManager.Instance.quizData.currentLevel;
                int questionCount = QuizManager.Instance.quizData.questions?.Length ?? 0;

                Debug.Log($"✅ QuizData level: {quizLevel}");
                Debug.Log($"✅ Questions count: {questionCount}");

                if (questionCount > 0 && QuizManager.Instance.quizData.questions != null)
                {
                    Debug.Log($"📝 First question: {QuizManager.Instance.quizData.questions[0].question}");
                }
            }
            else
            {
                Debug.LogError("❌ QuizManager.quizData is null!");
            }
        }
        else
        {
            Debug.LogError("❌ QuizManager.Instance is null!");
        }

        Debug.Log("✅ LEVEL SYSTEM TEST COMPLETED!");
    }
}
