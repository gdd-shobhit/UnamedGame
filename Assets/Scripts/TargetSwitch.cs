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


    public GameObject GetClosestEnemy(GameObject from)
    {
        if (nearbyEnemies.Count == 0) return null;
        else if (nearbyEnemies.Count == 1) return nearbyEnemies[0].gameObject;
        else
        {
            float near = 0;
            Collider closest = null;
            for (int i = 0; i < nearbyEnemies.Count; i++)
            {
                float distance = Vector3.Distance(nearbyEnemies[i].transform.position, from.transform.position);
                if (near == 0) { near = distance; closest = nearbyEnemies[i]; }
                else if (distance < near) { near = distance; closest = nearbyEnemies[i]; }
            }
            return closest.gameObject;
        } 
    }
}
