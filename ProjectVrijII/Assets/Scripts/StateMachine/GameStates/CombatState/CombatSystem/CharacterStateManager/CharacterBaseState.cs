using System.Collections;
using UnityEngine;

public class CharacterBaseState : BaseState, IHitable {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Base class containing the main functionality for the character
    /// </summary>

    [SerializeField] private LayerMask groundCheckLayers;
    [SerializeField] private ComboCounter comboCounter;
    protected Transform currentEnemy; // temporaily for facing
    protected Collider2D myCollider;
    protected bool isGrounded;
    protected CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.RIGHT;

    protected bool activeState = false; // this is needed to not fire all animation events multiple times in all states
    protected Rigidbody2D rb;
    protected bool stunned;

    protected Animator animator;
    protected SO_Character character; // later input by player selection maybe?
    protected InputHandler inputHandler;

    protected virtual void Awake() {
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stunned = false;
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

    public void OnEndTurnState() {
        stateManager.SwitchState(typeof(IdleState));
    }

    public void SelectGoBackState() {
        stateManager.SwitchState(typeof(SelectionState));
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

    public void OnHit(Vector2 force, float freezeTime) {
        stunned = true;
        stateManager.SwitchState(typeof(IdleState)); // GOTTA BE REMOVED VERY SOON AGAIN JUST FOR TESTING THIS!!!
        rb.AddForce(force, ForceMode2D.Impulse);
        if (force.x > 0) transform.localEulerAngles = new Vector3(0, 0, 0);
        else if (force.x < 0) transform.localEulerAngles = new Vector3(0, 180, 0);
        animator.SetTrigger("stunned");

        // hit freeze
        Time.timeScale = 0; // maybe later use a variable timescale ...
        StartCoroutine(ResumeTime(freezeTime));

        // take damage...
    }

    private IEnumerator ResumeTime(float time) {
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
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

public interface IHitable {
    void OnHit(Vector2 force, float freezeTime);
}
