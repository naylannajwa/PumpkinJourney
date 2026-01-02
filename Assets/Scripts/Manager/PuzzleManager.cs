using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    
    [Header("UI References")]
    public GameObject puzzlePanel;
    public TextMeshProUGUI questionText;
    public TMP_InputField answerInput;
    public Button submitButton;
    public Button closeButton;
    
    private string currentCorrectAnswer;
    private Action<bool> onPuzzleComplete;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        puzzlePanel.SetActive(false);
        submitButton.onClick.AddListener(CheckAnswer);
        closeButton.onClick.AddListener(ClosePuzzle);
    }
    
    public void ShowPuzzle(string question, string correctAnswer, Action<bool> callback)
    {
        puzzlePanel.SetActive(true);
        questionText.text = question;
        currentCorrectAnswer = correctAnswer;
        onPuzzleComplete = callback;
        answerInput.text = "";
        
        Time.timeScale = 0;
    }
    
    void CheckAnswer()
    {
        bool isCorrect = answerInput.text.Trim().ToLower() == currentCorrectAnswer.ToLower();
        ClosePuzzle();
        onPuzzleComplete?.Invoke(isCorrect);
    }
    
    void ClosePuzzle()
    {
        puzzlePanel.SetActive(false);
        Time.timeScale = 1;
    }
}