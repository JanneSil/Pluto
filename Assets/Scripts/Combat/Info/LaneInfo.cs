using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneInfo : MonoBehaviour {

    public int LanePos;
    public bool LaneChosen;
    public GameObject UnitOnLane;
    public bool TargetedByIbofang;
    public bool tempTargetByIbofang;
    private bool doOnce;
    private bool doOnceLaneChoose;
    private bool turnChanging;
    private SpriteRenderer ibofangSR;
    private SpriteRenderer laneColor;
    private BattleManager BM;

    private Color greenColor = new Color32(165, 144, 118, 255);
    private Color grayColor = Color.gray;
    private Color redColor = Color.red;

    private void Start()
    {
        greenColor.a = 0.30f;
        grayColor.a = 0.30f;
        redColor.a = 0.30f;
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            if (r.gameObject.name == "IbofangTarget")
            {
                ibofangSR = r;
                r.enabled = false;
            }

        }

        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            if (r.gameObject.name == "LaneColor")
            {
                laneColor = r;
                r.enabled = false;
            }

        }
        BM = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    private void LateUpdate()
    {
        if (BM.playerTurn && turnChanging)
        {
            turnChanging = false;
            TargetedByIbofang = false;
            ibofangSR.enabled = false;
        }
        if (LaneChosen)
        {
            if (doOnce)
            {
                laneColor.enabled = true;
                laneColor.color = redColor;
                doOnce = false;
            }
        }
        else if(!BM.SelectingMove)
        {
            if (!doOnce)
            {
                laneColor.enabled = false;
                doOnce = true;
            }
        }

        if (tempTargetByIbofang || TargetedByIbofang)
        {
            ibofangSR.enabled = true;
        }
        else
        {
            ibofangSR.enabled = false;
        }

        if (!BM.playerTurn)
        {
            LaneChosen = false;
            turnChanging = true;
        }

        if (BM.SelectingMove)
        {
            if (BM.StaminaCostMovement((BM.SelectedCharacter.GetComponent<Character>().LanePos - LanePos), BM.SelectedCharacter) <= BM.SelectedCharacter.GetComponent<Character>().AvailableStamina)
            {

                if (!LaneChosen && !doOnceLaneChoose && BM.SelectedCharacter.GetComponent<Character>().LanePos != LanePos)
                {
                    laneColor.enabled = true;
                    doOnceLaneChoose = true;
                    laneColor.color = greenColor;
                }
            }
        }
        else
        {
            if (doOnceLaneChoose)
            {
                if (!LaneChosen)
                {
                    laneColor.enabled = false;
                }
                doOnceLaneChoose = false;

            }
        }
    }


}
