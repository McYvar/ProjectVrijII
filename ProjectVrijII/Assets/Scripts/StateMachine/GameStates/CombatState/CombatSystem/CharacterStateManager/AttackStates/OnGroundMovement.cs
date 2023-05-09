using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundMovement : AttackState {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// On ground movement handler
    /// </summary>
    [SerializeField] float groundMovementSpeed;
    [SerializeField] float jumpForce;

    public override void OnEnter() {
        base.OnEnter();
        playerInput.eastFirst += OnGroundStrong;
        playerInput.southFirst += OnGroundKick;
        playerInput.westFirst += OnGroundPunch;
    }

    public override void OnExit() {
        base.OnExit();
        playerInput.eastFirst -= OnGroundStrong;
        playerInput.southFirst -= OnGroundKick;
        playerInput.westFirst -= OnGroundPunch;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        if (!GroundCheck()) stateManager.SwitchState(typeof(InAirMovement));

        if (lastInputDirection == LeftInputDirection.top) Jump();
    }

    public override void OnFixedUpdate() {
        base.OnFixedUpdate();
        GroundMovement();
    }

    public void GroundMovement() {
        rb.AddForce(playerInput.leftDirection * groundMovementSpeed);
    }

    public void Jump() {
        float nearVectorLenght = rb.velocity.magnitude *
            Mathf.Cos(Vector3.Angle(-transform.up, rb.velocity) * Mathf.Deg2Rad);
        rb.velocity += transform.up * nearVectorLenght;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        stateManager.SwitchState(typeof(InAirMovement));
    }

    #region ground attacks
    private void OnGroundPunch() {
        if (attackDelay <= 0) {
            switch (GroundInputCompare()) {
                case AttackTypes.STANDING:
                    currentAttack = currentCharacter.standingPunch;
                    break;
                case AttackTypes.CROUCHING:
                    currentAttack = currentCharacter.crouchingPunch;
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

    private void OnGroundKick() {
        if (attackDelay <= 0 || lastAttack as SO_Punch) {
            switch (GroundInputCompare()) {
                case AttackTypes.STANDING:
                    currentAttack = currentCharacter.standingKick;
                    break;
                case AttackTypes.CROUCHING:
                    currentAttack = currentCharacter.crouchingKick;
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

    private void OnGroundStrong() {
        if (attackDelay <= 0 || lastAttack as SO_Punch || lastAttack as SO_Kick) {
            switch (GroundInputCompare()) {
                case AttackTypes.STANDING:
                    currentAttack = currentCharacter.standingStrong;
                    break;
                case AttackTypes.CROUCHING:
                    currentAttack = currentCharacter.crouchingStrong;
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
    private AttackTypes GroundInputCompare() {
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
        if (lastInputDirection == LeftInputDirection.bottomLeft ||
            lastInputDirection == LeftInputDirection.bottom ||
            lastInputDirection == LeftInputDirection.bottomRight)
            return AttackTypes.CROUCHING;

        return AttackTypes.STANDING;
    }
    #endregion
}
