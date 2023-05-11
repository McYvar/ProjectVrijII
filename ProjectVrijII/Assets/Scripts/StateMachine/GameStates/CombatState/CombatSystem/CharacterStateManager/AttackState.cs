using System;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class AttackState : CharacterBaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// A state that starting signature attack and ends when attack combo is finished
    /// </summary>

    [SerializeField] protected SO_Character character; // later input by player selection maybe?

    [SerializeField] private float comboMaxTime;

    [SerializeField] private LayerMask groundCheckLayers;

    protected float attackDelay;
    protected SO_Attack currentAttack;
    protected SO_Attack lastAttack;

    [SerializeField] protected InputOrder[] inputOrders;

    protected CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.RIGHT;

    private float joyThreshold = 0.3f;

    protected Rigidbody2D rb;
    protected Collider2D myCollider;

    private bool canDoublePress;
    private float maxDoublePressTime = 0.5f;
    protected Action OnDoublePress;

    public override void OnAwake() {
        base.OnAwake();
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    public override void OnEnter() {
        base.OnEnter();
        attackDelay = 0;
        //numpadInputOrder.Clear(); // if this is enabled, then you can't input buffer mid air
        OnDoublePress = null;
        character.attackPhase = AttackPhase.ready;
        lastAttack = null;
    }

    public override void OnUpdate() {
        LeftInputComboHandler(playerInput.leftDirection);

        // if combo over, switch to enemy turns, combo is over when enemy is no longer hitstun
        if (character.attackPhase == AttackPhase.ready) {
            character.attackMovementReductionScalar = 1;
        }
    }

    public override void OnFixedUpdate() {
        Movement();
    }

    #region attacks
    public void OnAttack() {
        string input = "";
        foreach (int i in character.numpadInputOrder) {
            input += i + ", ";
        }
        Debug.Log(input);

        if (currentAttack != null) {
            if (CanAttack()) {
                // if the player does an attack...
                //character.characterAnimator.SetBool(character.currentAttackName, true);
                Debug.Log(character.currentAttackName);
                // enemy -> Idamagable interface --> GetDamaged2(strength[0])
                character.attackMovementReductionScalar = currentAttack.movementReduction;
                character.attackPhase = AttackPhase.startup;

                // then reset the attack
                lastAttack = currentAttack;
                currentAttack = null;
                character.numpadInputOrder.Clear();
            }
        }
    }

    private bool CanAttack() {
        if (lastAttack == null || character.attackPhase == AttackPhase.ready) return true; // no last attack/delay means can attack
        if (!lastAttack.isSpecial && currentAttack.isSpecial) return true; // last attack non special, gets canceled by any special
        if (character.attackPhase == AttackPhase.startup || character.attackPhase == AttackPhase.active) return false; // these phases can only be canceled by specials
        if (lastAttack.isSpecial) return false; // specials can't be canceld
        // then this is recovery phase
        if (lastAttack as SO_Punch && (currentAttack as SO_Kick || currentAttack as SO_Strong)) return true; // non special punch can be canceled by kick/strong
        if (lastAttack as SO_Kick && currentAttack as SO_Strong) return true; // non special kick can be canceld by strong
        return false;
    }

    public void SetAttackPhase(AttackPhase attackPhase) {
        character.attackPhase = attackPhase;
    }

    public void LeftInputComboHandler(Vector2 direction) {
        LeftInputDirection currentInputDirection = character.lastInputDirection;
        if (direction.magnitude < joyThreshold) {
            currentInputDirection = LeftInputDirection.centre;
        } else {
            // if the max amount of combo buttons is pressed, remove the one at the bottom
            if (character.numpadInputOrder.Count > 10) {
                character.numpadInputOrder.RemoveFirst();
            }

            float angle = Vector2.Angle(Vector2.right, direction);
            if (direction.y < 0) angle = 360 - angle;

            if ((angle >= 337.5f && angle < 360) || (angle >= 0 && angle < 22.5f)) currentInputDirection = LeftInputDirection.right;
            else if (angle >= 22.5f && angle < 67.5f) currentInputDirection = LeftInputDirection.topRight;
            else if (angle >= 67.5f && angle < 112.5f) currentInputDirection = LeftInputDirection.top;
            else if (angle >= 112.5f && angle < 157.5f) currentInputDirection = LeftInputDirection.topLeft;
            else if (angle >= 157.5f && angle < 202.5f) currentInputDirection = LeftInputDirection.left;
            else if (angle >= 202.5f && angle < 247.5f) currentInputDirection = LeftInputDirection.bottomLeft;
            else if (angle >= 247.5f && angle < 292.5f) currentInputDirection = LeftInputDirection.bottom;
            else if (angle >= 292.5f && angle < 337.5f) currentInputDirection = LeftInputDirection.bottomRight;
        }

        if (character.lastInputDirection != currentInputDirection) {
            character.lastInputDirection = currentInputDirection;

            character.numpadInputOrder.AddLast(DirectionKeyValue(currentInputDirection));
            //Debug.Log(character.lastInputDirection.ToString());
            if (character.numpadInputOrder.Count > 2 && character.numpadInputOrder.Last.Previous.Value == 5) // system to check if a button was pressed twice in fast succesion
                if (character.numpadInputOrder.Last.Previous.Previous.Value == character.numpadInputOrder.Last.Value &&
                    canDoublePress) {
                    OnDoublePress?.Invoke();
                    OnDoublePress = null;
                    canDoublePress = false;
                } else {
                    canDoublePress = true;
                    CancelInvoke("CancelDoublePress");
                    Invoke("CancelDoublePress", maxDoublePressTime);
                }
        }
    }

    private void CancelDoublePress() {
        canDoublePress = false;
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

    protected virtual AttackTypes InputCompare() {
        foreach (InputOrder requiredOrder in inputOrders) {
            // first we seek for the right combination
            if (character.numpadInputOrder.Count < 3 || character.numpadInputOrder.Count < requiredOrder.numpadOrder.Length) continue; // go to next combination if not right lenght
            if (requiredOrder.characterFacingDirection != characterFacingDirection) continue; // or if combination is not facing the same direction

            int[] inputOrder = new int[character.numpadInputOrder.Count];
            int getLastValues = inputOrder.Length - requiredOrder.numpadOrder.Length;
            character.numpadInputOrder.CopyTo(inputOrder, 0);
            bool doBreak = false;
            for (int i = 0; i < requiredOrder.numpadOrder.Length; i++) {
                if (inputOrder[i + getLastValues] != requiredOrder.numpadOrder[i]) { doBreak = true; break; }
            }

            if (doBreak) continue;

            // if we found the right combination
            return requiredOrder.attackType;
        }

        return AttackTypes.UNASSIGNED;
    }
    #endregion

    protected bool GroundCheck() {
        float maxRayDistance = (myCollider.bounds.size.y / 2) + 0.1f; // check actual cast dist sometime in future
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, maxRayDistance, groundCheckLayers);
        
        if (hit.collider != null) {
            return true;
        }

        return false;
    }

    protected virtual void Movement() { }

    protected virtual void Jump(float jumpStrength) {
        float nearVectorLenght = rb.velocity.magnitude *
            Mathf.Cos(Vector2.Angle(Vector2.down, rb.velocity) * Mathf.Deg2Rad);
        rb.velocity += Vector2.up * nearVectorLenght;
        rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
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
    UNASSIGNED = -1,
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

public enum AttackPhase {
    ready = 0,
    startup = 1,
    active = 2,
    recovery = 3
}