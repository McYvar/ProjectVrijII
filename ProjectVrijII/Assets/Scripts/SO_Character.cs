using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "new character")]
public class SO_Character : ScriptableObject
{
    public string characterName;
    public GameObject character;
    public float movementSpeed;

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
}
