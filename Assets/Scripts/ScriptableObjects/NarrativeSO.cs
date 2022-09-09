using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName ="Tools/NarrativeSO")]
public class NarrativeSO : ScriptableObject
{
    public Dictionary<string,List<string>> keyValuePairs = new Dictionary<string,List<string>>();
}
