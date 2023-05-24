using System.Collections;
using UnityEngine;

public class CharacterBaseState : BaseState, IHitable {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Base class containing the main functionality for the character
    /// </summary>

    [SerializeField] private LayerMask groundCheckLayers;
    [SerializeField] private ComboCounter comboCounter;
    [SerializeField] protected Transform currentEnemy; // temporaily for facing
    protected Collider2D myCollider;
    protected bool isGrounded;
    protected CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.RIGHT;

    protected bool activeState = false; // this is needed to not fire all animation events multiple times in all states
    protected Rigidbody2D rb;
    protected bool stunned;


    protected Animator animator;
    protected SO_Character character; // later input by player selection maybe?
    protected InputHandler inputHandler;
    private bool isInitialized = false;

    protected virtual void Awake() {
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stunned = false;
    }

    public void PlayerAssignment(InputHandler newInputHandler, SO_Character newCharacter) {
        inputHandler = newInputHandler;
        character = newCharacter;
        animator.runtimeAnimatorController = character.overrideController;
        isInitialized = true;
    }

    public override void OnEnter() {
        activeState = true;
    }

    public override void OnExit() {
        activeState = false;
    }

    public override void OnFixedUpdate() {
        if (!isInitialized) return;
    }

    public override void OnLateUpdate() {
        if (!isInitialized) return;
    }

    public override void OnUpdate() {
        if (!isInitialized) return;
        Debug.Log(inputHandler.gameObject.name);
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
        float maxRayDistance = (myCollider.bounds.size.y / 2) + 0.1f; // check actual cast dist sometime in future
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, maxRayDistance, groundCheckLayers);

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
