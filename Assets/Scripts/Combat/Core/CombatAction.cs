using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatAction
{
    public bool IsPlayer;
    public bool IsTanking;
    public bool SkillInUse;
    public bool SwitchingPlaces;
    public bool IbofangSkill;

    public string Class;
    public string Skill;

    public GameObject IbofangTarget1;
    public GameObject IbofangTarget2;
    public GameObject IbofangTarget3;

    public GameObject Agent;
    public GameObject OtherAgent;
    public GameObject Target;

    public int ActionSpeed;
    public int TargetIndex;
    public int OtherAgentTargetIndex = -1;
    public int StaminaCost;
}

