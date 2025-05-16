using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] GameObject optionMenu;
    [SerializeField] GameObject creditMenu;
    public AudioSource audioClip;

    public bool isMenu;
    private void Update()
    {
        changeScene();
    }
    private void Start()
    {
        if (isMenu)
            audioClip.Play();
    }

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

    public void changeScene()
    {
        if (Input.GetKey(KeyCode.F1)) 
        {
            SceneManager.LoadScene("Level_One");
        }

        if (Input.GetKey(KeyCode.F2))
        {
            SceneManager.LoadScene("Bau");
        }
    }
}
