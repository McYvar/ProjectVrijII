using System;
using UnityEngine;

public class CharacterBaseState : BaseState, INeedInput {
    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Base class containing the main functionality for the character's state.
    /// </summary>

    // Properties
    public InputHandler inputHandler { get; set; }
    public int playerId { get; set; }

    // Variables for ground check and character facing direction
    private LayerMask groundCheckLayers;
    protected Transform currentEnemy; // Temporarily used for facing
    protected Collider2D myCollider;
    protected bool isGrounded;
    protected CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.RIGHT;

    // State-related variables
    protected bool activeState = false; // Used to prevent firing animation events multiple times in all states
    protected Rigidbody2D rb;
    protected Animator animator;
    protected SO_Character character; // May be set by player selection in the future

    // Input buffer actions
    protected Action RecoveryInputBuffer;
    protected Action ReadyInputBuffer;

    protected FModEventCaller myFModEventCaller;

    protected virtual void Awake() {
        // Set up ground check layer mask, collider, animator, and rigidbody
        groundCheckLayers = LayerMask.GetMask("Ground");
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        myFModEventCaller = GetComponent<FModEventCaller>();
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

    public override void OnUpdate() {
        isGrounded = GroundCheck();
        animator.SetBool("isGrounded", isGrounded);

        // character should always face the current enemy
        if (character.attackPhase == AttackPhase.ready) {
            FaceEnemy();
        }
    }

    public void FaceEnemy() {
        if (!activeState) return;
        if (currentEnemy != null) {
            if (currentEnemy.position.x >= transform.position.x) // enemy is to our right
            {
                characterFacingDirection = CharacterFacingDirection.RIGHT;
                transform.localEulerAngles = new Vector3(0, 0, 0);
            } else {
                characterFacingDirection = CharacterFacingDirection.LEFT;
                transform.localEulerAngles = new Vector3(0, 180, 0);
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
                rb.gravityScale = 1;

                character.rbInput = true;
                break;
            case AttackPhase.startup:
                character.attackMovementReductionScalar = 0; // SOLUTION LATER
                break;
            case AttackPhase.active:
                break;
            case AttackPhase.recovery:
                if (RecoveryInputBuffer != null) {
                    RecoveryInputBuffer.Invoke();
                    RecoveryInputBuffer = null;
                    ReadyInputBuffer = null;
                }
                rb.gravityScale = 1;
                break;
        }
    }

    protected virtual void OnDestroy() {
        inputHandler.ResetBindings(); // Reset input bindings
    }

    public override void OnFixedUpdate() {
    }

    public override void OnLateUpdate() {
    }
}
