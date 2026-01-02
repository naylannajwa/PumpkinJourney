using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject levelSelectPanel;
    
    void Start()
    {
        levelSelectPanel.SetActive(false);
    }
    
    public void PlayGame()
    {
        // Load level pertama - kunci akan direset otomatis di GameManager.Start()
        GameManager.EnsureInstance();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel("gameplay");
        }
        else
        {
            SceneManager.LoadScene("gameplay");
        }
    }
    
    public void OpenLevelSelect()
    {
        levelSelectPanel.SetActive(true);
    }
    
    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}