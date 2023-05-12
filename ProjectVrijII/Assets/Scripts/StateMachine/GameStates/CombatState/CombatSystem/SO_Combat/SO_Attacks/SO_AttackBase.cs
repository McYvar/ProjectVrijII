using UnityEngine;

public class SO_Attack : ScriptableObject {

    /// <summary>
    /// Each attack has its own attack type, animation, duration, strength.
    /// All of this should be character specific
    /// </summary>

    [Header("Make sure the amount of strengths is equal to the amount of attacks in the animation!")]
    public float[] strength; // should we make this randomized a bit?
    [Range(0f, 1f)] public float movementReduction;
    [Range(0f, 1f)] public float fallReduction;
    [Header("Should be used in air attacks only!")]
    public Vector2 attackBounce;
    public bool isSpecial;
    public bool canceledByJump;
}
