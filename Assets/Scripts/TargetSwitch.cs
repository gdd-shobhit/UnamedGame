using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TargetSwitch : MonoBehaviour
{
    [SerializeField] List<Collider> nearbyTargets;
    [SerializeField] private CinemachineVirtualCamera followCam;

    public Collider leftCollider;
    public Collider rightCollider;
    public Collider centerCollider;
    public Collider upCollider;
    public Collider downCollider;

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
        centerCollider = null;
        upCollider = null;
        downCollider = null;
        float closestLeft = -99;
        float closestRight = 99;
        float closestCenter = 99;
        float closestUp = 99;
        float closestDown = -99;

        float dir;
        Vector3 targetDir, fwd, up, perp;

        for (int i = 0; i < nearbyTargets.Count; i++)
        {
            // if nearbyTargets[i] is the current target, don't do calculations for it
            if (currentTarget != null && currentTarget == nearbyTargets[i]) continue;

            SetDirectionVariables(i);

            //if (!InFrontOfPlayer()) continue;


            if (dir < 0 && dir > closestLeft) // closest object on left side
            {
                closestLeft = dir;
                SetTarget("left", nearbyTargets[i]);
            }
            if (dir > 0 && dir < closestRight) // closest object on right side
            {
                closestRight = dir;
                SetTarget("right", nearbyTargets[i]);
            }
            if (Mathf.Abs(dir) < closestCenter) // closest object to center
            {
                closestCenter = Mathf.Abs(dir);
                SetTarget("center", nearbyTargets[i]);
            }
            //Debug.Log(nearbyTargets[i].name + ": " + dir);

            if (!(nearbyTargets[i].name.Contains("top")
                || nearbyTargets[i].name.Contains("middle")
                || nearbyTargets[i].name.Contains("bottom")))
            { continue; }


            //Debug.Log(nearbyTargets[i].name +": "+targetDir.y);

            if (currentTarget == null) continue;

            float nbY = nearbyTargets[i].transform.position.y;
            float cY = currentTarget.transform.position.y;
            float heightDif = nbY - cY;
            //Debug.Log(nearbyTargets[i].name +": "+ heightDif);

            if (heightDif > 0 && heightDif < closestUp)
            {
                closestUp = heightDif;
                SetTarget("up", nearbyTargets[i]);
            }
            else if (heightDif < 0 && heightDif > closestDown)
            {
                closestDown = heightDif;
                SetTarget("down", nearbyTargets[i]);
            }

            // if there is not a current target, return...
            // we don't need to do calculations for the other colliders right now
            /*
            if (!currentTarget || dir > 2) continue;

            if (nearbyTargets[i] == leftCollider || nearbyTargets[i] == rightCollider) break;

            if (nearbyTargets[i].transform.position.y > currentTarget.transform.position.y)
            {
                if (nearbyTargets[i].transform.position.y < closestUp)
                {
                    closestUp = nearbyTargets[i].transform.position.y;
                    SetTarget("up", nearbyTargets[i]);
                }
            }
            else if (nearbyTargets[i].transform.position.y < currentTarget.transform.position.y)
            {
                if (nearbyTargets[i].transform.position.y < closestDown)
                {
                    closestDown = nearbyTargets[i].transform.position.y;
                    SetTarget("down", nearbyTargets[i]);
                }
            }*/

        }
        
        

        bool InFrontOfPlayer()
        {
            if (Vector3.Dot(targetDir.normalized, fwd) > 0.7f) return true;
            else return false;
        }

        void SetDirectionVariables(int i)
        {
            targetDir = nearbyTargets[i].transform.position - followCam.transform.position; // direction from nearbyTargets[i] and the player follow camera
            fwd = followCam.transform.forward; // forward direction vector based on the player follow camera
            up = followCam.transform.up; // up direction vector based on the player follow camera
            perp = Vector3.Cross(fwd, targetDir);
            dir = Vector3.Dot(perp, up);

            if (Vector3.Dot(targetDir.normalized, fwd) > 0)
            {
                //Debug.Log(nearbyTargets[i].name + "is in front of player (" + Vector3.Dot(targetDir.normalized, fwd) + ")");
            }
            else {/*Debug.Log(nearbyTargets[i].name + "is in back of player (" + Vector3.Dot(targetDir.normalized, fwd) + ")");*/ }
        }
    }

    private void SetTarget(string target, Collider c)
    {
        switch (target)
        {
            case "center": centerCollider = c; break;
            case "left": leftCollider = c; break;
            case "right": rightCollider = c; break;
            case "up": upCollider = c; break;
            case "down": downCollider = c; break;
        }
    }

    public Collider GetTarget(string target)
    {
        Collider gotTarget = null;
        switch (target)
        {
            case "center": gotTarget = centerCollider; break;
            case "left": gotTarget = leftCollider; break;
            case "right": gotTarget = rightCollider; break;
            case "up": gotTarget = upCollider; break;
            case "down": gotTarget = downCollider; break;
        }
        if (gotTarget == null) return null;
        currentTarget = gotTarget;
        return gotTarget;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }
}
