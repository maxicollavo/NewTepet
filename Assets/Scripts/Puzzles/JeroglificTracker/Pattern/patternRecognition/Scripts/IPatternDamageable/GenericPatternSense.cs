using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericPatternSense : MonoBehaviour, IPatternDamageable
{
    public UnityEvent OnPatternMachExecute;


    public PatternSO flagPattern;


    public void Execute()
    {
        OnPatternMachExecute.Invoke();
    }

    public PatternSO GetPattern()
    {
        return flagPattern;
    }
}
