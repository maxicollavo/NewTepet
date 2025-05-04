using PDollarGestureRecognizer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class AtackWithRecognize : MonoBehaviour
{
    public Vector2 CenterOfAttack;
    public float RadiusOfAttack;


    public void SetCenterOfAttack(Vector2 cOA)
    {
        CenterOfAttack = cOA;
    }

    public void CaptureObjetivesInRadius(Result inPattern)
    {
        Collider[] hitColliders = Physics.OverlapSphere(CenterOfAttack, RadiusOfAttack);
        IPatternDamageable[] damageables = hitColliders.Select(obj => obj.GetComponent<IPatternDamageable>()) // Intenta obtener el componente
            .Where(component => component != null).ToArray(); // Filtra los nulos 
        Attack(damageables,inPattern.GestureClass);

    }
    public void Attack(IPatternDamageable[] damageables,string inPattern)
    {
        foreach (var damageable in damageables)
        {
            if(damageable.GetPattern().paternName == inPattern)
            {
                damageable.Execute();
            }
        }
    }


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CenterOfAttack, RadiusOfAttack);
    }



}
