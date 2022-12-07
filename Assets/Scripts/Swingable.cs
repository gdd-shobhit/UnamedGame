using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is pretty empty at the moment since it doesn't actually need to do much at this point beyond confirming that an object is swingable
/// </summary>
public class Swingable : MonoBehaviour, IGrabbable
{
    public IEnumerator Grab(Transform t, float f)
    {
        yield return null;
    }

    public bool GetSwingable() { return true; }
}
