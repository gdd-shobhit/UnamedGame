using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TargetSwitch : MonoBehaviour
{
    [SerializeField] List<Collider> nearbyEnemies;
    void Start()
    {
        nearbyEnemies = new List<Collider>();
    }


    void Update()
    {
        float near = 0;
        Collider closest = null;
        for (int i = 0; i < nearbyEnemies.Count; i++)
        {
            float distance = Vector3.Distance(nearbyEnemies[i].transform.position, this.transform.position);
            if (near == 0) { near = distance; closest = nearbyEnemies[i]; }
            else if (distance < near) { near = distance; closest = nearbyEnemies[i]; }
        }
        Debug.Log("closest collider : " + closest.name);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.name.ToLower().Contains("enemy"))
        {
            if (!nearbyEnemies.Contains(c)) { 
                nearbyEnemies.Add(c);
            }
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (c.name.ToLower().Contains("enemy"))
        {
            if(nearbyEnemies.Contains(c)) { nearbyEnemies.Remove(c); }
        }
    }
}
