using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIScript : MonoBehaviour {

    public bool SelectAttack;
    public bool SelectSkill;

    private BattleManager BM;

    public void Start()
    {
        BM = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    public void SelectingAttack()
    {
        SelectAttack = !SelectAttack;

        if (BM.SelectedCharacter.GetComponent<Character>().Attacking == true || BM.SelectedCharacter.GetComponent<Character>().UsingSkill == true)
        {
            for (int i = 0; i < BM.ActionList.Count; ++i)
            {
                if (BM.ActionList[i].Agent == BM.SelectedCharacter)
                {
                    BM.ActionList[i].Agent.GetComponent<Character>().AvailableStamina += BM.ActionList[i].StaminaCost;

                    if (BM.ActionList[i].Target != null)
                    {
                        BM.ActionList[i].Target.GetComponent<Character>().Targeted = false;
                    }

                    if (BM.ActionList[i].SkillInUse)
                    {
                        BM.SelectedCharacter.GetComponent<Character>().ActionPoints += 3;
                        BM.SelectedCharacter.GetComponent<Character>().UsingSkill = false;
                        BM.SelectedCharacter.GetComponent<Character>().SkillBeingUsed = "";
                    }
                    else
                    {
                        BM.SelectedCharacter.GetComponent<Character>().ActionPoints += 2;
                    }
                    BM.ActionList.RemoveAt(i);
                }
            }

            BM.SelectedCharacter.GetComponent<Character>().Attacking = false;

            return;
        }
    }

    public void SelectingSkill()
    {
        SelectSkill = !SelectSkill;
    }

}
