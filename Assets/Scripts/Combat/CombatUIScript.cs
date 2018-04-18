using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIScript : MonoBehaviour {

    public bool SelectAttack;
    public bool SelectSkill;

    public void SelectingAttack()
    {
        SelectAttack = !SelectAttack;
    }

    public void SelectingSkill()
    {
        SelectSkill = !SelectSkill;
    }

}
