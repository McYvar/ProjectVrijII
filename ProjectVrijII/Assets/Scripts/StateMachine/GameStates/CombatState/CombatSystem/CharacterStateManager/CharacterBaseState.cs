using System;
using System.Collections;
using UnityEngine;

public class CharacterBaseState : BaseState, INeedInput {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Base class containing the main functionality for the character
    /// </summary>
    
    public InputHandler inputHandler { get; set; }
    public int playerId { get; set; }

    private LayerMask groundCheckLayers;
    protected Transform currentEnemy; // temporaily for facing
    protected Collider2D myCollider;
    protected bool isGrounded;
    protected CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.RIGHT;

    protected bool activeState = false; // this is needed to not fire all animation events multiple times in all states
    protected Rigidbody2D rb;

    protected Animator animator;
    protected SO_Character character; // later input by player selection maybe?

    protected Action RecoveryInputBuffer;
    protected Action ReadyInputBuffer;

    protected virtual void Awake() {
        groundCheckLayers = LayerMask.GetMask("Ground");
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public void SetInputHandler(InputHandler newInputHandler) {
        inputHandler = newInputHandler;
    }

    public void SetCharacter(SO_Character newCharacter) {
        character = newCharacter;
        animator.runtimeAnimatorController = character.overrideController;
    }

    public void SetEnemy(Transform enemy) {
        currentEnemy = enemy;
    }

    public override void OnEnter() {
        activeState = true;
    }

    public override void OnExit() {
        activeState = false;
    }

    public override void OnFixedUpdate() {
    }

    public override void OnLateUpdate() {
    }

    public override void OnUpdate() {
        isGrounded = GroundCheck();
        animator.SetBool("isGrounded", isGrounded);

        // character always faces the current enemy, as should the enemy also face our character, but thats for later
        if (character.attackPhase == AttackPhase.ready) {
            if (currentEnemy != null) {
                if (currentEnemy.position.x >= transform.position.x) { // enemy is to our right
                    characterFacingDirection = CharacterFacingDirection.RIGHT;
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                } else {
                    characterFacingDirection = CharacterFacingDirection.LEFT;
                    transform.localEulerAngles = new Vector3(0, 180, 0);
                }
            }
        }
    }

    private bool GroundCheck() {
        float maxRayDistance = 0.1f; // check actual cast dist sometime in future
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, maxRayDistance, groundCheckLayers);

        Debug.DrawLine(transform.position, transform.position + Vector3.down * maxRayDistance);

        if (hit.collider != null) {
            return true;
        }

        return false;
    }

    public void SetAttackPhase(AttackPhase attackPhase) {
        if (!activeState) return;

        character.SetAttackPhase(attackPhase);

        // reset values here
        switch (attackPhase) {
            case AttackPhase.ready:
                character.attackMovementReductionScalar = 1;
                if (ReadyInputBuffer != null) {
                    ReadyInputBuffer.Invoke();
                    ReadyInputBuffer = null;
                    RecoveryInputBuffer = null;
                }

                character.rbInput = true;
                break;
            case AttackPhase.startup:
                break;
            case AttackPhase.active:
                break;
            case AttackPhase.recovery:
                if (RecoveryInputBuffer != null) {
                    RecoveryInputBuffer.Invoke();
                    RecoveryInputBuffer = null;
                    ReadyInputBuffer = null;
                }
                break;
        }
    }

    private void OnDestroy() {
        inputHandler.ResetBindings();
    }
}