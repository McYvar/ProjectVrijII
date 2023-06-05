using System.Collections;
using UnityEngine;

public class IdleState : CharacterBaseState, IHitable {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Idle state while the enemy turn is processed
    /// </summary>
    private Health health;

    private float hitstunTimer = 0;
    private bool isStunned;

    protected override void Awake() {
        base.Awake();
        health = GetComponent<Health>();
    }

    public override void OnEnter() {
        base.OnEnter();
        rb.velocity = Vector2.zero;
    }

    public override void OnExit() {
        base.OnExit();
        SetAttackPhase(AttackPhase.ready);
    }

    public void Launch(Vector2 force, float freezeTime) {
        if (!activeState) return;
        rb.AddForce(force, ForceMode2D.Impulse);
        if (force.x > 0) transform.localEulerAngles = new Vector3(0, 0, 0);
        else if (force.x < 0) transform.localEulerAngles = new Vector3(0, 180, 0);

        // hit freeze
        Time.timeScale = 0; // maybe later use a variable timescale ...
        StartCoroutine(ResumeTime(freezeTime));

        // take damage...
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (hitstunTimer > 0) {
            hitstunTimer -= Time.deltaTime;
            isStunned = true;
        } else if (isStunned) {
            isStunned = false;
            TurnSystem.Instance.OnReset.Invoke();
            animator.SetBool("isStunned", false);
        }
    }

    private IEnumerator ResumeTime(float time) {
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }

    public void TakeDamage(float damage) {
        health.GetDamaged((int)damage);
    }

    public void SetHitstun(float time) {
        if (!activeState) return;
        animator.SetBool("isStunned", true);
        TurnSystem.Instance.OnHit.Invoke(time);
        hitstunTimer = time;
    }
}
