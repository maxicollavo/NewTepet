using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;


[CreateAssetMenu(fileName = "PatternSO", menuName = "PatternSO")]
public class PatternSO : ScriptableObject
{
    public List<Point> pattern;
    public string paternName;
    public int StrokeAmmount;
    public int ID;

}
