using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class NarrativeTrigger : MonoBehaviour
{
    public GameObject dialogPrompt; // UI to tell the player they can start a dialog sequence
    public GameObject player; // Player Character
    public GameObject hud; // Hud
    [SerializeField] private StarterAssetsInputs input; // Inputs

    private bool inTrigger; // Can the player start a dialog sequence?
    private bool inDialog; // Is the player in a dialog sequence?

    public UnityEvent dialogStart; // Tells Game when Dialog Starts

    private void Update()
    {
        dialogPrompt.gameObject.SetActive(inTrigger && !inDialog); // If the player can start a dialog sequence and is not already in one show the prompt

        // Starts a Dialog Sequence if the player is in a trigger for one
        if (inTrigger && !inDialog && input.interact)
        {
            ActivateControls(false);

            // Unlock Cursor
            Cursor.lockState = CursorLockMode.None;

            dialogStart.Invoke();

            input.interact = false;
        }
    }

    // Checks if the Player is inside the Narrative Trigger
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            inTrigger = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            inTrigger = false;
        }
    }

    public void ActivateControls(bool activate)
    {
        Debug.Log($"Controls set to: {activate}");

        inDialog = !activate;

        // Disable Controls
        player.GetComponent<ThirdPersonController>().inDialog = !activate;
        player.GetComponent<FrogCharacter>().inDialog = !activate;

        // Diable UI
        hud.gameObject.SetActive(activate);
    }
}
