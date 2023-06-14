using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;

public class FModEventCaller : MonoBehaviour
{
    private FMOD.Studio.EventInstance fmodEventInstance;

    // Function called by the animation event
    public void PlayFMODEvent(string fmodEvent)
    {
        // Load the FMOD event and then play
        RuntimeManager.CreateInstance(fmodEvent).start();
    }

    private void OnDestroy()
    {
        // Release the FMOD event instance when the object is destroyed
        fmodEventInstance.release();
    }
}