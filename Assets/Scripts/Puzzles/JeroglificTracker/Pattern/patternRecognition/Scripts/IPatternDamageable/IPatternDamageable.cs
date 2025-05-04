using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPatternDamageable
{
    public PatternSO GetPattern();
    public void Execute();
}
