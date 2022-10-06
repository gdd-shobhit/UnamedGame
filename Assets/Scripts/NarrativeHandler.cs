using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Yarn.Unity;
using TMPro;
using StarterAssets;

public class NarrativeHandler : MonoBehaviour
{
    public GameObject startDialogPrompt; // UI to tell the player they can start a dialog sequence
    public GameObject dialog; // Dialog
    public GameObject player; // Player Character
    public GameObject hud; // Hud
    [SerializeField] private StarterAssetsInputs input; // Inputs

    private bool inTrigger; // Can the player start a dialog sequence?
    private bool inDialog; // Is the player in a dialog sequence?

    private void Update()
    {
        startDialogPrompt.gameObject.SetActive(inTrigger && !inDialog); // If the player can start a dialog sequence and is not already in one show the prompt

        // Starts a Dialog Sequence if the player is in a trigger for one
        if (inTrigger && input.interact)
        {
            inDialog = true;

            // Disable Controls
            player.GetComponent<ThirdPersonController>().inDialog = true;
            player.GetComponent<FrogCharacter>().inDialog = true;

            // Show & Hide UI
            
            hud.gameObject.SetActive(false);
            dialog.gameObject.SetActive(true);

            // Unlock Cursor
            Cursor.lockState = CursorLockMode.None;
            

        }
        else
        {
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
}
