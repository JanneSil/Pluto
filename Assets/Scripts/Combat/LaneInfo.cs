﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneInfo : MonoBehaviour {

    public int LanePos;
    public bool LaneChosen;
    public GameObject UnitOnLane;
    public bool TargetedByIbofang;

    private SpriteRenderer sr;
    private BattleManager BM;

    private Color color = new Color(255F, 255F, 255F, 0.72F);

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        BM = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    private void LateUpdate()
    {
        if (LaneChosen)
        {
            sr.color = Color.red;
        }
        else if(!BM.SelectingMove)
        {
            sr.color = Color.white;
        }

        if (!BM.playerTurn)
        {
            LaneChosen = false;
        }

        if (BM.SelectingMove)
        {
            if (BM.StaminaCostMovement((BM.SelectedCharacter.GetComponent<Character>().LanePos - LanePos), BM.SelectedCharacter) <= BM.SelectedCharacter.GetComponent<Character>().AvailableStamina)
            {
                if (!LaneChosen)
                {
                    sr.color = color;
                }
            }
        }
    }


}
