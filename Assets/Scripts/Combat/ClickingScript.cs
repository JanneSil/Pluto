using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickingScript : MonoBehaviour {


    //private Transform clickedObject;
    public bool ObjectClicked;
    //public GameObject Marker;

    private BattleManager BM;
    private GameObject tempUnitHolder;
    private GameObject tempEnemyUnitHolder;
    public bool UnitClicked;
    public bool EnemyUnitClicked;

    //Doubleclick
    //private bool oneClick;
    //private bool doubleClick;
    //private float timerForDoubleClick;
    //private float delay = 0.25f;

    private void Start()
    {
        BM = GetComponent<BattleManager>();

        //Marker.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetButtonDown("Fire1") && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            
            if (Physics.Raycast(ray, out hit, 1000))
            {

                if (hit.collider.tag == "Lane" && UnitClicked && BM.SelectingMove)
                {
                    ObjectClicked = true;
                    EnemyUnitClicked = false;
                    BM.SelectingMove = false;
                    if (tempEnemyUnitHolder != null)
                    {
                        tempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }
                    BM.SelectedLanePos = hit.collider.GetComponent<LaneInfo>().LanePos;
                    BM.SelectedCharacter.GetComponent<Character>().ActionPoints -= 1;
                    BM.SelectedCharacter.GetComponent<Character>().Moving = true;
                    Debug.Log("Lane Pos: " + BM.SelectedLanePos);
                    BM.AddMove();
                    BM.InfoText.text = "";
                    ResetSelection();
                    //Marker.transform.position = hit.transform.position;
                    //Marker.SetActive(ObjectClicked);
                }

                else if (hit.collider.tag == "Player")
                {
                    ObjectClicked = false;
                    EnemyUnitClicked = false;
                    //Marker.SetActive(ObjectClicked);

                    if (tempEnemyUnitHolder != null)
                    {
                        tempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }

                    if (tempUnitHolder != null)
                    {
                        tempUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }

                    UnitClicked = true;
                    tempUnitHolder = hit.collider.gameObject;
                    Debug.Log("Character clicked");
                    hit.collider.GetComponent<Character>().CharacterClick();
                }

                else if (hit.collider.tag == "Enemy" && UnitClicked && BM.SelectingAttack)
                {
                    ObjectClicked = false;
                    //Marker.SetActive(ObjectClicked);
                    BM.SelectingAttack = false;

                    if (tempEnemyUnitHolder != null)
                    {
                        tempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }

                    EnemyUnitClicked = true;
                    tempEnemyUnitHolder = hit.collider.gameObject;
                    Debug.Log("Enemy clicked");
                    hit.collider.GetComponent<Character>().CharacterClick();
                    if (BM.ChoosingSkill.Length > 0)
                    {
                        BM.AddSkill();
                    }
                    else
                    {
                        BM.AddAttack();
                    }

                    BM.InfoText.text = "";
                    BM.SelectedCharacter.GetComponent<Character>().ActionPoints -= 2;
                    BM.SelectedCharacter.GetComponent<Character>().Attacking = true;
                    ResetSelection();
                }


            }

            else
            {
                ResetSelection();
                Debug.Log("Nothing Clicked ");
            }
        }

    }

    public void ResetSelection()
    {
        ObjectClicked = false;
        UnitClicked = false;
        EnemyUnitClicked = false;
        BM.InfoText.text = "";
        //Marker.SetActive(ObjectClicked);

        if (tempUnitHolder != null)
        {
            tempUnitHolder.GetComponent<Character>().UnitChosen = false;
        }

        if (tempEnemyUnitHolder != null)
        {
            tempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
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
