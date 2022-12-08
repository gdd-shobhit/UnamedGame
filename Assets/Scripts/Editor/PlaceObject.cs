using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlaceObject : MonoBehaviour
{
    [MenuItem("GameObject/Place Object on Terrain")]
    static void PlaceOnTerrain()
    {
        Transform selectedObj;
        RaycastHit hit;

        selectedObj = Selection.activeTransform;

        if (Physics.Raycast(selectedObj.position, Vector3.down, out hit, 100))
        {
            if (hit.collider.GetType() == typeof(TerrainCollider))
            {
                selectedObj.position = hit.point;
            }
        }
    }
}
