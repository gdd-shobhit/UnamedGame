using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class NarrativeTrigger : MonoBehaviour
{
    public NarrativeHandler narrativeHandler;

    public string node;
    public bool automatic;
    public bool repeatable;
    public bool triggerComplete;

    // Checks if the Player is inside the Narrative Trigger
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !triggerComplete)
        {
            narrativeHandler.inTrigger = true;
            narrativeHandler.currentTrigger = this;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && !triggerComplete)
        {
            narrativeHandler.inTrigger = false;
            narrativeHandler.currentTrigger = null;
        }
    }
}
