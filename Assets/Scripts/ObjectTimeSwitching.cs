using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimeSwitching : MonoBehaviour, ISwitcheable
{
    [SerializeField] GameObject pastObj;
    [SerializeField] GameObject presentObj;

    [SerializeField] List<Outline> outlines;

    private WaitForSeconds wfs = new WaitForSeconds(1.3f);

    private void Awake()
    {
        DisableOutline();
    }

    public void Aiming()
    {
        EnableOutline();
    }

    public void Switch()
    {
        StartCoroutine(TurnObjects());
    }

    private IEnumerator TurnObjects()
    {
        GameObject activeObj = presentObj.activeSelf ? presentObj : pastObj;
        GameObject inactiveObj = presentObj.activeSelf ? pastObj : presentObj;

        DisolveController activeDC = activeObj.GetComponent<DisolveController>();
        DisolveController inactiveDC = inactiveObj.GetComponent<DisolveController>();

        if (activeDC != null && inactiveDC != null)
        {
            inactiveObj.SetActive(true); // Mostrar antes de restaurar
            StartCoroutine(activeDC.DissolveAndRestore(inactiveDC));
        }

        yield return wfs;

        activeObj.SetActive(false);
    }

    void EnableOutline()
    {
        foreach (var o in outlines)
        {
            o.enabled = true;
        }
    }

    public void DisableOutline()
    {
        foreach (var o in outlines)
        {
            o.enabled = false;
        }
    }
}
