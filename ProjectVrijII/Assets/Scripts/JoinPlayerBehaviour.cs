using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinPlayerBehaviour : MonoBehaviour {

    public void JoinPlayer(InputAction.CallbackContext cc) {
        Debug.Log(PlayerDistribution.Instance.FindFreePlayerSlot() != -1);
        if (cc.started && PlayerDistribution.Instance.FindFreePlayerSlot() != -1) {
            PlayerDistribution.Instance.AssignPlayer(cc.control.device);
        }
    }
}
