using System.Collections.Generic;
using UnityEngine;

public class AttackState : CharacterBaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// A state that starting signature attack and ends when attack combo is finished
    /// </summary>

    [SerializeField] protected SO_Character currentCharacter; // later input by player selection maybe?

    [SerializeField] private float comboMaxTime;
    private float comboTimer;

    private bool didFirstAttack;

    [SerializeField] private LayerMask groundCheckLayers;

    protected float attackDelay;
    protected SO_Attack currentAttack;
    protected SO_Attack lastAttack;

    public LinkedList<int> numpadInputOrder = new LinkedList<int>();
    [SerializeField] private ushort maxInputRememberAmount;
    [SerializeField] protected InputOrder[] inputOrders;

    protected LeftInputDirection lastInputDirection = LeftInputDirection.centre;

    protected CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.RIGHT;

    private float attackStrength;

    private float joyThreshold = 0.3f;

    protected Rigidbody rb;

    public override void OnAwake() {
        base.OnAwake();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEnter() {
        base.OnEnter();
        comboTimer = 0; // comboTimer should reset on a hit
        didFirstAttack = false;
        attackDelay = 0;
        numpadInputOrder.Clear();
        lastInputDirection = LeftInputDirection.centre;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        LeftInputComboHandler(playerInput.leftDirection);

        // if combo over, switch to enemy turns
        if (comboTimer > comboMaxTime) {
            //stateManager.SwitchState(typeof(IdleState));
        } else if (didFirstAttack) comboTimer += Time.deltaTime;

        if (attackDelay > 0) attackDelay -= Time.deltaTime;
    }

    #region attacks
    public void OnAttack() {
        string input = "";
        foreach (int i in numpadInputOrder) {
            input += i + ", ";
        }
        Debug.Log(input);

        if (currentAttack != null) {
            // if the player does an attack...
            didFirstAttack = true;
            attackDelay = currentAttack.duration;
            //attackStrength = currentAttack.strength[0]; // later follow up attacks

            // then do the actual attack...
            Debug.Log(currentAttack);
            // enemy -> Idamagable interface --> GetDamaged2(strength[0])

            // then reset the attack
            lastAttack = currentAttack;
            currentAttack = null;
            numpadInputOrder.Clear();
        }
    }

    public void LeftInputComboHandler(Vector2 direction) {
        if (direction.magnitude < joyThreshold) {
            return;
        }

        // if the max amount of combo buttons is pressed, remove the one at the bottom
        if (numpadInputOrder.Count > maxInputRememberAmount) {
            numpadInputOrder.RemoveFirst();
        }

        LeftInputDirection currentInputDirection = lastInputDirection;
        float angle = Vector2.Angle(Vector2.right, direction);
        if (direction.y < 0) angle = 360 - angle;

        if ((angle >= 337.5f && angle < 360) || (angle >= 0 && angle < 22.5f)) currentInputDirection = LeftInputDirection.right;
        if (angle >= 22.5f && angle < 67.5f) currentInputDirection = LeftInputDirection.topRight;
        if (angle >= 67.5f && angle < 112.5f) currentInputDirection = LeftInputDirection.top;
        if (angle >= 112.5f && angle < 157.5f) currentInputDirection = LeftInputDirection.topLeft;
        if (angle >= 157.5f && angle < 202.5f) currentInputDirection = LeftInputDirection.left;
        if (angle >= 202.5f && angle < 247.5f) currentInputDirection = LeftInputDirection.bottomLeft;
        if (angle >= 247.5f && angle < 292.5f) currentInputDirection = LeftInputDirection.bottom;
        if (angle >= 292.5f && angle < 337.5f) currentInputDirection = LeftInputDirection.bottomRight;

        if (lastInputDirection != currentInputDirection) {
            lastInputDirection = currentInputDirection;

            numpadInputOrder.AddLast(DirectionKeyValue(currentInputDirection));
        }
    }

    public int DirectionKeyValue(LeftInputDirection inputDirection) {
        switch (inputDirection) {
            case LeftInputDirection.bottomLeft:
                return 1;
            case LeftInputDirection.bottom:
                return 2;
            case LeftInputDirection.bottomRight:
                return 3;
            case LeftInputDirection.left:
                return 4;
            case LeftInputDirection.right:
                return 6;
            case LeftInputDirection.topLeft:
                return 7;
            case LeftInputDirection.top:
                return 8;
            case LeftInputDirection.topRight:
                return 9;
            default:
                return 5;
        }
    }
    #endregion

    public bool GroundCheck() {
        Ray ray = new Ray(transform.position, Vector3.down);
        float maxRayDistance = transform.localScale.y; // check actual cast dist sometime in future
        if (Physics.Raycast(ray, maxRayDistance, groundCheckLayers)) return true;
        return false;
    }
}

public enum CharacterFacingDirection {
    LEFT = 0,
    RIGHT = 1
}

[System.Serializable]
public struct InputOrder {
    public string name;
    public AttackTypes attackType;
    public CharacterFacingDirection characterFacingDirection;
    public ushort[] numpadOrder;
}

public enum AttackTypes {
    STANDING = 0,
    CROUCHING = 1,
    JUMPING = 2,
    DRAGON_PUNCH = 3,
    QUARTER_CIRCLE = 4,
    HALF_CIRCLE = 5
}

public enum LeftInputDirection {
    bottomLeft = 1,
    bottom = 2,
    bottomRight = 3,
    left = 4,
    centre = 5,
    right = 6,
    topLeft = 7,
    top = 8,
    topRight = 9

}