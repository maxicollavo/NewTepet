using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonmanager : MonoBehaviour
{
   public GameObject button;

    public void StartScene()
    {
        SceneManager.LoadScene("Level_One");
    }
    
    public void Options()
    {
        SceneManager.LoadScene("Options");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }


}
