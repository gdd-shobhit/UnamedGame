using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    IEnumerator Grab(Transform t_player, float pullSpeed);
    bool GetSwingable(); //returns true or false for if the player can use the grabbable object to swing
}
