using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuizData", menuName = "Game/Quiz Data", order = 1)]
public class QuizData : ScriptableObject
{
    [Header("Data Pertanyaan")]
    public QuizQuestion[] questions;

    [Header("Settings")]
    [Tooltip("Berapa pertanyaan yang harus dijawab benar untuk lulus")]
    public int requiredCorrectAnswers = 3;

    [Header("Level Settings")]
    [Tooltip("Current level (1-4) to load questions for")]
    public int currentLevel = 1;
    [Tooltip("Number of questions per level")]
    public int questionsPerLevel = 3;

    [Header("Load from Text File")]
    [Tooltip("Path to the text file in Resources/Quiz/")]
    public string textFilePath = "Quiz/Soal_Jawaban";

    [Header("Level-Specific Files (Optional)")]
    [Tooltip("Use separate files per level")]
    public bool useLevelSpecificFiles = true;
    [Tooltip("Base filename for level files (e.g., 'Soal_Level_')")]
    public string levelFilePrefix = "Soal_Level_";

    /// <summary>
    /// Set the current level for question loading
    /// </summary>
    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 4); // Ensure level is between 1-4
        Debug.Log($"🎯 QuizData level set to {currentLevel}");

        // Auto-reload questions when level changes
        LoadFromTextFile();
    }

    /// <summary>
    /// Load questions from text file using Resources
    /// </summary>
    public void LoadFromTextFile()
    {
        string actualFilePath = textFilePath;

        // Use level-specific file if enabled
        if (useLevelSpecificFiles)
        {
            string levelFileName = $"{levelFilePrefix}{currentLevel}"; // No .txt extension for Resources
            TextAsset levelAsset = Resources.Load<TextAsset>($"Quiz/{levelFileName}");

            if (levelAsset != null)
            {
                actualFilePath = $"Quiz/{levelFileName}";
                Debug.Log($"📄 Using level-specific TextAsset: {levelFileName}");
            }
            else
            {
                Debug.Log($"⚠️ Level-specific TextAsset not found: {levelFileName}, using default");
            }
        }

        // Load the TextAsset
        string resourcesPath = actualFilePath;
        TextAsset textAsset = Resources.Load<TextAsset>(resourcesPath);

        if (textAsset == null)
        {
            Debug.LogError($"❌ TextAsset not found in Resources: {resourcesPath}");
            return;
        }

        string[] lines = textAsset.text.Split('\n'); // Split by newlines
        List<QuizQuestion> loadedQuestions = new List<QuizQuestion>();

        int i = 0;
        while (i < lines.Length)
        {
            // Skip empty lines
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                i++;
                continue;
            }

            // Check if it's a question (not starting with + or #)
            if (!lines[i].StartsWith("+") && !lines[i].StartsWith("#"))
            {
                QuizQuestion q = new QuizQuestion();
                q.question = lines[i].Trim();

                // Read options (4 lines starting with +)
                List<string> options = new List<string>();
                for (int j = 0; j < 4 && i + j + 1 < lines.Length; j++)
                {
                    string optionLine = lines[i + j + 1];
                    if (optionLine.StartsWith("+"))
                    {
                        options.Add(optionLine.Substring(1).Trim());
                    }
                    else
                    {
                        break;
                    }
                }

                if (options.Count == 4)
                {
                    q.answerA = options[0];
                    q.answerB = options[1];
                    q.answerC = options[2];
                    q.answerD = options[3];

                    // Read correct answer (next line after options)
                    int correctLineIndex = i + 5;
                    if (correctLineIndex < lines.Length)
                    {
                        string correctAnswer = lines[correctLineIndex].Trim();
                        // Check if it's a text answer (not A/B/C/D)
                        if (correctAnswer.Length > 1)
                        {
                            // Find which option matches the correct answer text
                            if (correctAnswer == q.answerA) q.correctAnswerIndex = 0;
                            else if (correctAnswer == q.answerB) q.correctAnswerIndex = 1;
                            else if (correctAnswer == q.answerC) q.correctAnswerIndex = 2;
                            else if (correctAnswer == q.answerD) q.correctAnswerIndex = 3;
                            else
                            {
                                Debug.LogWarning($"⚠️ Correct answer '{correctAnswer}' not found in options for question: {q.question}");
                                q.correctAnswerIndex = 0; // Default to A
                            }
                        }
                        else
                        {
                            // Legacy A/B/C/D format
                            string correctAnswerUpper = correctAnswer.ToUpper();
                            switch (correctAnswerUpper)
                            {
                                case "A": q.correctAnswerIndex = 0; break;
                                case "B": q.correctAnswerIndex = 1; break;
                                case "C": q.correctAnswerIndex = 2; break;
                                case "D": q.correctAnswerIndex = 3; break;
                                default:
                                    Debug.LogWarning($"⚠️ Invalid correct answer: {correctAnswer} for question: {q.question}");
                                    q.correctAnswerIndex = 0; // Default to A
                                    break;
                            }
                        }
                    }

                    // Read feedback messages (2 lines after correct answer)
                    int correctFeedbackIndex = i + 6;
                    int wrongFeedbackIndex = i + 7;

                    if (correctFeedbackIndex < lines.Length && !string.IsNullOrWhiteSpace(lines[correctFeedbackIndex]))
                    {
                        q.correctFeedback = lines[correctFeedbackIndex].Trim();
                    }

                    if (wrongFeedbackIndex < lines.Length && !string.IsNullOrWhiteSpace(lines[wrongFeedbackIndex]))
                    {
                        q.wrongFeedback = lines[wrongFeedbackIndex].Trim();
                    }

                    loadedQuestions.Add(q);
                    i += 8; // Move to next question (question + 4 options + 1 answer + 2 feedback)
                }
                else
                {
                    Debug.LogWarning($"⚠️ Incomplete options for question: {q.question}");
                    i++;
                }
            }
            else
            {
                i++;
            }
        }

        // If using level-specific files, use all loaded questions
        // If using single file, filter by level
        if (useLevelSpecificFiles)
        {
            // Use all questions from the level-specific file
            questions = loadedQuestions.ToArray();
            Debug.Log($"✅ Loaded {questions.Length} questions from level-specific file for level {currentLevel}");

            // Debug: Show first question
            if (questions.Length > 0)
            {
                Debug.Log($"📝 Level {currentLevel} first question: {questions[0].question}");
            }
        }
        else
        {
            // Legacy filtering for single file
            int startIndex = (currentLevel - 1) * questionsPerLevel;
            int endIndex = Mathf.Min(startIndex + questionsPerLevel, loadedQuestions.Count);

            Debug.Log($"🎯 Filtering for Level {currentLevel}: startIndex={startIndex}, questionsPerLevel={questionsPerLevel}, loadedQuestions.Count={loadedQuestions.Count}");

            if (startIndex >= loadedQuestions.Count)
            {
                Debug.LogWarning($"⚠️ No questions available for level {currentLevel}. Start index {startIndex} >= total questions {loadedQuestions.Count}");
                questions = new QuizQuestion[0];
            }
            else
            {
                int questionCount = endIndex - startIndex;
                List<QuizQuestion> levelQuestions = loadedQuestions.GetRange(startIndex, questionCount);
                questions = levelQuestions.ToArray();

                Debug.Log($"✅ Loaded {questions.Length} questions for level {currentLevel} (indices {startIndex}-{endIndex-1})");

                // Debug: Show first question of this level
                if (questions.Length > 0)
                {
                    Debug.Log($"📝 Level {currentLevel} first question: {questions[0].question}");
                }
            }
        }
    }
}
