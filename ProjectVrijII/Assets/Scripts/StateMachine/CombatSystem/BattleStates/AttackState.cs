using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerBaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// A state that starting signature attack and ends when attack combo is finished
    /// </summary>

    [SerializeField] private SO_Character currentCharacter; // later input by player selection maybe?

    [SerializeField] private float comboMaxTime;
    private float comboTimer;
    private uint currentCombo; // higher damage due to an increase in combo's

    private bool didFirstAttack;

    [SerializeField] private LayerMask groundCheckLayers;
    private bool isGrounded;

    private float attackDelay;
    private SO_Attack currentAttack;
    private SO_Attack lastAttack;

    public LinkedList<int> numpadInputOrder = new LinkedList<int>();
    [SerializeField] private float maxInputRememberTime;
    [SerializeField] private ushort maxInputRememberAmount;
    [SerializeField] private InputOrder[] inputOrders;
    private float inputRemeberTimer;

    private LeftInputDirection lastInputDirection = LeftInputDirection.centre;

    private CharacterFacingDirection characterFacingDirection = CharacterFacingDirection.LEFT;

    private float attackStrength;

    private float deadZone = 0.5f;
    private float joyThreshold = 0.3f;

    public override void OnEnter() {
        base.OnEnter();
        currentCombo = 0; // currentCombo should update on a hit
        comboTimer = 0; // comboTimer should reset on a hit
        didFirstAttack = false;
        attackDelay = 0;

        playerInput.eastFirst += OnStrong;
        playerInput.southFirst += OnKick;
        playerInput.westFirst += OnPunch;
    }

    public override void OnExit() {
        base.OnExit();
        playerInput.eastFirst -= OnStrong;
        playerInput.southFirst -= OnKick;
        playerInput.westFirst -= OnPunch;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        isGrounded = GroundCheck();
        PlayerMovement();

        // if combo over, switch to enemy turns
        if (comboTimer > comboMaxTime) {
            //stateManager.SwitchState(typeof(IdleState));
            stateManager.SwitchState(typeof(AttackState));
        } else if (didFirstAttack) comboTimer += Time.deltaTime;

        // if a key combination takes too long, it's essentially canceled
        if (inputRemeberTimer > maxInputRememberTime) {
            ResetNumpadOrder();
        } else {
            inputRemeberTimer += Time.deltaTime;
        }

        if (attackDelay > 0) attackDelay -= Time.deltaTime;



    }

    public bool GroundCheck() {
        Ray ray = new Ray(transform.position, Vector3.down);
        float maxRayDistance = transform.localScale.y; // check actual cast dist sometime in future
        if (Physics.Raycast(ray, maxRayDistance, groundCheckLayers)) return true;
        return false;
    }

    public void OnPunch() {
        if (attackDelay <= 0) {
            bool isAirAttack = false;
            switch (InputCompare(out isAirAttack)) {
                case AttackTypes.STANDING:
                    currentAttack = currentCharacter.standingPunch;
                    break;
                case AttackTypes.CROUCHING:
                    currentAttack = currentCharacter.crouchingPunch;
                    break;
                case AttackTypes.JUMPING:
                    currentAttack = currentCharacter.jumpingPunch;
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    currentAttack = currentCharacter.dragonpunchPunch;
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    currentAttack = currentCharacter.quaterCirclePunch;
                    break;
                case AttackTypes.HALF_CIRCLE:
                    currentAttack = currentCharacter.halfCirclePunch;
                    break;
                default:
                    currentAttack = null;
                    break;
            }
        }

        OnAttack();
    }

    public void OnKick() {
        if (attackDelay <= 0 || lastAttack as SO_Punch) {
            bool isAirAttack = false;
            switch (InputCompare(out isAirAttack)) {
                case AttackTypes.STANDING:
                    currentAttack = currentCharacter.standingKick;
                    break;
                case AttackTypes.CROUCHING:
                    currentAttack = currentCharacter.crouchingKick;
                    break;
                case AttackTypes.JUMPING:
                    currentAttack = currentCharacter.jumpingKick;
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    currentAttack = currentCharacter.dragonpunchKick;
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    currentAttack = currentCharacter.quaterCircleKick;
                    break;
                case AttackTypes.HALF_CIRCLE:
                    currentAttack = currentCharacter.halfCircleKick;
                    break;
                default:
                    currentAttack = null;
                    break;
            }
        }

        OnAttack();
    }

    public void OnStrong() {
        if (attackDelay <= 0 || lastAttack as SO_Punch || lastAttack as SO_Kick) {
            bool isAirAttack = false;
            switch (InputCompare(out isAirAttack)) {
                case AttackTypes.STANDING:
                    currentAttack = currentCharacter.standingStrong;
                    break;
                case AttackTypes.CROUCHING:
                    currentAttack = currentCharacter.crouchingStrong;
                    break;
                case AttackTypes.JUMPING:
                    currentAttack = currentCharacter.jumpingStrong;
                    break;
                case AttackTypes.DRAGON_PUNCH:
                    currentAttack = currentCharacter.dragonpunchStrong;
                    break;
                case AttackTypes.QUARTER_CIRCLE:
                    currentAttack = currentCharacter.quaterCircleStrong;
                    break;
                case AttackTypes.HALF_CIRCLE:
                    currentAttack = currentCharacter.halfCircleStrong;
                    break;
                default:
                    currentAttack = null;
                    break;
            }
        }

        OnAttack();
    }

    public void PlayerMovement() {
        OnDPadValueChanged(playerInput.leftDirection);
    }

    public void OnAttack() {
        if (currentAttack != null) {
            string input = "";
            foreach (ushort i in numpadInputOrder) {
                input += i + ", ";
            }
            Debug.Log(input);

            // if the player does an attack...
            didFirstAttack = true;
            attackDelay = currentAttack.duration;
            attackStrength = currentAttack.strength;

            // then do the actual attack...
            Debug.Log(currentAttack);

            // then reset the attack
            lastAttack = currentAttack;
            currentAttack = null;
        }
    }

    public void OnDPadValueChanged(Vector2 direction) {
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
            inputRemeberTimer = 0;
            switch (currentInputDirection) {
                case LeftInputDirection.bottomLeft:
                    numpadInputOrder.AddLast(1);
                    break;
                case LeftInputDirection.bottom:
                    numpadInputOrder.AddLast(2);
                    break;
                case LeftInputDirection.bottomRight:
                    numpadInputOrder.AddLast(3);
                    break;
                case LeftInputDirection.left:
                    numpadInputOrder.AddLast(4);
                    break;
                case LeftInputDirection.right:
                    numpadInputOrder.AddLast(6);
                    break;
                case LeftInputDirection.topLeft:
                    numpadInputOrder.AddLast(7);
                    break;
                case LeftInputDirection.top:
                    numpadInputOrder.AddLast(8);
                    break;
                case LeftInputDirection.topRight:
                    numpadInputOrder.AddLast(9);
                    break;
                default:
                    numpadInputOrder.AddLast(5);
                    break;
            }
        }
    }

    public void ResetNumpadOrder() {
        numpadInputOrder.Clear();
        lastInputDirection = default;
    }

    // on attack with special move the input from the dpad has to be compared
    public AttackTypes InputCompare(out bool doAirAttack) {
        InputOrder correctInputOrder;

        foreach (InputOrder input in inputOrders) {
            // first we seek for the right combination
            if (input.numpadOrder.Length != numpadInputOrder.Count) continue; // go to next combination if not of same length
            if (input.characterFacingDirection != characterFacingDirection) continue; // or if combination is not facing the same direction

            int[] order = new int[numpadInputOrder.Count];
            numpadInputOrder.CopyTo(order, 0);
            bool doBreak = false;
            for (int i = 0; i < input.numpadOrder.Length; i++) {
                if (order[i] != input.numpadOrder[i]) { doBreak = true; break; }
            }

            if (doBreak) { continue; }

            // if we found the right combination, check if in the air, and the special is allowed in the air
            correctInputOrder = input;
            if (isGrounded) {
                doAirAttack = false;
                return input.attackType;
            } else {
                if (input.allowAir) {
                    doAirAttack = true;
                    return input.attackType;
                }
            }
        }

        // we did not find a right combination, thus depending on standing on the ground or not we assign default attacktypes
        if (isGrounded) {
            doAirAttack = false;
            if (playerInput.leftDPadDirection.y < -deadZone) return AttackTypes.CROUCHING;
            return AttackTypes.STANDING;
        } else {
            doAirAttack = true;
            return AttackTypes.JUMPING;
        }
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
    public bool allowAir;
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