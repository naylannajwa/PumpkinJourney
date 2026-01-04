using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance;
    
    [Header("UI References - Quiz Panel")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    
    [Header("Answer Buttons")]
    public Button answerButtonA;
    public Button answerButtonB;
    public Button answerButtonC;
    public Button answerButtonD;
    
    [Header("Key Progress - 4 Sprites")]
    public Image keyProgressImage;
    public Sprite keySprite25;  // 1 benar = 25%
    public Sprite keySprite50;  // 2 benar = 50%
    public Sprite keySprite75;  // 3 benar = 75%
    public Sprite keySprite100; // 3 benar = 100% FULL
    
    [Header("Quiz Data")]
    public QuizData quizData;
    
    [Header("Animation Settings")]
    public float keyAnimDuration = 0.8f;
    public float delayBeforeNextQuestion = 0.5f; // Reduced delay for auto-advance
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Key Completion Animation")]
    public Color keyGlowColor = Color.yellow; // Warna cahaya saat berhasil
    public float keyScaleFactor = 1.2f; // Faktor pembesaran kunci
    public float keyCompletionAnimDuration = 1.0f; // Durasi animasi selesai
    
    // Private variables
    private Button[] answerButtons;
    private int currentQuestionIndex = 0;
    private int correctAnswersCount = 0;
    private bool isAnswering = false;
    private Action<bool> onQuizComplete;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ QuizManager initialized!");
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate QuizManager found! Destroying...");
            Destroy(gameObject);
            return;
        }

        // Auto-load QuizData dari Resources jika belum di-assign
        if (quizData == null)
        {
            quizData = Resources.Load<QuizData>("QuizData");
            if (quizData != null)
            {
                Debug.Log("✅ QuizData auto-loaded from Resources!");
            }
            else
            {
                Debug.LogError("❌ QuizData tidak ditemukan di Resources! Buat asset QuizData di Assets/Resources/QuizData.asset");
            }
        }

        // Load questions from text file if quizData is assigned
        if (quizData != null)
        {
            quizData.LoadFromTextFile();
        }
    }

    public static void EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("QuizManager");
            go.AddComponent<QuizManager>();
            Debug.Log("✅ QuizManager instance created!");
        }
    }
    
    void Start()
    {
        // Initialize button array
        answerButtons = new Button[] 
        { 
            answerButtonA, 
            answerButtonB, 
            answerButtonC, 
            answerButtonD 
        };
        
        // Hide quiz panel
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }
        
        // Setup button listeners
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
        
        // Set key progress to 25% (default/starting state)
        if (keyProgressImage != null && keySprite25 != null)
        {
            keyProgressImage.sprite = keySprite25;
        }
        
        Debug.Log("✅ QuizManager Start complete!");
    }
    
    public void ShowQuiz(Action<bool> callback)
    {
        Debug.Log("🎯 ShowQuiz called!");

        onQuizComplete = callback;
        currentQuestionIndex = 0;
        correctAnswersCount = 0;

        // Set level based on current game level
        if (GameManager.Instance != null)
        {
            int level = GameManager.Instance.GetCurrentLevel();
            Debug.Log($"🎯 QuizManager: GameManager level is {level}");

            if (quizData != null)
            {
                Debug.Log($"📊 Before SetLevel: quizData.currentLevel = {quizData.currentLevel}");
                quizData.SetLevel(level); // Ini akan auto-call LoadFromTextFile()
                Debug.Log($"📊 After SetLevel: quizData.currentLevel = {quizData.currentLevel}");

                Debug.Log($"✅ Quiz questions loaded for level {level}. Total questions available: {quizData.questions?.Length ?? 0}");

                // Debug: Show sample questions
                if (quizData.questions != null && quizData.questions.Length > 0)
                {
                    Debug.Log($"📝 Sample question for level {level}: {quizData.questions[0].question}");
                }
            }
            else
            {
                Debug.LogError("❌ QuizData is null! Cannot load questions.");
            }
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null! Cannot determine current level.");
        }

        // Reset key progress to 25%
        if (keyProgressImage != null && keySprite25 != null)
        {
            keyProgressImage.sprite = keySprite25;
        }

        // Show quiz panel
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
        }

        // Activate answer buttons
        foreach (Button btn in answerButtons)
        {
            if (btn != null)
            {
                btn.gameObject.SetActive(true);
            }
        }

        // Pause game
        Time.timeScale = 0;

        // Show first question
        ShowQuestion();
    }
    
    void ShowQuestion()
    {
        if (quizData == null || quizData.questions == null)
        {
            Debug.LogError("❌ Quiz Data is NULL or has no questions!");
            return;
        }
        
        if (currentQuestionIndex >= quizData.questions.Length)
        {
            Debug.Log("🎉 All questions completed!");
            CompleteQuiz();
            return;
        }
        
        QuizQuestion q = quizData.questions[currentQuestionIndex];
        
        // Set question text
        if (questionText != null)
        {
            questionText.text = q.question;
        }
        
        // Set answer buttons text
        if (answerButtons[0] != null)
            answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = $"A. {q.answerA}";
        if (answerButtons[1] != null)
            answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = $"B. {q.answerB}";
        if (answerButtons[2] != null)
            answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = $"C. {q.answerC}";
        if (answerButtons[3] != null)
            answerButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = $"D. {q.answerD}";
        
        // Enable all buttons
        foreach (Button btn in answerButtons)
        {
            if (btn != null)
            {
                btn.interactable = true;
            }
        }
        
        // Reset feedback
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        
        isAnswering = false;
        
        Debug.Log($"📝 Showing question {currentQuestionIndex + 1}/{quizData.questions.Length}");
    }
    
    void OnAnswerSelected(int answerIndex)
    {
        if (isAnswering) return;
        
        isAnswering = true;
        
        QuizQuestion q = quizData.questions[currentQuestionIndex];
        
        // Disable all buttons
        foreach (Button btn in answerButtons)
        {
            if (btn != null)
            {
                btn.interactable = false;
            }
        }
        
        bool isCorrect = (answerIndex == q.correctAnswerIndex);
        
        Debug.Log($"🔍 Answer: {answerIndex}, Correct: {q.correctAnswerIndex}, Result: {(isCorrect ? "✅" : "❌")}");
        
        if (isCorrect)
        {
            // ✅ CORRECT ANSWER
            correctAnswersCount++;

            if (feedbackText != null)
            {
                feedbackText.text = q.correctFeedback;
                feedbackText.color = new Color(0.2f, 0.8f, 0.2f); // Green
            }

            // Animate key progress
            StartCoroutine(AnimateKeyProgress(correctAnswersCount));

            // Check if we have answered all questions correctly
            if (currentQuestionIndex >= quizData.questions.Length - 1)
            {
                Debug.Log("🎉 All questions answered correctly! Completing quiz...");
                StartCoroutine(CompleteQuizEarly());
            }
            else
            {
                // Auto move to next question
                StartCoroutine(MoveToNextQuestionDelayed());
            }
        }
        else
        {
            // ❌ WRONG ANSWER - Tetap di soal yang sama
            if (feedbackText != null)
            {
                feedbackText.text = q.wrongFeedback;
                feedbackText.color = new Color(0.8f, 0.2f, 0.2f); // Red
            }

            // Re-enable buttons after delay untuk mencoba lagi di soal yang sama
            StartCoroutine(RetryAfterDelay(2.5f));

            Debug.Log($"❌ Wrong answer! Staying on question {currentQuestionIndex + 1} for retry");
        }
    }
    
    /// <summary>
    /// Animate key progress dengan ganti sprite
    /// 1 correct = 50% (sprite 2)
    /// 2 correct = 75% (sprite 3)
    /// 3 correct = 100% (sprite 4 - FULL) + GLOW & SCALE ANIMATION
    /// </summary>
IEnumerator AnimateKeyProgress(int correctCount)
{
    if (keyProgressImage == null)
    {
        Debug.LogError("❌ Key Progress Image is NULL!");
        yield break;
    }

    // Determine which sprite to show
    Sprite targetSprite = null;

    switch (correctCount)
    {
        case 1:
            targetSprite = keySprite50; // 50%
            Debug.Log("🔑 Key progress: 50% (1/3 correct)");
            break;
        case 2:
            targetSprite = keySprite75; // 75%
            Debug.Log("🔑 Key progress: 75% (2/3 correct)");
            break;
        case 3:
            targetSprite = keySprite100; // 100% FULL
            Debug.Log("🔑 Key progress: 100% FULL! (3/3 correct)");
            break;
    }

    if (targetSprite == null)
    {
        Debug.LogWarning($"⚠️ No sprite assigned for correctCount: {correctCount}");
        yield break;
    }

    // ⭐ SMOOTH FADE ANIMATION (NO SCALE!)
    float elapsed = 0;
    Color startColor = keyProgressImage.color;
    startColor.a = 0.3f; // Start semi-transparent

    Color endColor = Color.white;
    endColor.a = 1f; // End fully visible

    keyProgressImage.sprite = targetSprite; // Change sprite immediately
    keyProgressImage.color = startColor;

    // Fade in smoothly
    while (elapsed < keyAnimDuration)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = elapsed / keyAnimDuration;

        // Smooth fade
        keyProgressImage.color = Color.Lerp(startColor, endColor, scaleCurve.Evaluate(t));

        yield return null;
    }

    // Ensure final state
    keyProgressImage.color = endColor;

    // ⭐ SPECIAL ANIMATION FOR 3 CORRECT ANSWERS: GLOW & SCALE
    if (correctCount == 3)
    {
        Debug.Log("✨ Starting key completion animation: Glow & Scale!");
        StartCoroutine(AnimateKeyCompletion());
    }
    else
    {
        Debug.Log("✅ Key sprite changed with smooth fade!");
    }
}

/// <summary>
/// Animasi khusus saat kuis selesai (3 benar): Cahaya & pembesaran
/// </summary>
IEnumerator AnimateKeyCompletion()
{
    if (keyProgressImage == null)
    {
        Debug.LogError("❌ Key Progress Image is NULL for completion animation!");
        yield break;
    }

    float elapsed = 0;
    Vector3 originalScale = keyProgressImage.transform.localScale;
    Color originalColor = keyProgressImage.color;

    // Target scale and color
    Vector3 targetScale = originalScale * keyScaleFactor;
    Color targetColor = keyGlowColor;

    Debug.Log($"🎨 Starting completion animation - Scale: {originalScale} -> {targetScale}, Color: {originalColor} -> {targetColor}");

    // Animate to glow and scale
    while (elapsed < keyCompletionAnimDuration / 2)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = elapsed / (keyCompletionAnimDuration / 2);

        // Smooth interpolation
        keyProgressImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleCurve.Evaluate(t));
        keyProgressImage.color = Color.Lerp(originalColor, targetColor, scaleCurve.Evaluate(t));

        yield return null;
    }

    // Ensure peak state
    keyProgressImage.transform.localScale = targetScale;
    keyProgressImage.color = targetColor;

    Debug.Log("🔥 Key at peak glow and scale!");

    // Hold for a moment
    yield return new WaitForSecondsRealtime(0.5f);

    // Animate back to normal
    elapsed = 0;
    while (elapsed < keyCompletionAnimDuration / 2)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = elapsed / (keyCompletionAnimDuration / 2);

        // Smooth interpolation back
        keyProgressImage.transform.localScale = Vector3.Lerp(targetScale, originalScale, scaleCurve.Evaluate(t));
        keyProgressImage.color = Color.Lerp(targetColor, originalColor, scaleCurve.Evaluate(t));

        yield return null;
    }

    // Ensure final normal state
    keyProgressImage.transform.localScale = originalScale;
    keyProgressImage.color = originalColor;

    Debug.Log("✅ Key completion animation finished!");
}
    
    IEnumerator MoveToNextQuestionDelayed()
    {
        yield return new WaitForSecondsRealtime(delayBeforeNextQuestion);
        
        currentQuestionIndex++;
        
        // Clear feedback
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        
        ShowQuestion();
    }
    
    IEnumerator RetryAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        
        // Re-enable buttons
        foreach (Button btn in answerButtons)
        {
            if (btn != null)
            {
                btn.interactable = true;
            }
        }
        
        // Clear feedback
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        
        isAnswering = false;
        
        Debug.Log("🔄 Retry enabled");
    }
    
    void CompleteQuiz()
    {
        bool success = (correctAnswersCount >= quizData.requiredCorrectAnswers);
        
        Debug.Log($"🏁 Quiz Complete! Score: {correctAnswersCount}/{quizData.questions.Length}");
        
        if (success)
        {
            // ⭐ CUSTOM MESSAGE - Bisa diganti sesuai keinginan
            if (questionText != null)
            {
                questionText.text = "Selamat!\nKamu berhasil menjawab semua pertanyaan!";
            }
            
            if (feedbackText != null)
            {
                feedbackText.text = "Pintu telah terbuka!";
                feedbackText.color = new Color(0.2f, 0.8f, 0.2f);
            }
            
            // Hide answer buttons
            foreach (Button btn in answerButtons)
            {
                if (btn != null)
                {
                    btn.gameObject.SetActive(false);
                }
            }
            
            StartCoroutine(CloseQuizDelayed(2.5f, true));
        }
        else
        {
            if (questionText != null)
            {
                questionText.text = "Belum Berhasil!";
            }
            
            if (feedbackText != null)
            {
                feedbackText.text = $"Kamu perlu menjawab {quizData.requiredCorrectAnswers} soal dengan benar.\nCoba lagi!";
                feedbackText.color = new Color(0.8f, 0.2f, 0.2f);
            }
            
            StartCoroutine(CloseQuizDelayed(3f, false));
        }
    }
    
    IEnumerator CompleteQuizEarly()
    {
        yield return new WaitForSecondsRealtime(1.0f); // Brief pause to show the final key animation

        // Complete the quiz successfully
        CompleteQuiz();
    }

    IEnumerator CloseQuizDelayed(float delay, bool success)
    {
        yield return new WaitForSecondsRealtime(delay);

        // Hide quiz panel
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        // Resume game
        Time.timeScale = 1;

        // Callback
        onQuizComplete?.Invoke(success);

        Debug.Log($"🚪 Quiz closed. Success: {success}");
    }
}