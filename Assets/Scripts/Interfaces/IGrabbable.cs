using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    IEnumerator GrabbablePull(Transform t_player, float pullSpeed);
}
