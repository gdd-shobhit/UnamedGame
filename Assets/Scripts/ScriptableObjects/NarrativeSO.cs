using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NarrativeSO", menuName = "ScriptableObjects/NarrativeSO", order = 1)]
public class NarrativeSO : ScriptableObject
{
    public Dictionary<int,List<string>> keyValuePairs = new Dictionary<int,List<string>>();
}
