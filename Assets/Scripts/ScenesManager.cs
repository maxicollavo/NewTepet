using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] GameObject creditMenuUI;
    [SerializeField] GameObject AreYouSureUI;
    [SerializeField] GameObject PauseMenuUI;
    [SerializeField] GameObject OptionMainMenu;
    [SerializeField] AudioSource ClickAudio;

    public bool isMenu;

    private void Start()
    {
        if (isMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartScene()
    {
        SceneManager.LoadScene("Level_One");
    }


    public void BackToMenu()
    {
        creditMenuUI.SetActive(false);
        OptionMainMenu.SetActive(false);
    }

    public void Credits()
    {
        creditMenuUI.SetActive(true);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void AreYouSureMenu()
    {
        AreYouSureUI.SetActive(true);
    }
    public void DisableAreYouSure()
    {
        AreYouSureUI.SetActive(false);
    }
    public void UnPause()
    {
        PauseMenuUI.SetActive(false);
    }

    public void OptionMenu()
    {
        OptionMainMenu.SetActive(true);
    }

    public void Sound()
    {
        ClickAudio.Play();
    }
}
