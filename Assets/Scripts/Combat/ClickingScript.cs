using System.Collections;
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

    //Doubleclick
    //private bool oneClick;
    //private bool doubleClick;
    //private float timerForDoubleClick;
    //private float delay = 0.25f;

    private void Start()
    {
        BM = GetComponent<BattleManager>();
        UI = GameObject.Find("CombatButtons").GetComponent<CombatUIScript>();

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

                if (hit.collider.tag == "Lane" && CharacterClicked && BM.SelectingMove)
                {
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

                else if (hit.collider.tag == "Player")
                {
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
                    ObjectClicked = false;
                    //Marker.SetActive(ObjectClicked);
                    BM.SelectingAttack = false;

                    if (TempEnemyUnitHolder != null)
                    {
                        TempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }

                    EnemyCharacterClicked = true;
                    TempEnemyUnitHolder = hit.collider.gameObject;
                    //Debug.Log("Enemy clicked");
                    hit.collider.GetComponent<Character>().CharacterClick();
                    if (BM.ChoosingSkill.Length > 0)
                    {
                        BM.AddSkill(BM.ChoosingSkill);
                    }
                    else
                    {
                        BM.AddAttack();
                    }

                    BM.InfoText.text = "";
                    //BM.SelectedCharacter.GetComponent<Character>().ActionPoints -= 2;
                    BM.SelectedCharacter.GetComponent<Character>().Attacking = true;
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
        ObjectClicked = false;
        BM.SelectingAttack = false;
        BM.SelectingMove = false;
        BM.SuccesfulAttack = false;
        CharacterClicked = false;
        EnemyCharacterClicked = false;
        BM.InfoText.text = "";
        UI.SelectAttack = false;
        UI.SelectSkill = false;
        //Marker.SetActive(ObjectClicked);

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
