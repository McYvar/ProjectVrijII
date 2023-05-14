using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAirMovement : AttackState {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// In air movement handler
    /// </summary>

    private float minInAirTime = 0.1f;
    private float inAirTimer;
    private bool didDoubleJump;
    private bool canDoubleJump;

    private bool doDash;
    private bool didDash;

    private Vector2 dashStartingPoint;

    private float originalGravityScale;

    public override void OnEnter() {
        base.OnEnter();
        playerInput.eastFirst += InAirStrong;
        playerInput.southFirst += InAirKick;
        playerInput.westFirst += InAirPunch;
        inAirTimer = 0;
        didDoubleJump = false;
        canDoubleJump = false;

        doDash = false;
        didDash = false;

        //animator.SetTrigger("jump animation?");
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

        if (!didDoubleJump && !canDoubleJump) {
            if (character.lastInputDirection == LeftInputDirection.left ||
                  character.lastInputDirection == LeftInputDirection.centre ||
                  character.lastInputDirection == LeftInputDirection.right) canDoubleJump = true;
        }
        
        if (character.attackPhase == AttackPhase.ready) {
            if (canDoubleJump && !didDoubleJump) {
                if (character.lastInputDirection == LeftInputDirection.top ||
                    character.lastInputDirection == LeftInputDirection.topLeft ||
                    character.lastInputDirection == LeftInputDirection.topRight) {
                    Jump(character.doubleJumpStrength);
                    didDoubleJump = true;
                }
            }
        } else {
            if (canDoubleJump && !didDoubleJump) {
                if (character.lastInputDirection == LeftInputDirection.top ||
                character.lastInputDirection == LeftInputDirection.topLeft ||
                character.lastInputDirection == LeftInputDirection.topRight) {
                    if (character.lastAttack.canceledByJump) {
                        RecoveryInputBuffer = () => {
                            Jump(character.doubleJumpStrength);
                            didDoubleJump = true;
                        };
                    } else {
                        ReadyInputBuffer = () => {
                            Jump(character.doubleJumpStrength);
                            didDoubleJump = true;
                        };
                    }
                }
            }
        }
    }

    protected override void Movement() {
        if (character.lastInputDirection == LeftInputDirection.centre) character.variableMovementSpeed = 0;
        else {
            OnDoublePress -= Dash; // air dash can interup all recovery types
            OnDoublePress += Dash; // air dash can interup all recovery types
        }

        if (character.attackPhase == AttackPhase.ready || character.attackPhase == AttackPhase.recovery) {
            if (doDash && !didDash) {
                if (Mathf.Abs(rb.velocity.x) < 1) {
                    doDash = false;
                    didDash = true;
                }
                if (Vector2.Distance(transform.position, dashStartingPoint) >= character.airDashLength) {
                    doDash = false;
                    didDash = true;
                    rb.gravityScale = originalGravityScale;
                }
                rb.velocity = new Vector2(rb.velocity.x, 0);
                return;
            }
        }

        if (!character.verticalDashAttack) {
            Vector2 horizonal = new Vector2(playerInput.leftDirection.x, 0);
            float resultMovementSpeed = (character.airMovementSpeed + character.variableMovementSpeed) *
                character.attackMovementReductionScalar;
            rb.velocity = new Vector2(horizonal.normalized.x * resultMovementSpeed, rb.velocity.y);
        }
    }

    private void Dash() {
        if (didDash) return;
        int direction = 0;
        if (character.lastInputDirection == LeftInputDirection.left) direction = -1;
        else if (character.lastInputDirection == LeftInputDirection.right) direction = 1;
        else return;

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(direction * character.airDashStrength, 0);

        doDash = true;
        dashStartingPoint = transform.position;
        SetAttackPhase(AttackPhase.ready); // meaning an air dash can be interupted
    }

    #region air attacks
    private void InAirPunch() {
        switch (InputCompare()) {
            case AttackTypes.JUMPING:
                character.currentAttack = character.jumpingPunch;
                character.currentAttackName = "jP";
                break;
            case AttackTypes.QUARTER_CIRCLE:
                character.currentAttack = character.quaterCirclePunch;
                character.currentAttackName = "j236P";
                break;
            case AttackTypes.HALF_CIRCLE:
                character.currentAttack = character.halfCirclePunch;
                character.currentAttackName = "j41236P";
                break;
        }

        OnAttack();
    }

    private void InAirKick() {
        switch (InputCompare()) {
            case AttackTypes.JUMPING:
                character.currentAttack = character.jumpingKick;
                character.currentAttackName = "jK";
                break;
            case AttackTypes.QUARTER_CIRCLE:
                character.currentAttack = character.quaterCircleKick;
                character.currentAttackName = "j236K";
                break;
            case AttackTypes.HALF_CIRCLE:
                character.currentAttack = character.halfCircleKick;
                character.currentAttackName = "j41236K";
                break;
        }

        OnAttack();
    }

    private void InAirStrong() {
        switch (InputCompare()) {
            case AttackTypes.JUMPING:
                character.currentAttack = character.jumpingStrong;
                character.currentAttackName = "jS";
                break;
            case AttackTypes.QUARTER_CIRCLE:
                character.currentAttack = character.quaterCircleStrong;
                character.currentAttackName = "j236S";
                break;
            case AttackTypes.HALF_CIRCLE:
                character.currentAttack = character.halfCircleStrong;
                character.currentAttackName = "j41236S";
                break;   
        }

        OnAttack();
    }

    // on attack with special move the input from the dpad has to be compared
    protected override AttackTypes InputCompare() {
        AttackTypes result = base.InputCompare();
        if (result != AttackTypes.UNASSIGNED) return result;

        // otherwise we assign one
        return AttackTypes.JUMPING;
    }
    #endregion
}
