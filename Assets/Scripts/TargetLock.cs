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

    void Update()
    {
        // if active, move the target lock sprite to the object being targeted
        if (targetLock.IsActive())
        {
            // temp vector3 to make it look prettier :)
            // TODO: find a way to put targetlock on any object's center
            Vector3 tragetPos = new Vector3(target.position.x, 
                target.position.y + 1, 
                target.position.z);   

            targetLock.gameObject.transform.position = mainCam.WorldToScreenPoint(tragetPos);
        }

        if (input.lockOnEnemy) {
            ToggleCamSwitch();
            input.lockOnEnemy = false;
        }
    }


    // Called when user toggles the target lock
    // Disables the current camera and enables the other one
    // Ex: If the freecam is active and the user turns on target lock, disable freecam and enable target cam
    void ToggleCamSwitch()
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
}
