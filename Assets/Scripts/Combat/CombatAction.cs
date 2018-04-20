using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatAction
{
    public bool IsPlayer;
    public bool IsTanking;
    public bool SkillInUse;
    public bool SwitchingPlaces;

    public string Class;
    public string Skill;

    public GameObject Agent;
    public GameObject OtherAgent;
    public GameObject Target;

    public int ActionSpeed;
    public int TargetIndex;
    public int OtherAgentTargetIndex;
    public int StaminaCost;
}

