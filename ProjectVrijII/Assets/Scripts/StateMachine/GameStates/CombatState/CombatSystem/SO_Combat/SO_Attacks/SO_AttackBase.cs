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
}
