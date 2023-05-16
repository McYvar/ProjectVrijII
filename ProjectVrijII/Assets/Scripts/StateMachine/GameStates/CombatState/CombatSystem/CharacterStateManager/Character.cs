using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Character : CombatBase {

    Rigidbody2D rb;
    Animator animator;

    [SerializeField] private SO_Character character;
    [SerializeField] private Vector3 point;
    [SerializeField] private float speed;
    private bool stunned = false;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void OnHit(Vector2 force) {
        stunned = true;
        rb.AddForce(force, ForceMode2D.Impulse);
        if (force.x > 0) transform.localEulerAngles = new Vector3(0, 0, 0);
        else if (force.x < 0) transform.localEulerAngles = new Vector3(0, 180, 0);
        animator.SetTrigger("stunned");
        // take damage...
    }

    public override void OnFixedUpdate() {
        base.OnFixedUpdate();
        if (!character.isStunned && !stunned) {
            rb.velocity = new Vector2((point.x - transform.position.x) * speed, rb.velocity.y);
        } else stunned = false;
    }
}
