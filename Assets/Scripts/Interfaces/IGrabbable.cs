using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    void GrabbablePull(Vector3 direction, float force);
}
