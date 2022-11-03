using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    // a value a float will (hopefully) NEVER be set to...
    // doing this bc u cant null a float lmao
    const float FNULL = 0;

    float closestLeft, closestRight, closestCenter, closestUp, closestDown;
    float dir, heightDif;
    Vector3 targetDir, fwd, up, perp;

    [SerializeField] List<Collider> leftTargets;
    [SerializeField] List<Collider> rightTargets;
    [SerializeField] List<Collider> upTargets;
    [SerializeField] List<Collider> downTargets;

    void Start()
    {
        nearbyTargets = new List<Collider>();
        leftTargets = new List<Collider>();
        rightTargets = new List<Collider>();
        upTargets = new List<Collider>();
        downTargets = new List<Collider>();
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
        leftTargets.Clear(); rightTargets.Clear();
        upTargets.Clear(); downTargets.Clear();

        for (int i = 0; i < nearbyTargets.Count; i++)
        {
            ResetVariables(i);
            SetDirectionVariables(i);
            HeightDifferenceCalculation(i);

            if (i == 0)
            {
                Debug.Log("dir: " + dir + ", heightDif: "+heightDif);
            }
            // if nearbyTargets[i] is the current target, don't do calculations for it
            if (currentTarget != null && currentTarget == nearbyTargets[i]) continue;

            SortTarget(i);
        }
        PickBestColliders();

        void PickBestColliders()
        {
            if (leftTargets.Count != 0)
            {
                if (leftTargets.Count == 1) { leftCollider = leftTargets[0]; }
            }

            if (rightTargets.Count != 0)
            {
                if (rightTargets.Count == 1) { rightCollider = rightTargets[0]; }
            }

            if (upTargets.Count != 0)
            {
                if (upTargets.Count == 1) { upCollider = upTargets[0]; }
            }

            if (downTargets.Count != 0)
            {
                if (downTargets.Count == 1) { downCollider = downTargets[0]; }
            }

        }

        void SortTarget(int i)
        {
            bool left = false, right = false, up = false, down = false;
            if (dir < 0) { left = true; }
            else if (dir > 0) { right = true; }

            if (heightDif > 0) { up = true; }
            else if (heightDif < 0) { down = true; }
            //if (left) { leftTargets.Add(nearbyTargets[i]); }
            //if (right) { rightTargets.Add(nearbyTargets[i]); }

            Debug.Log(i + ", left: "+left+ ", up: " + up);

            if(left && up)
            {
                if (heightDif > Mathf.Abs(dir)) upTargets.Add(nearbyTargets[i]);
                else leftTargets.Add(nearbyTargets[i]);
            }
            else if(left && down)
            {
                if (Mathf.Abs(heightDif) > Mathf.Abs(dir)) downTargets.Add(nearbyTargets[i]);
                else leftTargets.Add(nearbyTargets[i]);
            }
            else if(right && up)
            {
                if (heightDif > dir) upTargets.Add(nearbyTargets[i]);
                else rightTargets.Add(nearbyTargets[i]);
            }
            else if(right && down)
            {
                if (Mathf.Abs(heightDif) > dir) downTargets.Add(nearbyTargets[i]);
                else rightTargets.Add(nearbyTargets[i]);
            }
        }

        void ResetVariables(int i)
        {
            leftCollider = null;
            rightCollider = null;
            //centerCollider = null;
            upCollider = null;
            downCollider = null;
            closestLeft = -99;
            closestRight = 99;
            closestCenter = 99;
            closestUp = 99;
            closestDown = -99;

            heightDif = FNULL;
        }

        void LeftRightColliderCalculations(int i)
        {
            if (dir < 0 && dir > closestLeft) // closest object on left side
            {
                //if(heightDif != FNULL && heightDif < Mathf.Abs(dir))
                closestLeft = dir;
                SetTarget("left", nearbyTargets[i]);
            }
            if (dir > 0 && dir < closestRight) // closest object on right side
            {
                //if (heightDif != FNULL && heightDif < dir)
                closestRight = dir;
                SetTarget("right", nearbyTargets[i]);
            }
            if (Mathf.Abs(dir) < closestCenter) // closest object to center
            {
                closestCenter = Mathf.Abs(dir);
                SetTarget("center", nearbyTargets[i]);
            }
        }

        void UpDownColliderCalculations(int i)
        {
            if (heightDif > 0 && heightDif < closestUp)
            {
                if (heightDif < Mathf.Abs(dir))
                {
                    closestUp = heightDif;
                    SetTarget("up", nearbyTargets[i]);
                }

            }
            else if (heightDif < 0 && heightDif > closestDown)
            {
                if (heightDif < Mathf.Abs(dir))
                {
                    closestDown = heightDif;
                    SetTarget("down", nearbyTargets[i]);
                }
            }
        }

        void HeightDifferenceCalculation(int i)
        {
            float nbY = nearbyTargets[i].transform.position.y;
            float cY = currentTarget.transform.position.y;
            heightDif = nbY - cY;
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
