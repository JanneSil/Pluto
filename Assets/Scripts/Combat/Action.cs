using System;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public bool IsPlayer;
    public bool IsTanking;
    public bool SkillInUse;

    public string Class;
    public string Skill;

    public GameObject Agent;
    public GameObject Target;

    public int ActionSpeed;
    public int TargetIndex;
    public int StaminaCost;
}

