using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;
        public GameObject lockIcon;
        public string sceneName;
        public int levelNumber;
    }
    
    public LevelButton[] levels;
    
    void Start()
    {
        InitializeLevelButtons();
    }
    
    void InitializeLevelButtons()
    {
        // Level 1 always unlocked
        PlayerPrefs.SetInt("Level1Unlocked", 1);
        
        foreach (LevelButton level in levels)
        {
            bool isUnlocked = PlayerPrefs.GetInt("Level" + level.levelNumber + "Unlocked", 0) == 1;
            
            level.button.interactable = isUnlocked;
            level.lockIcon.SetActive(!isUnlocked);
             
            if (isUnlocked)
            {
                string sceneName = level.sceneName;
                level.button.onClick.AddListener(() => LoadLevel(sceneName));
            }
        }
    }
    
    void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}