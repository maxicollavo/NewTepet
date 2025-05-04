using System.Collections;
using TMPro;
using UnityEngine;

public class Jeroglific : MonoBehaviour, IRead
{
    public TextMeshProUGUI subtitle;
    public string text;

    [SerializeField] Outline outline;

    [SerializeField] bool StartsLevel;
    [SerializeField] GameObject door;
    [SerializeField] GameObject subText;
    [SerializeField] GameObject subTrigger;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        DisableOutline();
    }

    public void Aiming()
    {
        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }

    public void Read()
    {
        if (StartsLevel)
        {
            StartCoroutine(StartLevelCoroutine());
            UIManager.Instance.ChangeCursor(false);

            return;
        }

        UIManager.Instance.ChangeCursor(false);
        StartCoroutine(SetSubtitle());
    }

    IEnumerator StartLevelCoroutine()
    {
        StartCoroutine(SetSubtitle());
        Destroy(subTrigger);
        subText.SetActive(false);
        yield return new WaitForSeconds(3f);
        door.SetActive(false);
        gameObject.SetActive(false);
    }

    IEnumerator SetSubtitle()
    {
        GameManager.Instance.clickBlock = true;

        subtitle.gameObject.SetActive(true);
        subtitle.text = text;

        yield return new WaitForSeconds(3f);

        GameManager.Instance.clickBlock = false;

        subtitle.gameObject.SetActive(false);
    }

    void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }
}