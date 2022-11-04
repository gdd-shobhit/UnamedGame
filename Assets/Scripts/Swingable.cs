using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swingable : MonoBehaviour, IGrabbable
{
    public IEnumerator Grab(Transform t, float f)
    {
        Debug.Log("swingable detected");
        yield return null;
    }

    public bool GetSwingable() { return true; }
}
