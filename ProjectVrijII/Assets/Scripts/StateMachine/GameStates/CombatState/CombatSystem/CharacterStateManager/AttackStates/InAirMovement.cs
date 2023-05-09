using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAirMovement : AttackState {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// In air movement handler
    /// </summary>
    [SerializeField] float airMovementSpeed;

    private float minInAirTime = 0.1f;
    private float inAirTimer;

    public override void OnEnter() {
        base.OnEnter();
        playerInput.eastFirst += InAirStrong;
        playerInput.southFirst += InAirKick;
        playerInput.westFirst += InAirPunch;
        inAirTimer = 0;
    }

    public override void OnExit() {
        base.OnExit();
        playerInput.eastFirst -= InAirStrong;
        playerInput.southFirst -= InAirKick;
        playerInput.westFirst -= InAirPunch;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (inAirTimer > minInAirTime) {
            if (GroundCheck()) stateManager.SwitchState(typeof(OnGroundMovement));
        } else {
            inAirTimer += Time.deltaTime;
        }
    }

    #region ground attacks
    private void InAirPunch() {
        if (attackDelay <= 0) {
            switch (InAirInputCompare()) {
                case AttackTypes.JUMPING:
                    currentAttack = currentCharacter.jumpingPunch;
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    currentAttack = currentCharacter.dragonpunchPunch;
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    currentAttack = currentCharacter.quaterCirclePunch;
                    break;
                case AttackTypes.HALF_CIRCLE:
                    currentAttack = currentCharacter.halfCirclePunch;
                    break;
            }
        }

        OnAttack();
    }

    private void InAirKick() {
        if (attackDelay <= 0 || lastAttack as SO_Punch) {
            switch (InAirInputCompare()) {
                case AttackTypes.JUMPING:
                    currentAttack = currentCharacter.jumpingKick;
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    currentAttack = currentCharacter.dragonpunchKick;
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    currentAttack = currentCharacter.quaterCircleKick;
                    break;
                case AttackTypes.HALF_CIRCLE:
                    currentAttack = currentCharacter.halfCircleKick;
                    break;
            }
        }

        OnAttack();
    }

    private void InAirStrong() {
        if (attackDelay <= 0 || lastAttack as SO_Punch || lastAttack as SO_Kick) {
            switch (InAirInputCompare()) {
                case AttackTypes.JUMPING:
                    currentAttack = currentCharacter.jumpingStrong;
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    currentAttack = currentCharacter.dragonpunchStrong;
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    currentAttack = currentCharacter.quaterCircleStrong;
                    break;
                case AttackTypes.HALF_CIRCLE:
                    currentAttack = currentCharacter.halfCircleStrong;
                    break;
            }
        }

        OnAttack();
    }

    // on attack with special move the input from the dpad has to be compared
    private AttackTypes InAirInputCompare() {
        foreach (InputOrder requiredOrder in inputOrders) {
            // first we seek for the right combination
            if (numpadInputOrder.Count < 3 || numpadInputOrder.Count < requiredOrder.numpadOrder.Length) continue; // go to next combination if not right lenght
            if (requiredOrder.characterFacingDirection != characterFacingDirection) continue; // or if combination is not facing the same direction

            int[] inputOrder = new int[numpadInputOrder.Count];
            int getLastValues = inputOrder.Length - requiredOrder.numpadOrder.Length;
            Debug.Log(getLastValues);
            numpadInputOrder.CopyTo(inputOrder, 0);
            bool doBreak = false;
            for (int i = 0; i < requiredOrder.numpadOrder.Length; i++) {
                if (inputOrder[i + getLastValues] != requiredOrder.numpadOrder[i]) { doBreak = true; break; }
            }

            if (doBreak) { continue; }

            // if we found the right combination
            return requiredOrder.attackType;
        }

        // otherwise we assign one
        return AttackTypes.JUMPING;
    }
    #endregion
}
