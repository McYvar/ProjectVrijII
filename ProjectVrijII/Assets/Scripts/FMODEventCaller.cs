using UnityEngine;
using FMODUnity;
using UnityEngine.Rendering;

public class FMODEventCaller : MonoBehaviour
{
    private FMOD.Studio.EventInstance fmodEventInstance;

    // Function called by the animation event
    public void PlayFMODEvent(string fmodEvent)
    {
        RuntimeManager.CreateInstance(fmodEvent).start();
    }

    private void OnDestroy()
    {
        // Release the FMOD event instance when the object is destroyed
        fmodEventInstance.release();
    }
}
