using UnityEngine;

public class OnGroundMovement : AttackState {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// On ground movement handler
    /// </summary>
    [SerializeField] float jumpForce;
    private bool canJump;

    public override void OnEnter() {
        base.OnEnter();
        playerInput.eastFirst += OnGroundStrong;
        playerInput.southFirst += OnGroundKick;
        playerInput.westFirst += OnGroundPunch;
        canJump = false;

        animator.SetBool("isGrounded", true);
        //animator.SetTrigger("landing animaiton?");
    }

    public override void OnExit() {
        base.OnExit();
        playerInput.eastFirst -= OnGroundStrong;
        playerInput.southFirst -= OnGroundKick;
        playerInput.westFirst -= OnGroundPunch;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (!isGrounded) stateManager.SwitchState(typeof(InAirMovement));

        if (character.lastInputDirection == LeftInputDirection.left ||
                character.lastInputDirection == LeftInputDirection.centre ||
                character.lastInputDirection == LeftInputDirection.right) canJump = true;

        if (character.attackPhase == AttackPhase.ready) {
            if (character.lastInputDirection == LeftInputDirection.bottom)
                animator.SetInteger("Stance", 1);
            else if (character.lastInputDirection == LeftInputDirection.centre)
                animator.SetInteger("Stance", 0);

            if (canJump) {
                if (character.lastInputDirection == LeftInputDirection.top ||
                               character.lastInputDirection == LeftInputDirection.topLeft ||
                               character.lastInputDirection == LeftInputDirection.topRight) {
                    Jump(character.groundJumpStrength);
                    stateManager.SwitchState(typeof(InAirMovement));
                }
            }
        } else {
            if (canJump) {
                if (character.lastInputDirection == LeftInputDirection.top ||
                    character.lastInputDirection == LeftInputDirection.topLeft ||
                    character.lastInputDirection == LeftInputDirection.topRight) {
                    if (character.lastAttack.canceledByJump) {
                        RecoveryInputBuffer = () => {
                            Jump(character.groundJumpStrength);
                            stateManager.SwitchState(typeof(InAirMovement));
                        };
                    } else {
                        ReadyInputBuffer = () => {
                            Jump(character.groundJumpStrength);
                            stateManager.SwitchState(typeof(InAirMovement));
                        };
                    }
                }
            }
        }

    }

    protected override void Movement() {
        if (character.lastInputDirection == LeftInputDirection.bottomLeft || // crouching movement
            character.lastInputDirection == LeftInputDirection.bottom ||
            character.lastInputDirection == LeftInputDirection.bottomRight) {

            Vector2 horizonal = new Vector2(playerInput.leftDirection.x, 0);
            rb.velocity = new Vector2(horizonal.normalized.x * character.crouchMovementSpeed *
                character.attackMovementReductionScalar, rb.velocity.y);

            //animator.SetInteger("walking", 1);

        } else { // walking/running movement
            if (character.lastInputDirection == LeftInputDirection.centre) character.variableMovementSpeed = 0;
            else {
                OnDoublePress -= SetVariableSpeedToRunning;
                OnDoublePress += SetVariableSpeedToRunning;
            }

            Vector2 horizonal = new Vector2(playerInput.leftDirection.x, 0);
            float resultMovementSpeed =
                (character.groundMovementSpeed + character.variableMovementSpeed) *
                character.attackMovementReductionScalar;
            rb.velocity = new Vector2(
                horizonal.normalized.x * resultMovementSpeed,
                rb.velocity.y);

            if (character.variableMovementSpeed == 0) { // means walking
                //animator.SetInteger("walking", 1);
            } else { // means running
                //animator.SetInteger("walking", 1);
            }
        }
    }

    private void SetVariableSpeedToRunning() {
        character.variableMovementSpeed = character.runningMovementSpeed;
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
            case AttackTypes.HALF_CIRCLE:
                character.currentAttack = character.halfCirclePunch;
                character.currentAttackName = "41236P";
                break;
        }

        OnAttack();
    }

    private void OnGroundKick() {
        switch (InputCompare()) {
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
            case AttackTypes.HALF_CIRCLE:
                character.currentAttack = character.halfCircleKick;
                character.currentAttackName = "41236K";
                break;
        }

        OnAttack();
    }

    private void OnGroundStrong() {
        switch (InputCompare()) {
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
            case AttackTypes.HALF_CIRCLE:
                character.currentAttack = character.halfCircleStrong;
                character.currentAttackName = "41236S";
                break;
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
