using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinPlayerBehaviour : MonoBehaviour {

    public void JoinPlayer(InputAction.CallbackContext cc) {
        if (cc.started && PlayerDistribution.Instance.connectedPlayers < PlayerDistribution.Instance.maxPlayerSlots) {
            PlayerDistribution.Instance.AssignPlayer(cc.control.device);
        }
    }
}
