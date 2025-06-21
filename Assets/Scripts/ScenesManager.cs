using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] GameObject optionMenu;
    [SerializeField] GameObject creditMenu;

    public bool isMenu;

    public void Exit()
    {
        Application.Quit();
    }

    public void StartScene()
    {
        SceneManager.LoadScene("Level_One");
    }

    public void Options()
    {
        optionMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        optionMenu.SetActive(false);
        creditMenu.SetActive(false);
    }

    public void Credits()
    {
        creditMenu.SetActive(true);
        optionMenu.SetActive(false);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
