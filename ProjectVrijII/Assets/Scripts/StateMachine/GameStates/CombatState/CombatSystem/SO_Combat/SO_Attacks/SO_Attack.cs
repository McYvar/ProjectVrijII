using UnityEngine;

public class SO_Attack : ScriptableObject {

    /// <summary>
    /// Each attack has its own attack type, animation, duration, strength.
    /// All of this should be character specific
    /// </summary>

    [Header("Make sure the amount of strengths is equal to the amount of attacks in the animation!")]
    public string attackName;
    public float[] strength;
    public Vector2[] enemyLaunchStrength;
    public float[] attackFreezeTime;
    public Vector2[] onHitPushBack;
    public float[] onHitHitstun;
    public string[] onHitSoundEventName;
    [Range(0f, 1f)] public float movementReduction;
    [Range(0f, 1f)] public float fallReduction;
    public bool isSpecial;
    public bool canceledByJump;

    public SO_Attack CopyTo(SO_Attack copy) {
        copy.attackName = attackName;
        copy.strength = new float[strength.Length];
        strength.CopyTo(copy.strength, 0);

        copy.enemyLaunchStrength = new Vector2[enemyLaunchStrength.Length];
        enemyLaunchStrength.CopyTo(copy.enemyLaunchStrength, 0);

        copy.attackFreezeTime = new float[attackFreezeTime.Length];
        attackFreezeTime.CopyTo(copy.attackFreezeTime, 0);

        copy.onHitPushBack = new Vector2[onHitPushBack.Length];
        onHitPushBack.CopyTo(copy.onHitPushBack, 0);

        copy.onHitHitstun = new float[onHitHitstun.Length];
        onHitHitstun.CopyTo(copy.onHitHitstun, 0);

        copy.onHitSoundEventName = new string[onHitSoundEventName.Length];
        onHitSoundEventName.CopyTo(copy.onHitSoundEventName, 0);

        copy.movementReduction = movementReduction;
        copy.fallReduction = fallReduction;
        copy.isSpecial = isSpecial;
        copy.canceledByJump = canceledByJump;
        return copy;
    }

    public void DoAttack(IHitable enemy, int hitNumber) {
        try {
            enemy.TakeDamage(strength[hitNumber]);
            Debug.Log($"Enemy took {strength[hitNumber]} damage with {attackName}");

            enemy.SetHitstun(onHitHitstun[hitNumber]);
            Debug.Log($"Enemy is stunned for {onHitHitstun[hitNumber]} seconds with {attackName}");
        } catch {
            Debug.LogWarning($"There are too few elements ({strength.Length}) in strength from attack {attackName}." +
                $" Make sure the strength list from attack {attackName} has at least {hitNumber} elements!");
            Debug.LogWarning($"There are too few elements ({onHitHitstun.Length}) in onHitHitstun from attack {attackName}." +
                $" Make sure the onHitHitstun list from attack {attackName} has at least {hitNumber} elements!");
        }
    }

    public void LaunchEnemey(IHitable enemy, int hitNumber, CharacterFacingDirection characterFacingDirection) {
        try {
            if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
                enemy.Launch(enemyLaunchStrength[hitNumber], attackFreezeTime[hitNumber]);
            } else {
                enemy.Launch(enemyLaunchStrength[hitNumber] * new Vector2(-1, 1), attackFreezeTime[hitNumber]);
            }
        } catch {
            if (hitNumber > enemyLaunchStrength.Length) {
                Debug.LogWarning($"There are too few elements ({enemyLaunchStrength.Length}) in enemyLaunchStrength from attack {attackName}." +
                    $" Make sure the enemyLaunchStrength list from attack {attackName} has at least {hitNumber} elements!");
            }

            if (hitNumber > attackFreezeTime.Length) {
                Debug.LogWarning($"There are too few elements ({attackFreezeTime.Length}) in attackFreezeTime from attack {attackName}." +
                    $" Make sure the attackFreezeTime list from attack {attackName} has at least {hitNumber} elements!");
            }
        }
    }

    public bool DoPushBack(Rigidbody2D rb, int hitNumber, CharacterFacingDirection characterFacingDirection) {
        try {
            if (onHitPushBack[hitNumber].magnitude == 0) return false;
            if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
                rb.AddForce(onHitPushBack[hitNumber], ForceMode2D.Impulse);
            } else {
                rb.AddForce(onHitPushBack[hitNumber] * new Vector2(-1, 1), ForceMode2D.Impulse);
            }
            return true;
        } catch {
            Debug.LogWarning($"There are too few elements ({onHitPushBack.Length}) in onHitPushBack from attack {attackName}." +
                $" Make sure the onHitPushBack list from attack {attackName} has at least {hitNumber} elements!");
            return false;
        }
    }

    public void PlaySound(FModEventCaller caller, int hitNumber)
    {
        try
        {
            string eventName = onHitSoundEventName[hitNumber];
            if (eventName.Length > 0) caller.PlayFMODEvent(eventName);
        }
        catch
        {
            Debug.LogWarning($"There are too few elements ({onHitSoundEventName.Length}) in onHitSoundEventName from attack {attackName}." +
                $" Make sure the onHitSoundEventName list from attack {attackName} has at least {hitNumber} elements!");
        }
    }
}
