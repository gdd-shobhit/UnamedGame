using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using StarterAssets;
using TMPro;

public class NarrativeHandler : MonoBehaviour, IDataPersistence
{
    public int relationshipValue;
    public string croakName;

    public GameObject dialogPrompt; // UI to tell the player they can start a dialog sequence
    public GameObject player; // Player Character
    public GameObject hud; // Hud
    public DialogueRunner dialogSystem; // Yarnspinner
    public GameObject NameInput;
    public GameObject NameWarning;

    public NarrativeTrigger currentTrigger; // The Current Trigger the Player is in

    [SerializeField] private StarterAssetsInputs input; // Inputs

    public bool inTrigger; // Can the player start a dialog sequence?
    public bool inDialog; // Is the player in a dialog sequence?

    private static NarrativeHandler instance;

    public List<GameObject> dialogueCameras = new List<GameObject>();

    public static NarrativeHandler Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // Make NarrativeHandler a Singleton
        if (instance != null)
        {
            Debug.LogError("Found more than one NarrativeHandler in the scene");
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        dialogPrompt.gameObject.SetActive(inTrigger && !inDialog); // If the player can start a dialog sequence and is not already in one show the prompt

        // Starts a Dialog Sequence if the player is in a trigger for one
        if (inTrigger && !inDialog )
        {
            if (input.interact || currentTrigger.automatic)
            {
                ActivateControls(false);

                dialogSystem.StartDialogue(currentTrigger.node);
            }
            else
            {
                input.interact = false;
            }
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
        ActivateControls(true);
        inTrigger = false;
        inDialog = false;
        input.interact = false;

        // Set Trigger to complete so it can not be reactivated
        if (currentTrigger != null)
        {
            if (!currentTrigger.repeatable)
            {
                currentTrigger.triggerComplete = true;
            }
            currentTrigger = null;
        }
    }

    public void LoadData(GameData data)
    {
        this.relationshipValue = data.relationshipValue;
        this.croakName = data.croakName;
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log(data.croakName);
        data.relationshipValue = this.relationshipValue;
        data.croakName = this.croakName;
        Debug.Log(this.croakName);
    }

    public void SetName()
    {
        TMP_InputField inputField = NameInput.GetComponent<TMP_InputField>();

        if (inputField.text.Length > 9)
        {
            NameWarning.SetActive(true);
        }
        else
        {
            NameWarning.SetActive(false);

            this.croakName = inputField.text;
            NameInput.SetActive(false);
        }
    }
}
