﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickingScript : MonoBehaviour {


    //private Transform clickedObject;
    public bool ObjectClicked;
    //public GameObject Marker;

    private BattleManager BM;
    private CombatUIScript UI;

    public GameObject TempUnitHolder;
    public GameObject TempEnemyUnitHolder;
    public bool CharacterClicked;
    public bool EnemyCharacterClicked;
    private AudioSource AS;
    public AudioClip ClickSound;

    //Doubleclick
    //private bool oneClick;
    //private bool doubleClick;
    //private float timerForDoubleClick;
    //private float delay = 0.25f;

    private void Start()
    {
        BM = GetComponent<BattleManager>();
        UI = GameObject.Find("CombatWheelHolder").GetComponent<CombatUIScript>();
        AS = GameObject.Find("GameManagerAudio").GetComponent<AudioSource>();

        //Marker.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetButtonDown("Fire1") && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && BM.playerTurn)
        {
            
            if (Physics.Raycast(ray, out hit, 1000))
            {

                if (hit.collider.tag == "Lane" && CharacterClicked && !BM.SelectingMove && BM.ChoosingSkill == "IbofangSkill")
                {
                    AS.PlayOneShot(ClickSound);
                    if (BM.IbofangTarget1 == null)
                    {
                        BM.IbofangTarget1 = hit.collider.gameObject;
                        BM.IbofangTarget1.GetComponent<LaneInfo>().tempTargetByIbofang = true;
                    }
                    else if (BM.IbofangTarget2 == null)
                    {
                        BM.IbofangTarget2 = hit.collider.gameObject;
                        BM.IbofangTarget2.GetComponent<LaneInfo>().tempTargetByIbofang = true;
                    }
                    else if (BM.IbofangTarget3 == null)
                    {
                        BM.IbofangTarget3 = hit.collider.gameObject;
                        BM.IbofangTarget3.GetComponent<LaneInfo>().tempTargetByIbofang = true;
                    }

                    if (BM.IbofangTarget1 != null && BM.IbofangTarget2 != null && BM.IbofangTarget3 != null)
                    {
                        ObjectClicked = false;
                        BM.SelectingAttack = false;
                        BM.InfoText.text = "";
                        BM.SelectedCharacter.GetComponent<Character>().Attacking = true;
                        BM.AddSkill(BM.ChoosingSkill);
                        ResetSelection();
                    }

                }

                else if (hit.collider.tag == "Lane" && CharacterClicked && BM.SelectingMove)
                {
                    AS.PlayOneShot(ClickSound);
                    BM.SuccesfulAttack = false;
                    ObjectClicked = true;
                    EnemyCharacterClicked = false;
                    if (TempEnemyUnitHolder != null)
                    {
                        TempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }
                    BM.SelectedLanePos = hit.collider.GetComponent<LaneInfo>().LanePos;
                    BM.AddMove();
                    if (!BM.SuccesfulAttack)
                    {
                        return;
                    }
                    ResetSelection();
                    BM.InfoText.text = "";
                }

                else if (hit.collider.tag == "Lane" && !BM.SelectingMove)
                {
                    if (hit.collider.GetComponent<LaneInfo>().LaneChosen)
                    {
                        AS.PlayOneShot(ClickSound);
                        BM.SelectedCharacter = hit.collider.GetComponent<LaneInfo>().UnitOnLane;
                        BM.ChooseMove();
                        ResetSelection();
                    }

                }

                else if (hit.collider.tag == "Player")
                {
                    AS.PlayOneShot(ClickSound);
                    ResetSelection();
                    ObjectClicked = false;
                    EnemyCharacterClicked = false;

                    if (TempEnemyUnitHolder != null)
                    {
                        TempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }

                    if (TempUnitHolder != null)
                    {
                        TempUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }

                    CharacterClicked = true;
                    TempUnitHolder = hit.collider.gameObject;
                    //Debug.Log("Character clicked");
                    hit.collider.GetComponent<Character>().CharacterClick();
                }

                else if (hit.collider.tag == "Enemy" && CharacterClicked && BM.SelectingAttack)
                {
                    if (BM.ChoosingSkill == "IbofangSkill")
                    {
                        return;
                    }
                    ObjectClicked = false;
                    BM.SelectingAttack = false;

                    if (TempEnemyUnitHolder != null)
                    {
                        TempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }
                    AS.PlayOneShot(ClickSound);
                    EnemyCharacterClicked = true;
                    TempEnemyUnitHolder = hit.collider.gameObject;
                    hit.collider.GetComponent<Character>().CharacterClick();
                    BM.InfoText.text = "";
                    BM.SelectedCharacter.GetComponent<Character>().Attacking = true;
                    if (BM.ChoosingSkill.Length > 0)
                    {
                        BM.AddSkill(BM.ChoosingSkill);
                    }
                    else
                    {
                        BM.AddAttack();
                    }

                    ResetSelection();
                }
                else
                {
                    ResetSelection();
                }


            }

            else
            {
                ResetSelection();
                //Debug.Log("Nothing Clicked ");
            }
        }

    }

    public void ResetSelection()
    {

        if (BM.SelectedCharacter != null)
        {
            foreach (SpriteRenderer r in BM.SelectedCharacter.GetComponentsInChildren<SpriteRenderer>())
            {
                if (r.gameObject.name == "Target")
                {
                    r.enabled = false;
                }
            }

        }
        ObjectClicked = false;
        BM.SelectingAttack = false;
        BM.SelectingMove = false;
        BM.SuccesfulAttack = false;
        CharacterClicked = false;
        EnemyCharacterClicked = false;
        BM.InfoText.text = "";
        UI.SelectAttack = false;
        UI.SelectSkill = false;
        BM.SelectedCharacter = null;

        foreach (GameObject r in BM.Lanes)
        {
            if (r.GetComponentInChildren<SpriteRenderer>().gameObject.name == "LaneColor")
            {
                if (!r.GetComponent<LaneInfo>().LaneChosen)
                {
                        r.GetComponentInChildren<SpriteRenderer>().enabled = false;
                }
            }

        }

        if (BM.IbofangTarget1 != null)
        {
            BM.IbofangTarget1.GetComponent<LaneInfo>().tempTargetByIbofang = false;
            BM.IbofangTarget1 = null;
        }
        if (BM.IbofangTarget2 != null)
        {
            BM.IbofangTarget2.GetComponent<LaneInfo>().tempTargetByIbofang = false;
            BM.IbofangTarget2 = null;
        }
        if (BM.IbofangTarget3 != null)
        {
            BM.IbofangTarget3.GetComponent<LaneInfo>().tempTargetByIbofang = false;
            BM.IbofangTarget3 = null;
        }

        if (TempUnitHolder != null)
        {
            TempUnitHolder.GetComponent<Character>().UnitChosen = false;
        }

        if (TempEnemyUnitHolder != null)
        {
            TempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
        }
    }


    //void CheckDoubleClick()
    //{
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        if (!oneClick)
    //        {
    //            oneClick = true;
    //            timerForDoubleClick = Time.time;
    //        }
    //        else
    //        {
    //            oneClick = false;
    //            doubleClick = true;
    //        }
    //    }

    //    if (oneClick)
    //    {
    //        if ((Time.time - timerForDoubleClick) > delay)
    //        {
    //            oneClick = false;
    //            doubleClick = false;
    //        }
    //    }
    //}
}
