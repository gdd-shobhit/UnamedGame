using Cinemachine;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TargetLock : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera followCam;
    [SerializeField] private CinemachineVirtualCamera targetCam;
    [SerializeField] private Camera mainCam;

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject targetSwitcher;
    [SerializeField] private Image lockImage;
    [SerializeField] private StarterAssetsInputs input;

    [SerializeField] private Transform player;
    [SerializeField] private Transform targetCamHelper;

    void Update()
    {
        CheckCameraSwitch();
        if (lockImage.IsActive())
        {
            MoveLockOnCamera();
            MoveLockOnSprite();
        }
    }

    // if user presses the lock on toggle button, switch used camera
    private void CheckCameraSwitch()
    {
        if (input.lockOnEnemy)
        {
            input.lockOnEnemy = false;

            ToggleCamSwitch();
        }
    }

    void FindEnemy()
    {
        GameObject tempObj = targetSwitcher.GetComponent<TargetSwitch>().ClosestToCenterEnemy(this.gameObject);
        if (tempObj != null) target = tempObj;
    }

    // Called when user toggles the target lock
    // Disables the current camera and enables the other one
    // Ex: If the freecam is active and the user turns on target lock, disable freecam and enable target cam
    private void ToggleCamSwitch()
    {
        if (followCam.isActiveAndEnabled)
        {
            FindEnemy();
            if (target == null) return;
            targetCam.LookAt = target.transform;
            followCam.gameObject.SetActive(false);
            targetCam.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(true);
        }
        else
        {
            target = null;
            targetCam.LookAt = null;
            targetCam.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(false);
            followCam.gameObject.SetActive(true);
        }
    }

    // moves the lock on camera to look at the target while behind the player's head
    private void MoveLockOnCamera()
    {
        targetCamHelper.LookAt(target.transform);

        // normalized vector for the distance between the player and the target
        Vector3 btwn = (player.position - target.transform.position).normalized;

        // offsets the current position to behind the players head
        targetCamHelper.position = new Vector3(player.position.x + (btwn.x * 4), player.position.y + 2, player.position.z + (btwn.z * 4));
    }

    // move the target lock sprite to the object being targeted
    private void MoveLockOnSprite()
    {
        lockImage.gameObject.transform.position = mainCam.WorldToScreenPoint(GetEnemyMidpoint());
    }

    private Vector3 GetEnemyMidpoint()
    {
        float targetHeight = target.GetComponent<CapsuleCollider>().height;
        Vector3 targetPos = target.transform.position;
        return new Vector3(targetPos.x, targetPos.y + targetHeight/2, targetPos.z);
    }
}