using System.Collections;
using TMPro;
using UnityEngine;

public class Jeroglific : MonoBehaviour, IRead
{
    public TextMeshProUGUI subtitle;
    private GameObject panel;
    private BoxCollider coll;
    public string text;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        panel = subtitle.transform.parent.gameObject;
    }

    public void Aiming()
    {
        UIManager.Instance.ChangeCursor(true);
    }

    public void Read()
    {
        StartCoroutine(SetSubtitle());
        UIManager.Instance.ChangeCursor(false);
    }
    IEnumerator SetSubtitle()
    {
        panel.SetActive(true);
        subtitle.text = text;
        coll.enabled = false;

        yield return new WaitForSeconds(2f);

        panel.SetActive(false);
        coll.enabled = true;
    }
}