using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Singleton")]
    public GameObject idleCH;
    public GameObject readCH;

    public static UIManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ChangeCursor(bool OnRead)
    {
        if (OnRead)
        {
            readCH.SetActive(true);
            idleCH.SetActive(false);
        }
        else
        {
            idleCH.SetActive(true);
            readCH.SetActive(false);
        }
    }
}
