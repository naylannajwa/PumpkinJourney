using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    [Header("Pertanyaan")]
    [TextArea(3, 5)]
    public string question;
    
    [Header("Pilihan Jawaban")]
    public string answerA;
    public string answerB;
    public string answerC;
    public string answerD;
    
    [Header("Jawaban Benar (0=A, 1=B, 2=C, 3=D)")]
    [Range(0, 3)]
    public int correctAnswerIndex;
    
    [Header("Feedback")]
    [TextArea(2, 3)]
    public string correctFeedback = "✅ Benar! Kamu hebat!";
    
    [TextArea(2, 3)]
    public string wrongFeedback = "❌ Salah! Coba lagi.";
}