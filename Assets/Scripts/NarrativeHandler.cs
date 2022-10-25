using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using StarterAssets;

public class NarrativeHandler : MonoBehaviour
{
    public GameObject dialogPrompt; // UI to tell the player they can start a dialog sequence
    public GameObject player; // Player Character
    public GameObject hud; // Hud
    public DialogueRunner dialogSystem;

    public NarrativeTrigger currentTrigger;

    [SerializeField] private StarterAssetsInputs input; // Inputs


    public bool inTrigger; // Can the player start a dialog sequence?
    public bool inDialog; // Is the player in a dialog sequence?


    // Update is called once per frame
    void Update()
    {
        dialogPrompt.gameObject.SetActive(inTrigger && !inDialog); // If the player can start a dialog sequence and is not already in one show the prompt

        // Starts a Dialog Sequence if the player is in a trigger for one
        if (inTrigger && !inDialog && input.interact)
        {
            ActivateControls(false);

            dialogSystem.StartDialogue(currentTrigger.node);
        }
        else
        {
            input.interact = false;
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

        // Cursor Settings
        if (activate)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void DialogComplete()
    {
        Debug.Log("Dialog Done!");

        ActivateControls(true);

        currentTrigger.triggerComplete = true;
        inTrigger = false;
        inDialog = false;
        input.interact = false;
    }
}
