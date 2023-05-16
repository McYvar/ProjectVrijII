using UnityEngine;

public class CharacterBaseState : BaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Base class containing the main functionality for the character
    /// </summary>

    [SerializeField] protected SO_Character character; // later input by player selection maybe?
    [HideInInspector] protected PlayerInput playerInput;
    [SerializeField] private LayerMask groundCheckLayers;
    [SerializeField] protected Transform currentEnemy; // temporaily for facing
    [SerializeField] private ComboCounter comboCounter;
    protected Collider2D myCollider;
    protected Animator animator;
    protected bool isGrounded;
    protected CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.RIGHT;

    protected bool activeState = false; // this is needed to not fire all animation events multiple times in all states

    protected virtual void Awake() {
        playerInput = GetComponent<PlayerInput>();
        myCollider = GetComponent<Collider2D>();

        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = character.overrideController;
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
            if (currentEnemy.position.x >= transform.position.x) { // enemy is to our right
                characterFacingDirection = CharacterFacingDirection.RIGHT;
                transform.localEulerAngles = new Vector3(0, 0, 0);
            } else {
                characterFacingDirection = CharacterFacingDirection.LEFT;
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
        }
    }

    public void OnEndTurnState() {
        stateManager.SwitchState(typeof(IdleState));
    }

    public void SelectGoBackState() {
        stateManager.SwitchState(typeof(SelectionState));
    }

    private bool GroundCheck() {
        float maxRayDistance = (myCollider.bounds.size.y / 2) + 0.1f; // check actual cast dist sometime in future
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, maxRayDistance, groundCheckLayers);

        if (hit.collider != null) {
            return true;
        }

        return false;
    }

    public void DoStun() {
        if (!activeState) return;
        character.isStunned = true;
    }

    public void NoLongerStunned() {
        if (!activeState) return;
        character.isStunned = false;
        comboCounter.ResetCombo();
    }

    protected void OnHitEnemy() {
        comboCounter.IncreaseCombo();
    }
}
