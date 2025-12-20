using System.Collections;
using TMPro;
using UnityEngine;

public class Jeroglific : MonoBehaviour, IRead
{
    public TextMeshProUGUI subtitle;
    private GameObject panel;
    private BoxCollider coll;
    private Outline outline;
    public string text;
    [SerializeField] float onScreenTime;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        outline = GetComponent<Outline>();
        panel = subtitle.transform.parent.gameObject;
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void Aiming()
    {
        outline.enabled = true;
        UIManager.Instance.ChangeCursor(true);
    }

    public void DisableOutline()
    {
        outline.enabled = false;
        UIManager.Instance.ChangeCursor(false);
    }

    public void Read()
    {
        outline.enabled = false;
        StartCoroutine(SetSubtitle());
        UIManager.Instance.ChangeCursor(false);
        NewEventManager.TriggerFreeze(true);
    }

    IEnumerator SetSubtitle()
    {
        panel.SetActive(true);
        subtitle.text = text;
        coll.enabled = false;

        yield return new WaitForSeconds(onScreenTime);

        NewEventManager.TriggerFreeze(false);
        panel.SetActive(false);
        coll.enabled = true;
    }
}