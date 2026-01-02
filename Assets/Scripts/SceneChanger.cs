using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void GoToGameplay()
    {
        Debug.Log("Tombol berhasil diklik");
        SceneManager.LoadScene("gameplay");
    }

}
