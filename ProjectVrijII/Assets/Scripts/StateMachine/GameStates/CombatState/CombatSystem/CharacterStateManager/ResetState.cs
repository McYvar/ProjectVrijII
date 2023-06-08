using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetState : CharacterBaseState {

    /// Date: 5/28/2023, by: Yvar
    /// <summary>
    /// Character walks back to their original position
    /// </summary>

    private Vector3 startPos = Vector3.zero;
    private bool hasResetted = false;
    private float minmumResetDistance = 0.3f;

    private void Start() {
        startPos = transform.position;
    }

    public override void OnEnter() {
        base.OnEnter();
        hasResetted = false;
        animator.SetInteger("Stance", 0);
        animator.SetTrigger("reset");
        character.OnStartUp();
        SetAttackPhase(AttackPhase.recovery);
    }

    public override void OnExit() {
        SetAttackPhase(AttackPhase.ready);
        base.OnExit();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (!isGrounded) return;
        if (!hasResetted) {
            if (Mathf.Abs(transform.position.x - startPos.x) < minmumResetDistance) {
                hasResetted = true;
                TurnSystem.Instance.ReadyForNextTurn();
                animator.SetInteger("Move", 0);
            }
            else {
                // walk towards reset
                Vector2 direction = new Vector2(startPos.x - transform.position.x, 0);
                rb.velocity = direction.normalized * character.groundMovementSpeed;
                if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
                    if (direction.x > 0) animator.SetInteger("Move", 1); // walking foward facing right
                    if (direction.x < 0) animator.SetInteger("Move", -1); // walking backward facing right
                }
                else {
                    if (direction.x > 0) animator.SetInteger("Move", -1); // walking backward facing left
                    if (direction.x < 0) animator.SetInteger("Move", 1); // walking forward facing left
                }
            }
        }
    }

}
