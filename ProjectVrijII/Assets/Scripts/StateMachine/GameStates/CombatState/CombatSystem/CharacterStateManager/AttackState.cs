using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AttackState : CharacterBaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// A state that starting signature attack and ends when attack combo is finished
    /// </summary>

    [SerializeField] protected InputOrder[] inputOrders;

    protected float attackDelay;

    private float joyThreshold = 0.3f;

    private bool canDoublePress;
    private float maxDoublePressTime = 0.5f;
    protected Action OnDoublePress;

    [SerializeField] Collider2D[] hitboxes;
    [SerializeField] LayerMask hitLayer;

    protected Action RecoveryInputBuffer;
    protected Action ReadyInputBuffer;

    protected override void Awake() {
        base.Awake();
    }

    public override void OnEnter() {
        base.OnEnter();
        attackDelay = 0;
        //numpadInputOrder.Clear(); // if this is enabled, then you can't input buffer mid air
        OnDoublePress = null;
        //SetAttackPhase(AttackPhase.ready);
        ReadyInputBuffer = null; // in air and on ground buffered attacks/actions can't be buffered once we switched states
        RecoveryInputBuffer = null;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.Space)) stateManager.SwitchState(typeof(OnGroundMovement));
        LeftInputComboHandler(inputHandler.leftDirection);
    }

    public override void OnFixedUpdate() {
        Movement();
    }

    #region attacks
    public void OnAttack() {
        /*
        string input = "";
        foreach (int i in character.numpadInputOrder) {
            input += i + ", ";
        }
        Debug.Log(input);
        */
        if (character.currentAttack != null) {
            if (CanAttackInstant()) DoAttack(character.currentAttack);
            else if (CanBufferRecovery()) RecoveryInputBuffer = () => { 
                character.rbInput = true; DoAttack(character.currentAttack); 
            }; // can now be buffered before recovery
            else if (CanAttackInRecovery()) {
                character.rbInput = true;
                DoAttack(character.currentAttack);
            } else ReadyInputBuffer = () => { DoAttack(character.currentAttack); };
        }
    }

    private void DoAttack(SO_Attack newAttack) {
        // if the player does an attack...
        if (newAttack == null) return; // has to be fixed later on...
        animator.SetTrigger(character.currentAttackName);
        character.attackMovementReductionScalar = newAttack.movementReduction;
        character.fallReductionScalar = newAttack.fallReduction; // implement
        SetAttackPhase(AttackPhase.startup);

        // then reset the attack
        character.lastAttack = character.currentAttack;
        character.currentAttack = null;
        character.numpadInputOrder.Clear();
    }

    private bool CanAttackInstant() {
        if (character.lastAttack == null || character.attackPhase == AttackPhase.ready) return true; // no last attack/delay means can attack
        return false;
    }

    private bool CanBufferRecovery() {
        if (character.attackPhase == AttackPhase.startup || character.attackPhase == AttackPhase.active) { // these phases can only be canceled by specials
            if (character.lastAttack.isSpecial) return false; // specials can't be canceld
            if (!character.lastAttack.isSpecial && character.currentAttack.isSpecial) return true; // last attack non special, gets canceled by any special
            if (character.lastAttack as SO_Punch && (character.currentAttack as SO_Kick || character.currentAttack as SO_Strong)) return true; // non special punch can be canceled by kick/strong
            if (character.lastAttack as SO_Kick && character.currentAttack as SO_Strong) return true; // non special kick can be canceld by strong
        }
        return false;
    }

    private bool CanAttackInRecovery() {
        if (character.attackPhase == AttackPhase.recovery) {
            if (!character.lastAttack.isSpecial && character.currentAttack.isSpecial) return true; // last attack non special, gets canceled by any special
            if (character.lastAttack.isSpecial) return false; // specials can't be canceld
            if (character.lastAttack as SO_Punch && (character.currentAttack as SO_Kick || character.currentAttack as SO_Strong)) return true; // non special punch can be canceled by kick/strong
            if (character.lastAttack as SO_Kick && character.currentAttack as SO_Strong) return true; // non special kick can be canceld by strong
        }
        return false;
    }

    public void SetAttackPhase(AttackPhase attackPhase) {
        if (!activeState) return;

        character.SetAttackPhase(attackPhase);

        // reset values here
        switch (attackPhase) {
            case AttackPhase.ready:
                character.attackMovementReductionScalar = 1;
                if (ReadyInputBuffer != null) {
                    ReadyInputBuffer.Invoke();
                    ReadyInputBuffer = null;
                    RecoveryInputBuffer = null;
                }

                character.rbInput = true;
                break;
            case AttackPhase.startup:
                break;
            case AttackPhase.active:
                break;
            case AttackPhase.recovery:
                if (RecoveryInputBuffer != null) {
                    RecoveryInputBuffer.Invoke();
                    RecoveryInputBuffer = null;
                    ReadyInputBuffer = null;
                }
                break;
        }
    }

    public void StartRecoveryInputBuffer() {
        if (!activeState) return;
        RecoveryInputBuffer = null;
    }

    public void StartReadyInputBuffer() {
        if (!activeState) return;
        ReadyInputBuffer = null;
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

    public void TriggerHit(int hitNumber) {
        if (!activeState) return;
        List<IHitable> hitableEntities = new List<IHitable>();

        for (int i = 0; i < hitboxes.Length; i++) {
            Collider2D[] collisions = Physics2D.OverlapBoxAll(hitboxes[i].bounds.center, hitboxes[i].bounds.size, 0, hitLayer);

            // first we check for each hitbox if there are hitable entities in there and add them to a list
            foreach (var hit in collisions) {
                if (hit.transform == transform) continue;
                if (hit.GetComponent<IHitable>() != null) {
                    //Debug.Log(hit + " was hit in collision zone " + i);
                    if (!hitableEntities.Contains(hit.GetComponent<IHitable>()))
                        hitableEntities.Add(hit.GetComponent<IHitable>());
                }
            }
        }

        // then foreach hitable entity we call upon their function
        foreach (var entity in hitableEntities) {
            try {
                if (characterFacingDirection == CharacterFacingDirection.RIGHT) {
                    entity.OnHit(character.lastAttack.enemyLaunchStrength[hitNumber], character.lastAttack.attackFreezeTime[hitNumber]);
                    if (character.lastAttack.onHitPushBack[hitNumber].magnitude > 0) {
                        character.rbInput = false;
                        rb.AddForce(character.lastAttack.onHitPushBack[hitNumber], ForceMode2D.Impulse);
                    }
                } else {
                    entity.OnHit(character.lastAttack.enemyLaunchStrength[hitNumber] * new Vector2(-1, 1), character.lastAttack.attackFreezeTime[hitNumber]);
                    if (character.lastAttack.onHitPushBack[hitNumber].magnitude > 0) {
                        character.rbInput = false;
                        rb.AddForce(character.lastAttack.onHitPushBack[hitNumber] * new Vector2(-1, 1), ForceMode2D.Impulse);
                    }
                }
                OnHitEnemy();
                // enymy take damage with strength
            }
            catch {
                string error = character.lastAttack.name + " " + hitNumber + " has too few elements listed in: ";
                if (character.lastAttack.strength.Length - 1 < hitNumber) error += "strength: " + (character.lastAttack.strength.Length - 1) + " ";
                if (character.lastAttack.enemyLaunchStrength.Length - 1 < hitNumber) error += "enemyLaunchStrength: " + (character.lastAttack.enemyLaunchStrength.Length - 1) + " ";
                if (character.lastAttack.attackFreezeTime.Length - 1 < hitNumber) error += "attackFreezeTime: " + (character.lastAttack.attackFreezeTime.Length - 1) + " ";
                if (character.lastAttack.onHitPushBack.Length - 1 < hitNumber) error += "onHitPushBack: " + (character.lastAttack.onHitPushBack.Length - 1);
                Debug.Log(error);
            }
        }
    }

    public void LaunchVertical(float launchStrength)
    {
        if (!activeState) return;
        rb.AddForce(Vector2.up * launchStrength, ForceMode2D.Impulse);
        character.rbInput = false;
    }

    public void LaunchHorizontal(float launchStrength)
    {
        if (!activeState) return;
        rb.AddForce(transform.right * launchStrength, ForceMode2D.Impulse);
        character.rbInput = false;
    }

    public void RecoverLaunch()
    {
        if (!activeState) return;
        character.rbInput = true;
    }
    #endregion

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
    QUARTER_CIRCLE_BACKWARD = 5
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