using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInfo : MonoBehaviour {

    private BattleManager BM;
    private GameObject hoverInfo;
    private bool doOnce;
    private bool doOnceSecond;
    private bool doOnceThird;
    private Text infoText;
    private GameObject tempGameObject;
    private GameObject tempGameObjectTwo;
    private GameObject tempGameObjectThree;
    private Color color;

    private int strength;
    private int stamina;
    private int speed;
    private int dexterity;
    public Vector3 offset;

    // Use this for initialization
    void Start () {
        BM = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        hoverInfo = GameObject.Find("HoverInfo");
        infoText = hoverInfo.GetComponentInChildren<Text>();
        color = Color.green;
        color.a = 0.5f;
        hoverInfo.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (BM.playerTurn)
        {

            if (Physics.Raycast(ray, out hit, 1000))
            {

                if (hit.collider.tag == "Enemy" && !BM.SelectingAttack)
                {
                    if (tempGameObject != null && tempGameObject != hit.collider.gameObject)
                    {
                        foreach (SpriteRenderer r in tempGameObject.GetComponentsInChildren<SpriteRenderer>())
                            r.enabled = false;
                        doOnce = true;
                    }
                    if (doOnce)
                    {
                        tempGameObject = hit.collider.gameObject;
                        GetInfo(hit.collider.gameObject);
                        hoverInfo.SetActive(true);
                        foreach (SpriteRenderer r in tempGameObject.GetComponentsInChildren<SpriteRenderer>())
                            r.enabled = true;
                    }
                    transform.position = Input.mousePosition + offset;
                    infoText.text = "Strength: " + strength + 
                        " Stamina: " + stamina +
                        " Speed: " + speed +
                        " Dexterity: " + dexterity;
                    doOnce = false;
                }
                else if (hit.collider.tag == "Enemy" && BM.SelectingAttack)
                {
                    if (tempGameObject != null && tempGameObject != hit.collider.gameObject)
                    {
                        foreach (SpriteRenderer r in tempGameObject.GetComponentsInChildren<SpriteRenderer>())
                            r.enabled = false;
                        doOnce = true;
                    }
                    if (doOnce)
                    {
                        tempGameObject = hit.collider.gameObject;
                        foreach (SpriteRenderer r in tempGameObject.GetComponentsInChildren<SpriteRenderer>())
                            r.enabled = true;
                    }
                    doOnce = false;
                }
                else if (doOnce == false)
                {
                    hoverInfo.SetActive(false);
                    if (tempGameObject != null)
                    {
                        foreach (SpriteRenderer r in tempGameObject.GetComponentsInChildren<SpriteRenderer>())
                            r.enabled = false;
                    }
                    doOnce = true;
                }

                if (hit.collider.tag == "Lane")
                {
                    if (hit.collider.gameObject.GetComponent<LaneInfo>().LaneChosen)
                    {
                        if (!doOnceSecond)
                        {
                            foreach (SpriteRenderer r in hit.collider.gameObject.GetComponent<LaneInfo>().UnitOnLane.GetComponentsInChildren<SpriteRenderer>())
                                r.enabled = true;
                            tempGameObjectTwo = hit.collider.gameObject;
                        }
                        doOnceSecond = true;
                    }
                    else if (doOnceSecond)
                    {
                        if (tempGameObjectTwo != hit.collider.gameObject)
                        {
                            foreach (SpriteRenderer r in tempGameObjectTwo.GetComponent<LaneInfo>().UnitOnLane.GetComponentsInChildren<SpriteRenderer>())
                                r.enabled = false;
                        }

                        doOnceSecond = false;
                    }
                }
                else if (doOnceSecond)
                {
                        if (tempGameObjectTwo != hit.collider.gameObject)
                        {
                            foreach (SpriteRenderer r in tempGameObjectTwo.GetComponent<LaneInfo>().UnitOnLane.GetComponentsInChildren<SpriteRenderer>())
                                r.enabled = false;
                        }

                        doOnceSecond = false;
                }

                if (hit.collider.tag == "Player")
                {
                    if (tempGameObjectThree != null && tempGameObjectThree != hit.collider.gameObject)
                    {
                        foreach (SpriteRenderer r in tempGameObjectThree.GetComponentsInChildren<SpriteRenderer>())
                        {
                            if (tempGameObjectThree != BM.SelectedCharacter)
                            {
                                r.enabled = false;
                            }

                        }
                        doOnceThird = true;
                    }
                    if (doOnceThird)
                    {
                        tempGameObjectThree = hit.collider.gameObject;
                        foreach (SpriteRenderer r in tempGameObjectThree.GetComponentsInChildren<SpriteRenderer>())
                        {
                            r.enabled = true;
                            if (tempGameObjectThree != BM.SelectedCharacter)
                            {
                                r.color = color;
                            }
                            
                        }
                    }
                    doOnceThird = false;
                }
                else if (doOnceThird == false && BM.SelectedCharacter != hit.collider.gameObject)
                {
                    if (tempGameObjectThree != null)
                    {
                        foreach (SpriteRenderer r in tempGameObjectThree.GetComponentsInChildren<SpriteRenderer>())
                        {
                            if (tempGameObjectThree != BM.SelectedCharacter)
                            {
                                r.enabled = false;
                            }

                        }
                    }
                    doOnceThird = true;
                    tempGameObjectThree = null;
                }

            }

        }
    }

    void GetInfo(GameObject target)
    {
        strength = target.GetComponent<Character>().StrengthPoints;
        stamina = target.GetComponent<Character>().StaminaPoints;
        speed = target.GetComponent<Character>().Speed;
        dexterity = target.GetComponent<Character>().Dexterity;

    }
}
