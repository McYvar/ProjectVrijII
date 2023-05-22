using UnityEngine;

public class SO_Attack : ScriptableObject {

    /// <summary>
    /// Each attack has its own attack type, animation, duration, strength.
    /// All of this should be character specific
    /// </summary>

    [Header("Make sure the amount of strengths is equal to the amount of attacks in the animation!")]
    public float[] strength;
    public Vector2[] lauchStrenght;
    [Range(0f, 1f)] public float movementReduction;
    [Range(0f, 1f)] public float fallReduction;
    public bool isSpecial;
    public bool canceledByJump;
    public Vector2 onHitPushBack;

    [Header("Should be used in air attacks only!")]
    public Vector2 attackBounce;
}
