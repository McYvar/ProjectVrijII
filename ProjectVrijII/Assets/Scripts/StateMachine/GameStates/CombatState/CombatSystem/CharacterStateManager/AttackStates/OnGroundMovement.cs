using UnityEngine;

public class OnGroundMovement : AttackState {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// On ground movement handler
    /// </summary>

    public override void OnEnter() {
        base.OnEnter();
        SetAttackPhase(AttackPhase.ready);
        inputHandler.eastFirst += OnGroundStrong;
        inputHandler.southFirst += OnGroundKick;
        inputHandler.westFirst += OnGroundPunch;
        inputHandler.leftShoulderLast += SetVariableSpeedToWalking;

        animator.SetBool("isGrounded", true);
    }

    public override void OnExit() {
        base.OnExit();
        inputHandler.eastFirst -= OnGroundStrong;
        inputHandler.southFirst -= OnGroundKick;
        inputHandler.westFirst -= OnGroundPunch;
        inputHandler.leftShoulderLast -= SetVariableSpeedToWalking;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (!isGrounded) stateManager.SwitchState(typeof(InAirMovement));
    }

    protected override void CanJump() {
        base.CanJump();

        if (character.attackPhase == AttackPhase.ready) {
            DoJump(character.groundJumpStrength);
        } else {
            if (character.lastAttack.canceledByJump) {
                RecoveryInputBuffer = () => {
                    DoJump(character.groundJumpStrength);
                };
            } else {
                ReadyInputBuffer = () => {
                    DoJump(character.groundJumpStrength);
                };
            }
        }
    }
    protected override void Movement() {
        if (!character.rbInput) return;
        if (character.lastInputDirection == LeftInputDirection.bottomLeft || // crouching movement
            character.lastInputDirection == LeftInputDirection.bottom ||
            character.lastInputDirection == LeftInputDirection.bottomRight) {

            animator.SetInteger("Stance", 1);

            Vector2 horizonal = new Vector2(inputHandler.leftDirection.x, 0);
            rb.velocity = new Vector2(horizonal.normalized.x * character.crouchMovementSpeed *
                character.attackMovementReductionScalar, rb.velocity.y);

            if (rb.velocity.x == 0) animator.SetInteger("Move", 0);

        } else { // walking/running movement

            animator.SetInteger("Stance", 0);

            if (character.lastInputDirection == LeftInputDirection.centre) character.variableMovementSpeed = 0;
            else {
                OnDoublePress -= SetVariableSpeedToRunning;
                OnDoublePress += SetVariableSpeedToRunning;
            }

            if (inputHandler.leftShoulder) SetVariableSpeedToRunning();

            Vector2 horizonal = new Vector2(inputHandler.leftDirection.x, 0);
            float resultMovementSpeed =
                (character.groundMovementSpeed + character.variableMovementSpeed) *
                character.attackMovementReductionScalar;
            rb.velocity = new Vector2(
                horizonal.normalized.x * resultMovementSpeed,
                rb.velocity.y);

            if (horizonal.x == 0) animator.SetInteger("Move", 0);
            else if (character.variableMovementSpeed == 0) { // means walking
                if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
                    if (horizonal.x > 0) animator.SetInteger("Move", 1); // walking foward facing right
                    if (horizonal.x < 0) animator.SetInteger("Move", -1); // walking backward facing right
                } else {
                    if (horizonal.x > 0) animator.SetInteger("Move", -1); // walking backward facing left
                    if (horizonal.x < 0) animator.SetInteger("Move", 1); // walking forward facing left
                }
            } else { // means running
                if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
                    if (horizonal.x > 0) animator.SetInteger("Move", 2); // running foward facing right
                    if (horizonal.x < 0) animator.SetInteger("Move", -2); // running backward facing right
                } else {
                    if (horizonal.x > 0) animator.SetInteger("Move", -2); // running backward facing left
                    if (horizonal.x < 0) animator.SetInteger("Move", 2); // running forward facing left
                }
            }
        }
    }

    private void SetVariableSpeedToRunning() {
        character.variableMovementSpeed = character.runningMovementSpeed;
    }

    private void SetVariableSpeedToWalking()
    {
        character.variableMovementSpeed = 0;
    }

    #region ground attacks
    private void OnGroundPunch() {
        switch (InputCompare()) {
            case AttackTypes.STANDING:
                character.currentAttack = character.standingPunch;
                character.currentAttackName = "5P";
                break;
            case AttackTypes.CROUCHING:
                character.currentAttack = character.crouchingPunch;
                character.currentAttackName = "2P";
                break;
            case AttackTypes.DRAGON_PUNCH:
                character.currentAttack = character.dragonpunchPunch;
                character.currentAttackName = "623P";
                break;
            case AttackTypes.QUARTER_CIRCLE:
                character.currentAttack = character.quaterCirclePunch;
                character.currentAttackName = "236P";
                break;
            case AttackTypes.QUARTER_CIRCLE_BACKWARD:
                character.currentAttack = character.quaterBackwardCirclePunch;
                character.currentAttackName = "41236P";
                break;
        }

        OnAttack();
    }

    private void OnGroundKick() {
        if (inputHandler.leftTrigger > 0.7f)
        {
            character.currentAttack = character.quaterBackwardCircleKick;
            character.currentAttackName = "41236K";
        }
        else
        {
            switch (InputCompare())
            {
                case AttackTypes.STANDING:
                    character.currentAttack = character.standingKick;
                    character.currentAttackName = "5K";
                    break;
                case AttackTypes.CROUCHING:
                    character.currentAttack = character.crouchingKick;
                    character.currentAttackName = "2K";
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    character.currentAttack = character.dragonpunchKick;
                    character.currentAttackName = "623K";
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    character.currentAttack = character.quaterCircleKick;
                    character.currentAttackName = "236K";
                    break;
                case AttackTypes.QUARTER_CIRCLE_BACKWARD:
                    character.currentAttack = character.quaterBackwardCircleKick;
                    character.currentAttackName = "41236K";
                    break;
            }
        }

        OnAttack();
    }

    private void OnGroundStrong()
    {
        if (inputHandler.leftTrigger > 0.7f)
        {
            character.currentAttack = character.dragonpunchKick;
            character.currentAttackName = "623K";
        }
        else
        {
            switch (InputCompare())
            {
                case AttackTypes.STANDING:
                    character.currentAttack = character.standingStrong;
                    character.currentAttackName = "5S";
                    break;
                case AttackTypes.CROUCHING:
                    character.currentAttack = character.crouchingStrong;
                    character.currentAttackName = "2S";
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    character.currentAttack = character.dragonpunchStrong;
                    character.currentAttackName = "623S";
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    character.currentAttack = character.quaterCircleStrong;
                    character.currentAttackName = "236S";
                    break;
                case AttackTypes.QUARTER_CIRCLE_BACKWARD:
                    character.currentAttack = character.quaterBackwardCircleStrong;
                    character.currentAttackName = "41236S";
                    break;
            }
        }
        OnAttack();
    }

    // on attack with special move the input from the dpad has to be compared
    protected override AttackTypes InputCompare() {
        AttackTypes result = base.InputCompare();
        if (result != AttackTypes.UNASSIGNED) return result;

        // unassigned, so we assign one, standing / crouching
        if (character.lastInputDirection == LeftInputDirection.bottomLeft ||
            character.lastInputDirection == LeftInputDirection.bottom ||
            character.lastInputDirection == LeftInputDirection.bottomRight)
            return AttackTypes.CROUCHING;

        return AttackTypes.STANDING;
    }
    #endregion
}
