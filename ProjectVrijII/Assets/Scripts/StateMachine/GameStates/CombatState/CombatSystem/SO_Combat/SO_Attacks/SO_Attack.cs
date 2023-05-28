using UnityEngine;

public class SO_Attack : ScriptableObject {

    /// <summary>
    /// Each attack has its own attack type, animation, duration, strength.
    /// All of this should be character specific
    /// </summary>

    [Header("Make sure the amount of strengths is equal to the amount of attacks in the animation!")]
    public float[] strength;
    public Vector2[] enemyLaunchStrength;
    public float[] attackFreezeTime;
    public Vector2[] onHitPushBack;
    [Range(0f, 1f)] public float movementReduction;
    [Range(0f, 1f)] public float fallReduction;
    public bool isSpecial;
    public bool canceledByJump;

    public SO_Attack CopyTo(SO_Attack copy) {
        copy.strength = new float[strength.Length];
        strength.CopyTo(copy.strength, 0);

        copy.enemyLaunchStrength = new Vector2[enemyLaunchStrength.Length];
        enemyLaunchStrength.CopyTo(copy.enemyLaunchStrength, 0);

        copy.attackFreezeTime = new float[attackFreezeTime.Length];
        attackFreezeTime.CopyTo(copy.attackFreezeTime, 0);

        copy.onHitPushBack = new Vector2[onHitPushBack.Length];
        onHitPushBack.CopyTo(copy.onHitPushBack, 0);
        copy.movementReduction = movementReduction;
        copy.fallReduction = fallReduction;
        copy.isSpecial = isSpecial;
        copy.canceledByJump = canceledByJump;
        return copy;
    }

    public void DoAttack(IHitable enemy, int hitNumber) {
        enemy.TakeDamage(strength[hitNumber]);
        Debug.Log($"Enemy took {strength[hitNumber]} damage");
    }

    public void LaunchEnemey(IHitable enemy, int hitNumber, CharacterFacingDirection characterFacingDirection) {
        if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
            enemy.Launch(enemyLaunchStrength[hitNumber], attackFreezeTime[hitNumber]);
        } else {
            enemy.Launch(enemyLaunchStrength[hitNumber] * new Vector2(-1, 1), attackFreezeTime[hitNumber]);
        }
    }

    public void DoPushBack(Rigidbody2D rb, int hitNumber, CharacterFacingDirection characterFacingDirection) {
        if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
            rb.AddForce(onHitPushBack[hitNumber], ForceMode2D.Impulse);
        } else {
            rb.AddForce(onHitPushBack[hitNumber] * new Vector2(-1, 1), ForceMode2D.Impulse);
        }
    }
}
