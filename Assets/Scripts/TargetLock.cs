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

    [SerializeField] private Transform target;
    [SerializeField] private Image targetLock;
    [SerializeField] private StarterAssetsInputs input;

    [SerializeField] private Transform player;
    [SerializeField] private Transform lockObject;

    void Update()
    {
        CheckCameraSwitch();
        if (targetLock.IsActive())
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
            ToggleCamSwitch();
            input.lockOnEnemy = false;
        }
    }

    // Called when user toggles the target lock
    // Disables the current camera and enables the other one
    // Ex: If the freecam is active and the user turns on target lock, disable freecam and enable target cam
    private void ToggleCamSwitch()
    {
        if (followCam.isActiveAndEnabled) {
            followCam.gameObject.SetActive(false);
            targetCam.gameObject.SetActive(true);
            targetLock.gameObject.SetActive(true);
        }
        else {
            targetCam.gameObject.SetActive(false);
            targetLock.gameObject.SetActive(false);
            followCam.gameObject.SetActive(true);  
        }
    }

    // moves the lock on camera to look at the target while behind the player's head
    private void MoveLockOnCamera()
    {
        lockObject.LookAt(target);

        // normalized vector for the distance between the player and the target
        Vector3 btwn = (player.position - target.position).normalized;

        // offsets the current position to behind the players head
        lockObject.position = new Vector3(player.position.x + (btwn.x * 4), player.position.y + 2, player.position.z + (btwn.z * 4));
    }

    // move the target lock sprite to the object being targeted
    private void MoveLockOnSprite()
    {
        // temp vector3 to make it look prettier :)
        // TODO: find a way to put targetlock on any object's center
        Vector3 tragetPos = new Vector3(target.position.x,
            target.position.y + 1,
            target.position.z);

        targetLock.gameObject.transform.position = mainCam.WorldToScreenPoint(tragetPos);
    }
}