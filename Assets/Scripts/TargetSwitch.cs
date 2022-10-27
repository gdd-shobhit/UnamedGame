using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TargetSwitch : MonoBehaviour
{
    [SerializeField] List<Collider> nearbyTargets;
    [SerializeField] private CinemachineVirtualCamera followCam;

    [SerializeField] private Collider leftCollider;
    [SerializeField] private Collider rightCollider;
    [SerializeField] private Collider centerCollider;
    [SerializeField] private Collider upCollider;
    [SerializeField] private Collider downCollider;

    [SerializeField] private Collider currentTarget;


    void Start()
    {
        nearbyTargets = new List<Collider>();
    }


    void Update()
    {
        foreach (Collider c in nearbyTargets)
        {
            Vector3 to = c.transform.position;
            Vector3 from = this.gameObject.transform.position;
            Debug.DrawRay(from, to-from, Color.cyan);
        }
        SetDirectedColliders();
    }

    void OnTriggerEnter(Collider c)
    {
        //Debug.Log("hello " + c.name);
        if (c.name.ToLower().Contains("target"))
        {
            if (!nearbyTargets.Contains(c)) { 
                nearbyTargets.Add(c);
            }
        }
    }
    private void OnTriggerExit(Collider c)
    {
        //Debug.Log("goodbye " + c.name);
        if (c.name.ToLower().Contains("target"))
        {
            if(nearbyTargets.Contains(c)) { nearbyTargets.Remove(c); }
        }
    }


    // positive is to the right
    // negative is to the left
    // 0 is forward or backward
    private void SetDirectedColliders()
    {
        leftCollider = null;
        rightCollider = null;
        float leftMost = 0;
        float rightMost = 0;
        float centerMost = 99;


        for (int i = 0; i < nearbyTargets.Count; i++)
        {
            //if (currentTarget != null && currentTarget == nearbyTargets[i]) return;

            Vector3 fwd = followCam.transform.forward;
            Vector3 up = followCam.transform.up;
            Vector3 targetDir = nearbyTargets[i].transform.position - followCam.transform.position;
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);

            Debug.Log(nearbyTargets[i].name + ": " + dir);

            if(Vector3.Dot(targetDir.normalized, fwd) > 0) {
                Debug.Log(nearbyTargets[i].name + "is in front of player (" + Vector3.Dot(targetDir.normalized, fwd) +")");
            }
            else
            {
                Debug.Log(nearbyTargets[i].name + "is in back of player (" + Vector3.Dot(targetDir.normalized, fwd) + ")");
            }



            if(Vector3.Dot(targetDir.normalized, fwd) > 0) // if in front of player
            {
                if(dir < leftMost)
                {
                    leftMost = dir;
                    leftCollider = nearbyTargets[i];
                }
                if (dir > rightMost)
                {
                    rightMost = dir;
                    rightCollider = nearbyTargets[i];
                }
                if(Mathf.Abs(dir) < centerMost)
                {
                    centerMost = Mathf.Abs(dir);
                    centerCollider = nearbyTargets[i];
                }
            }
        }
    }

    public Collider GetCenterTarget()
    {
        currentTarget = centerCollider;
        //SetDirectedColliders();
        return centerCollider;
    }

    public Collider GetLeftTarget()
    {
        currentTarget = leftCollider;
        //SetDirectedColliders();
        return leftCollider;
    }

    public Collider GetRightTarget()
    {
        currentTarget = rightCollider;
        //SetDirectedColliders();
        return rightCollider;
    }
}
