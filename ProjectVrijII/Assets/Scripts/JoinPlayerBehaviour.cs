using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinPlayerBehaviour : MonoBehaviour {

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void JoinPlayer(InputAction.CallbackContext cc) {
        if (cc.started) {
            PlayerDistribution.Instance.AssignPlayer(cc.control.device);
        }
    }
}
