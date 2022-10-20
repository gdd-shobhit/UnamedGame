using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NarrativeLineViewer : DialogueViewBase
{

    // Grab Input System
    [SerializeField] private StarterAssetsInputs input;

    // The text view to display the line of dialogue in.
    [SerializeField] TMPro.TextMeshProUGUI text;

    // The game object that should animate in and out.
    [SerializeField] RectTransform container;

    // Stores a reference to the method to call when the user wants to advance
    // the line.
    Action advanceHandler = null;

    private void Start()
    {
        ShowDialog(false);
    }

    // RunLine receives a localized line, and is in charge of displaying it to
    // the user. When the view is done with the line, it should call
    // onDialogueLineFinished.
    //
    // Unless the line gets interrupted, the Dialogue Runner will wait until all
    // views have called their onDialogueLineFinished, before telling them to
    // dismiss the line and proceeding on to the next one. This means that if
    // you want to keep a line on screen for a while, simply don't call
    // onDialogueLineFinished until you're ready.
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        ShowDialog(true);
        text.text = dialogueLine.Text.Text;
        advanceHandler = requestInterrupt;
    }

    // DismissLine is called when the dialogue runner has instructed us to get
    // rid of the line. This is our view's opportunity to do whatever animations
    // we need to to get rid of the line. When we're done, we call
    // onDismissalComplete. When all line views have called their
    // onDismissalComplete, the dialogue runner moves on to the next line.
    public override void DismissLine(Action onDismissalComplete)
    {
        ShowDialog(false);
        onDismissalComplete();
    }

    // UserRequestedViewAdvancement is called by other parts of your game to
    // indicate that the user wants to proceed to the 'next' step of seeing the
    // line. What 'next' means is up to your view - in this view, it means to
    // either skip the current animation, or if no animation is happening,
    // interrupt the line.
    public override void UserRequestedViewAdvancement()
    {
        if (container.gameObject.activeSelf)
        {
            advanceHandler?.Invoke();
        }
    }

    public void ShowDialog(bool show)
    {
        container.gameObject.SetActive(show);
    }

    private void Update()
    {
        if (input.interact)
        {
            Debug.Log("test");
            UserRequestedViewAdvancement();
            input.interact = false;
        }
    }
}
