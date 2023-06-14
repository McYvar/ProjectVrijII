using UnityEngine;

public class InAirMovement : AttackState {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// In air movement handler
    /// </summary>

    private float minInAirTime = 0.1f;
    private float inAirTimer;

    private bool doDash;
    private bool didDash;

    private Vector2 dashStartingPoint;

    public override void OnEnter() {
        base.OnEnter();
        inputHandler.eastFirst += InAirStrong;
        inputHandler.southFirst += InAirKick;
        inputHandler.westFirst += InAirPunch;
        inputHandler.leftShoulderFirst += Dash;

        inAirTimer = 0;

        doDash = false;
        didDash = false;

        animator.SetBool("isGrounded", false);
        character.fallReductionScalar = 1;
    }

    public override void OnExit() {
        base.OnExit();
        inputHandler.eastFirst -= InAirStrong;
        inputHandler.southFirst -= InAirKick;
        inputHandler.westFirst -= InAirPunch;
        inputHandler.leftShoulderFirst -= Dash;

        animator.SetTrigger("landing");
        myFModEventCaller.PlayFMODEvent("event:/SfxLand");
        EndDash();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (inAirTimer > minInAirTime) {
            if (isGrounded) stateManager.SwitchState(typeof(OnGroundMovement));
        } else {
            inAirTimer += Time.deltaTime;
        }


    }

    protected override void CanJump() {
        base.CanJump();
        if (character.attackPhase == AttackPhase.ready) {
            DoJump(character.doubleJumpStrength);
        } else {
            if (character.lastAttack.canceledByJump) {
                RecoveryInputBuffer = () => {
                    DoJump(character.doubleJumpStrength);
                };
            } else {
                ReadyInputBuffer = () => {
                    DoJump(character.doubleJumpStrength);
                };
            }
        }
    }

    protected override void Movement() {
        if (character.lastInputDirection == LeftInputDirection.centre) character.variableMovementSpeed = 0;
        else if (character.lastInputDirection == LeftInputDirection.left || character.lastInputDirection == LeftInputDirection.right) {
            OnDoublePress -= Dash;
            OnDoublePress += Dash;
        }

        if (character.attackPhase == AttackPhase.ready || character.attackPhase == AttackPhase.recovery) {
            if (doDash && !didDash) {
                if (Mathf.Abs(rb.velocity.x) < 1) {
                    EndDash();
                }
                if (Vector2.Distance(transform.position, dashStartingPoint) >= character.airDashLength) {
                    EndDash();
                }
                rb.velocity = new Vector2(rb.velocity.x, 0);
                return;
            }
        }

        if ((character.attackPhase == AttackPhase.startup || character.attackPhase == AttackPhase.active) &&
            character.rbInput) {
            rb.velocity = new Vector2(
                rb.velocity.x,
                rb.velocity.y > 0 ? rb.velocity.y : rb.velocity.y * character.fallReductionScalar);
        }
    }

    private void EndDash() {
        rb.velocity = new Vector2(rb.velocity.x * character.airDashStopScalar, rb.velocity.y);
        doDash = false;
        didDash = true;
        rb.gravityScale = 1;
    }

    private void Dash() {
        if (didDash || !character.rbInput) return;
        myFModEventCaller.PlayFMODEvent("event:/SfxDash");
        int direction = 0;
        if (character.lastInputDirection == LeftInputDirection.left) direction = -1;
        else if (character.lastInputDirection == LeftInputDirection.right) direction = 1;
        else if (characterFacingDirection == CharacterFacingDirection.LEFT) direction = -1;
        else direction = 1;

        rb.gravityScale = 0;
        rb.velocity = new Vector2(direction * character.airDashStrength, 0);

        doDash = true;
        dashStartingPoint = transform.position;
        SetAttackPhase(AttackPhase.ready); // meaning an air dash can be interupted
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (!activeState) return;
        if (collision.gameObject != null) {
            if (collision.gameObject.layer == 11) {
                EndDash();
            }
        }
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
            case AttackTypes.QUARTER_CIRCLE_BACKWARD:
                character.currentAttack = character.quaterBackwardCirclePunch;
                character.currentAttackName = "j41236P";
                break;
        }

        OnAttack();
    }

    private void InAirKick()
    {
        if (inputHandler.leftTrigger > 0.7f)
        {
            character.currentAttack = character.quaterBackwardCircleKick;
            character.currentAttackName = "j41236K";
        }
        else
        {
            switch (InputCompare())
            {
                case AttackTypes.JUMPING:
                    character.currentAttack = character.jumpingKick;
                    character.currentAttackName = "jK";
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    character.currentAttack = character.quaterCircleKick;
                    character.currentAttackName = "j236K";
                    break;
                case AttackTypes.QUARTER_CIRCLE_BACKWARD:
                    character.currentAttack = character.quaterBackwardCircleKick;
                    character.currentAttackName = "j41236K";
                    break;
            }
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
            case AttackTypes.QUARTER_CIRCLE_BACKWARD:
                character.currentAttack = character.quaterBackwardCircleStrong;
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
