using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour //IPointerEnterHandler, IPointerExitHandler

{
    [SerializeField] GameObject creditMenuUI;
    [SerializeField] GameObject AreYouSureUI;
    [SerializeField] GameObject PauseMenuUI;
    [SerializeField] GameObject OptionMainMenu;
    [SerializeField] AudioSource ClickAudio;
    public AudioSource brickAudio;
    public BGSound bgSound;

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
        bgSound.PlayBG();
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
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    brickAudio.Play();
    //    Debug.Log("Mouse ha entrado al botón!");
    //}
    //
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    Debug.Log("Mouse ha salido del botón.");
    //}

    public void OnPointerMouseEnter()
    {
        brickAudio.Play();

    }
}
