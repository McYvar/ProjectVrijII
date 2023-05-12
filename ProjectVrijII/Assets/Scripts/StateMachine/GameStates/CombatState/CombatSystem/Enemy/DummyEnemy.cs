using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : CombatBase, IHitable {

    private Rigidbody2D rb;
    [SerializeField] Vector3 centreOfStage;
    float hitstunDuration = 1f; // later in animation frames again
    float hitstunTimer = 0f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnUpdate() {
        if (hitstunTimer > 0) hitstunTimer -= Time.deltaTime;
        else rb.velocity = (centreOfStage - transform.position).normalized * 2;
    }

    public void OnHit(Vector2 force) {
        rb.AddForce(force, ForceMode2D.Impulse);
        hitstunTimer = hitstunDuration;
    }
}
