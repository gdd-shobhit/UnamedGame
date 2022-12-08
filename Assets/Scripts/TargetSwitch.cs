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

    const float HEIGHT_WEIGHT = 1.2f;

    float closestLeft, closestRight, closestCenter, closestUp, closestDown;
    float closestLeftH;
    float dir, heightDif;
    Vector3 targetDir, fwd, up, perp;

    [SerializeField] List<Collider> leftTargets;
    [SerializeField] List<Collider> rightTargets;
    [SerializeField] List<Collider> upTargets;
    [SerializeField] List<Collider> downTargets;
    private List<float> dirList;
    private List<float> heightList;

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
        CheckAlive();

        foreach (Collider c in nearbyTargets)
        {
            Vector3 to = c.transform.position;
            Vector3 from = this.gameObject.transform.position;
            Debug.DrawRay(from, to-from, Color.cyan);
        }
        
        ClearColliders();
        PickBestCollider("center", nearbyTargets, "x");


        if (currentTarget != null) SetDirectedColliders();

        void ClearColliders()
        {
            leftTargets.Clear(); rightTargets.Clear();
            upTargets.Clear(); downTargets.Clear();
            leftCollider = null;
            rightCollider = null;
            centerCollider = null;
            upCollider = null;
            downCollider = null;
        }
        void FindCenterTarget()
        {
            /*if (Mathf.Abs(dir) < closestCenter) // closest object to center
        {
            closestCenter = Mathf.Abs(dir);
            SetTarget("center", nearbyTargets[i]);
        }*/
        }
    }

    private void CheckAlive()
    {
        foreach (Collider c in nearbyTargets)
        {
            if(c.transform.parent.GetComponent<Enemy>().health <= 0)
            {
                nearbyTargets.Remove(c);
            }
        }
        
    }

    void OnTriggerEnter(Collider c)
    {
        //Debug.Log("hello " + c.name);
        if (c.name.ToLower().Contains("target"))
        {
            if (nearbyTargets.Contains(c)) return;
            nearbyTargets.Add(c);
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


    private void SetDirectedColliders()
    {
        // Go through each nearby target and sort them into seperate lists
        for (int i = 0; i < nearbyTargets.Count; i++)
        {
            // if nearbyTargets[i] is the current target, don't do calculations for it
            if (currentTarget != null && currentTarget == nearbyTargets[i]) continue;

            SetDirectionVariables(nearbyTargets[i]);
            HeightDifferenceCalculation(nearbyTargets[i]);

            SortTarget(i);
        }

        // Now that all colliders are sorted, pick the best collider for each direction
        PickBestCollider("left",leftTargets,"x");
        PickBestCollider("right",rightTargets,"x");
        PickBestCollider("up",upTargets,"y");
        PickBestCollider("down",downTargets,"y");


        
  
        void SortTarget(int i)
        {
            bool left = false, right = false, up = false, down = false;
            // positive is to the right
            // negative is to the left
            if (dir < 0) { left = true; }
            else if (dir > 0) { right = true; }

            if (heightDif > 0) { up = true; }
            else if (heightDif < 0) { down = true; }

            //Debug.Log(i + ", left: "+left+ ", right: " + right + ", up: " + up + ", down: " + down);
            //if (nearbyTargets[i].name.Contains("4")) Debug.Log(nearbyTargets[i].name + ", heightDif: " + heightDif + ", dir: " + dir);
            if(left && up)
            {
                if (heightDif* HEIGHT_WEIGHT > Mathf.Abs(dir)) upTargets.Add(nearbyTargets[i]);
                else leftTargets.Add(nearbyTargets[i]);
            }
            else if(left && down)
            {
                if (Mathf.Abs(heightDif) * HEIGHT_WEIGHT > Mathf.Abs(dir)) downTargets.Add(nearbyTargets[i]);
                else leftTargets.Add(nearbyTargets[i]);
            }
            else if(right && up)
            {
                if (heightDif * HEIGHT_WEIGHT > dir) upTargets.Add(nearbyTargets[i]);
                else rightTargets.Add(nearbyTargets[i]);
            }
            else if(right && down)
            {
                if (Mathf.Abs(heightDif)* HEIGHT_WEIGHT > dir) downTargets.Add(nearbyTargets[i]);
                else rightTargets.Add(nearbyTargets[i]);
            }
        }

        
    }

    private void PickBestCollider(string tName, List<Collider> list, string pref)
    {
        float closestDir = 999;
        float closestHeight = 999;
        Collider closestDirC = null;
        Collider closestHeightC = null;

        if (list.Count != 0)
        {
            if (list.Count == 1) { SetTarget(tName, list[0]); }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    SetDirectionVariables(list[i]);
                    if(currentTarget!=null) HeightDifferenceCalculation(list[i]);

                    //if (tName == "left") Debug.Log(list[i].name + ", heightDif:" + heightDif + ", dir:" + dir);

                    if (Mathf.Abs(dir) < closestDir)
                    {
                        closestDir = MathF.Abs(dir);
                        closestDirC = list[i];
                    }
                    if (Mathf.Abs(heightDif) < closestHeight)
                    {
                        closestHeight = Mathf.Abs(heightDif);
                        closestHeightC = list[i];
                    }
                }
                switch (pref)
                {
                    case "x":
                        SetTarget(tName, closestDirC);
                        break;
                    case "y":
                        SetTarget(tName, closestHeightC);

                        break;
                }
            }
        }
    }

    private void HeightDifferenceCalculation(Collider i)
    {
        float nbY = i.transform.position.y;
        float cY = currentTarget.transform.position.y;
        heightDif = nbY - cY;
        //if(i.name.Contains("4")) Debug.Log(i.name + ", nbY: " + nbY + ", cY: " + cY+"... hd: "+heightDif);

    }

    private bool InFrontOfPlayer()
    {
        if (Vector3.Dot(targetDir.normalized, fwd) > 0.7f) return true;
        else return false;
    }

    private void SetDirectionVariables(Collider i)
    {
        targetDir = i.transform.position - followCam.transform.position; // direction from nearbyTargets[i] and the player follow camera
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
