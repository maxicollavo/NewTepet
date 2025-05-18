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
    }
    private void Start()
    {
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartScene()
    {
        SceneManager.LoadScene("Level_One");
            audioClip.Play();
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

    
}
