using System;
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

    public GameObject ClosestToCenterEnemy(GameObject from)
    {
        if (nearbyEnemies.Count == 0) return null;
        else if (nearbyEnemies.Count == 1) return nearbyEnemies[0].gameObject;
        else
        {
            GameObject closest = null;
            float window = 0;
            while(closest == null)
            {
                for (int i = 0; i < nearbyEnemies.Count; i++)
                {
                    if(Mathf.Abs(EnemyDirection(from, nearbyEnemies[i].gameObject)) <= window)
                    {
                        closest = nearbyEnemies[i].gameObject;
                        //Debug.Log("found on: "+window);
                    }
                    window += 1;
                    //Debug.Log(window);
                }
            }
            return closest;
        }
    }


    public GameObject GetClosestEnemy(GameObject from)
    {
        if (nearbyEnemies.Count == 0) return null;
        else if (nearbyEnemies.Count == 1)
        {
            Debug.Log(EnemyDirection(from, nearbyEnemies[0].gameObject)); 
            return nearbyEnemies[0].gameObject;
        }
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
            Debug.Log(EnemyDirection(from, closest.gameObject));
            return closest.gameObject;
        } 
    }

    public GameObject SwitchTarget(GameObject current)
    {
        if (nearbyEnemies.Count <= 1) return null;
        else
        {
            for (int i = 0; i < nearbyEnemies.Count; i++)
            {
                
            }
            return null;
        }
    }

    // positive is to the right
    // negative is to the left
    // 0 is forward or backward
    private float EnemyDirection(GameObject from, GameObject to)
    {
        Vector3 fwd = from.transform.forward;
        Vector3 up = from.transform.up;
        Vector3 perp = Vector3.Cross(fwd, to.transform.position);
        float direction = Vector3.Dot(perp, up);

        return direction;
        
    }
}
