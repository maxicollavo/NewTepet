using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyPatternController : MonoBehaviour ,IPatternDamageable
{
    public PatternSO pattern;

   public Animator animator;


    public PatternSO GetPattern()
    {
        return pattern;
    }
    public void Execute()
    {
        animator.SetTrigger("Damage");
    }


}
