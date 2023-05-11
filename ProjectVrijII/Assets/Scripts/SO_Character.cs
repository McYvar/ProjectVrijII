using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "new character")]
public class SO_Character : ScriptableObject
{
    public string characterName;
    public GameObject character;
    public Animator characterAnimator;
    public float groundMovementSpeed;
    [Header("Adds up to the ground movement speed")]
    public float runningMovementSpeed;
    [Header("Reduces from ground movement speed")]
    public float crouchMovementSpeed;
    public float groundJumpStrength;

    public float airMovementSpeed;
    public float airDashStrength;
    public float doubleJumpStrength;
    public float airDashLength;

    [Header("Character attacks")]
    public SO_Kick standingKick;
    public SO_Punch standingPunch;
    public SO_Strong standingStrong;

    public SO_Kick crouchingKick;
    public SO_Punch crouchingPunch;
    public SO_Strong crouchingStrong;
    
    public SO_Kick jumpingKick;
    public SO_Punch jumpingPunch;
    public SO_Strong jumpingStrong;
    
    public SO_Kick dragonpunchKick;
    public SO_Punch dragonpunchPunch;
    public SO_Strong dragonpunchStrong;
    
    public SO_Kick quaterCircleKick;
    public SO_Punch quaterCirclePunch;
    public SO_Strong quaterCircleStrong;
    
    public SO_Kick halfCircleKick;
    public SO_Punch halfCirclePunch;
    public SO_Strong halfCircleStrong;

    [HideInInspector] public LinkedList<int> numpadInputOrder = new LinkedList<int>();
    [HideInInspector] public LeftInputDirection lastInputDirection = LeftInputDirection.centre;
    [HideInInspector] public float variableMovementSpeed = 1;
    [HideInInspector] public float attackMovementReductionScalar = 1;
    [HideInInspector] public string currentAttackName = "";
    [HideInInspector] public AttackPhase attackPhase = AttackPhase.ready;
}
