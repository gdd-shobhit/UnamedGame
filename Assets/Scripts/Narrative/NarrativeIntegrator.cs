using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NarrativeIntegrator
{
    private static NarrativeHandler narrativeHandler = NarrativeHandler.Instance; // Get Ref to Narrative Handler

    [YarnCommand("ChangeCamera")]
    private static void ChangeCamera(int cameraIndex)
    {
        foreach (GameObject cam in narrativeHandler.dialogueCameras)
        {
            cam.SetActive(false);
        }

        narrativeHandler.dialogueCameras[cameraIndex].SetActive(true);
    }

    [YarnCommand("SetRelationshipValue")]
    private static void SetRelationshipValue(int value)
    {
        narrativeHandler.relationshipValue = value;
    }

    [YarnCommand("AddRelationshipValue")]
    private static void AddRelationshipValue(int value)
    {
        narrativeHandler.relationshipValue += value;
    }

    [YarnFunction("GetRelationshipValue")]
    private static int GetRelationshipValue()
    {
        return narrativeHandler.relationshipValue;
    }

    [YarnCommand("InputName")]
    private static void InputName()
    {
        narrativeHandler.NameInput.SetActive(true);
    }

    [YarnFunction("GetName")]
    private static string GetName()
    {
        return narrativeHandler.croakName;
    }
}
