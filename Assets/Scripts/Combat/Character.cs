﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anima2D;

public class Character : MonoBehaviour
{
    private BattleManager BM;
    private SpriteRenderer sprite;

    private GameObject staminaBar;
    private GameObject healthBar;

    private Text healthText;
    private Text staminaText;

    public bool UnitChosen;
    public bool IsTanking;

    public string Class;

    [Header("Points: ")]
    public int ActionPoints = 4;
    public int StrengthPoints = 100;
    public int StaminaPoints = 100;

    [Header("Status: ")]
    public bool Alive;
    public bool Attacking;
    public bool Defending;
    public bool Moving;
    public bool Resting;
    public bool UsingSkill;
    public bool Targeted;
    public bool SwitchingPlaces;
    public string SkillBeingUsed;

    [Header("Stats:")]
    [Range(1, 100)]
    public int Strength;
    [Range(1, 100)]
    public int Stamina;
    [Range(1, 100)]
    public int Speed;
    [Range(1, 100)]
    public int Dexterity;

    //Damage Dealing stats
    private float damageOutput;
    private float damageToStamina;
    private float damageToStrength;
    private float defendedBySpeed;
    private float defendedByStamina;
    private float defendedTotalAmount;
    private float distanceReduction;
    private float staminaOverkill;
    private float strengthPortion;

    //Movement variables
    private float moveMargin;
    private float movePause;
    private float moveSpeed;
    private Vector3 moveStartPos;
    private Vector3 moveTargetPos;

    //Turn saved stats
    public bool Player;

    public int AvailableStamina;
    public int DefendingStamina;
    public int LanePos;

    private int staminaPointsMax;
    private int strengthPointsMax;
    private bool criticalHit;
    private Component[] components;

    public GameObject gameObjectToMove;
    public Vector3 gameObjectOriginalPosition;
    public Vector3 gameObjectTargetPosition;
    [HideInInspector]
    public float speed;
    public bool HasToMove;
    Camera cam;
    private bool doOnce;

    //Unity functions
    void Start()
    {
        BM = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            if (r.gameObject.name == "Target")
            {
                r.enabled = false;
            }
        }
        if (Player)
        {
            gameObject.transform.Find("Attack").gameObject.SetActive(false);
            transform.Find("Normal").GetComponent<Animator>().SetFloat("Offset", Random.Range(0.0f, 1.0f));

        }
        else
        {
            GetComponent<Animator>().SetFloat("Offset", Random.Range(0.0f, 1.0f));
        }
        SortLayers(gameObject, LanePos);

        //Debug
        Alive = true;
        strengthPointsMax = Strength;
        StrengthPoints = strengthPointsMax;
        staminaPointsMax = Stamina;
        StaminaPoints = staminaPointsMax;

        strengthPointsMax = StrengthPoints;
        staminaPointsMax = StaminaPoints;

        AvailableStamina = StaminaPoints;

        GetStatusBars();
    }
    void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(gameObject.transform.position);

        //Clamp stremgth and stamina points
        if (StrengthPoints > strengthPointsMax)
        {
            StrengthPoints = strengthPointsMax;
        }
        if (StrengthPoints < 0)
        {
            StrengthPoints = 0;
        }
        if (StaminaPoints > staminaPointsMax)
        {
            StaminaPoints = staminaPointsMax;
        }
        if (StaminaPoints < 0)
        {
            StaminaPoints = 0;
        }

        //if (!UnitChosen && Player)
        //{
        //    foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        //        r.enabled = false;
        //    //sprite.enabled = false;
        //}
        //else if (!UnitChosen && !Player)
        //{
        //    //sprite.color = Color.red;
        //}

        if (BM.SelectingAttack && !Player && doOnce)
        {
            doOnce = false;
            //sprite.enabled = false;
        }
        if (!BM.SelectingAttack && !Player && !doOnce)
        {
            foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            {
                if (r.gameObject.name == "Target")
                {
                    r.enabled = false;
                }
            }
            doOnce = true;
            //sprite.color = Color.red;
        }

        //Update UI elements
        if (healthBar != null)
        {
            healthBar.GetComponent<Slider>().value = ((float)StrengthPoints / (float)strengthPointsMax);
            healthBar.transform.position = new Vector3(healthBar.transform.position.x, screenPos.y + 180, healthBar.transform.position.z);
        }
        if (staminaBar != null)
        {
            staminaBar.GetComponent<Slider>().value = ((float)StaminaPoints / (float)staminaPointsMax);
            staminaBar.transform.position = new Vector3(staminaBar.transform.position.x, screenPos.y + 180, staminaBar.transform.position.z);
        }
        if (healthText != null)
        {
            //healthText.text = StrengthPoints + "/" + strengthPointsMax;
            healthText.text = "";
        }
        if (staminaText != null)
        {
            //staminaText.text = StaminaPoints + "/" + staminaPointsMax;
            staminaText.text = "";
        }

        //Death, consider moving to damaging method
        if (StrengthPoints <= 0)
        {
            Die();
        }

        if (BM.actionTurn)
        {
            moveUpdate();
        }
    }
    private void LateUpdate()
    {
        if (HasToMove && transform.position.y != gameObjectTargetPosition.y)
        {
            gameObjectToMove.transform.position = Vector3.MoveTowards(transform.position, gameObjectTargetPosition, speed * Time.deltaTime);
        }
        else
        {
            HasToMove = false;
            gameObjectToMove = null;
        }
    }

    private void Die()
    {
        Alive = false;

        healthBar.SetActive(false);
        staminaBar.SetActive(false);
        if (Player)
        {
            BM.PlayerLanes[LanePos] = null;
        }
        else
        {
            BM.EnemyLanes[LanePos] = null;
        }

        Destroy(gameObject);
    }
    private void GetStatusBars()
    {
        if (Player)
        {
            healthBar = GameObject.Find("Canvas/LifeBars/PlayerHealthBar" + LanePos);
            staminaBar = GameObject.Find("Canvas/StaminaBars/PlayerStaminaBar" + LanePos);
            healthText = GameObject.Find("Canvas/LifeBars/PlayerHealthBar" + LanePos + "/HealthText").GetComponent<Text>();
            staminaText = GameObject.Find("Canvas/StaminaBars/PlayerStaminaBar" + LanePos + "/StaminaText").GetComponent<Text>();
            healthBar.SetActive(true);
            staminaBar.SetActive(true);
        }
        else
        {
            healthBar = GameObject.Find("Canvas/LifeBars/EnemyHealthBar" + LanePos);
            staminaBar = GameObject.Find("Canvas/StaminaBars/EnemyStaminaBar" + LanePos);
            healthText = GameObject.Find("Canvas/LifeBars/EnemyHealthBar" + LanePos + "/HealthText").GetComponent<Text>();
            staminaText = GameObject.Find("Canvas/StaminaBars/EnemyStaminaBar" + LanePos + "/StaminaText").GetComponent<Text>();
            healthBar.SetActive(true);
            staminaBar.SetActive(true);
        }
    }

    public void CharacterClick()
    {
        BM.ChooseCharacter(LanePos, Player, IsTanking);
        UnitChosen = true;
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.enabled = true;
            if (Player)
            {
                r.color = Color.green;
            }
            
        }
        //sprite.color = Color.green;
    }

    //Damage calculation
    private void CalculateDamage(GameObject target)
    {

        //Attacker damgage output = (Attacker strenght / 2) * (1 - Distance reduction * (1 - Attacker speed / 100))
        //Distance factor = 100% if same lanes, 50% if 1 lane away, 25% if 2 or 3 lane away, 12.5% if 4 or 5 lanes away

        //Distance factor
        if (Mathf.Abs(target.GetComponent<Character>().LanePos - LanePos) == 0)
        {
            //Distance between lanes is 0
            distanceReduction = 0;
        }
        else if (Mathf.Abs(target.GetComponent<Character>().LanePos - LanePos) == 1)
        {
            //Distance between lanes is 1
            distanceReduction = 0.5f;
        }
        else if (Mathf.Abs(target.GetComponent<Character>().LanePos - LanePos) == 2 || Mathf.Abs(target.GetComponent<Character>().LanePos - LanePos) == 3)
        {
            //Distance between lanes is 2 or 3
            distanceReduction = 0.75f;
        }
        else
        {
            //Distance between lanes is 4 or larger
            distanceReduction = 0.875f;
        }

        //Attacker damage output
        damageOutput = ((float)StrengthPoints / 2) * (1 - distanceReduction * (1 - (float)Speed / 100));

        //Factoring critical hit
        //Attack deals double the damage by a random factor, critical chance-% = Dexterity / 100
        if (Dexterity > 50)
        {
            if (Random.Range(100, 200) <= Dexterity * 2)
            {
                damageOutput = damageOutput * 3;
                criticalHit = true;
                Debug.Log("2x CRITICAL HIT PERFORMED BY: " + gameObject);
            }
            else
            {
                damageOutput = damageOutput * 2;
            }
        }
        else if (Random.Range(0, 100) <= Dexterity * 2)
        {
            damageOutput = damageOutput * 2;

            criticalHit = true;

            Debug.Log("CRITICAL HIT PERFORMED BY: " + gameObject);
        }

        //Damage is dealt to strength and stamina points by a random factor affected by attackers dexterity
        //Damage to strength = Damage output * Random number between 0 and 25 * Attacker Dexterity / 100
        //Rest from damage output is dealt to stamina
        strengthPortion = (Random.Range(0, 25) + Dexterity) / 100f;
        if (strengthPortion > 1)
        {
            strengthPortion = 1;
        }
        damageToStrength = damageOutput * strengthPortion;
        damageToStamina = damageOutput * (1 - strengthPortion);

        //Damage portion is offset more if target is defending or resting
        if (target.GetComponent<Character>().Defending)
        {
            //Damage to strength is offset by the amount that the character has invested in defending (10 at max) and the invensted points are reduced by strength damage avoided 
            //Damage defended = Defending stamina + (random number between 1 and defenders speed)/10
            defendedByStamina = target.GetComponent<Character>().DefendingStamina;
            defendedBySpeed = (Random.Range(1, target.GetComponent<Character>().Speed)) / 10;
            defendedTotalAmount = defendedByStamina + defendedBySpeed;

            //If defending staminapoints remain
            if (defendedTotalAmount >= damageToStrength)
            {
                //Remaining defending stamina points are reduced and damage to strength is dealt to stamina points instead
                target.GetComponent<Character>().DefendingStamina -= (int)(damageToStrength);
                damageToStamina += defendedByStamina;
                damageToStrength = 0;
            }
            //If defending staminapoints deplete
            else
            {
                //Defending staminapoints equal 0 and only the amount that was remaining is dealt to stamina points instead of strength
                target.GetComponent<Character>().DefendingStamina = 0;
                damageToStamina += defendedByStamina;
                damageToStrength -= defendedTotalAmount;
            }
        }
        else if (target.GetComponent<Character>().Resting)
        {
            defendedBySpeed = (Random.Range(1, target.GetComponent<Character>().Speed)) / 10;

            damageToStrength -= defendedBySpeed;
            damageToStamina += defendedBySpeed;
        }
        if (IsTanking) //The attacker is Tanking so damage dealt is halved
        {
            damageToStrength /= 2;
            damageToStrength /= 2;
        }
        //Attacking damage can't be negative
        if (damageToStrength < 0) damageToStrength = 0;
        if (damageToStamina < 0) damageToStamina = 0;
    }

    private void DealDamage(GameObject target, int damageMultiplier)
    {

        if (!target.GetComponent<Character>().IsTanking)
        {
            target.GetComponent<Character>().StrengthPoints -= (int)(damageToStrength * damageMultiplier);
        }

        if ((float)target.GetComponent<Character>().StaminaPoints - (damageToStamina * damageMultiplier) >= 0)
        {
            target.GetComponent<Character>().StaminaPoints -= (int)(damageToStamina * damageMultiplier);

            if (target.GetComponent<Character>().IsTanking)
            {
                BM.InstantiateDamageNumber((int)0, (int)damageToStamina * damageMultiplier, target.transform, criticalHit);
            }
            else
            {
                BM.InstantiateDamageNumber((int)damageToStrength * damageMultiplier, (int)damageToStamina * damageMultiplier, target.transform, criticalHit);
            }
        }
        else
        {
            staminaOverkill = -(target.GetComponent<Character>().StaminaPoints - (damageToStamina * damageMultiplier));

            target.GetComponent<Character>().StaminaPoints = 0;
            target.GetComponent<Character>().StrengthPoints -= (int)staminaOverkill;

            if (target.GetComponent<Character>().IsTanking)
            {
                BM.InstantiateDamageNumber((int)(staminaOverkill), (int)(Mathf.Abs((damageToStamina * damageMultiplier) - staminaOverkill)), target.transform, criticalHit);
            }
            else
            {
                BM.InstantiateDamageNumber((int)((damageToStrength * damageMultiplier) + staminaOverkill), (int)(Mathf.Abs((damageToStamina * damageMultiplier) - staminaOverkill)), target.transform, criticalHit);
            }

        }

        criticalHit = false;
    }

    //Character Functions
    public void SwitchPlaces(int startIndex, int targetIndex)
    {
        //Switches two character game objects index and position.
        if (Player)
        {
            if (Class == "Tank" && IsTanking && !SwitchingPlaces) //Moving in the tanking lanes
            {
                if (BM.PlayerLanes[targetIndex] != null && BM.PlayerTankLanes[targetIndex] == null)
                {
                    BM.PlayerTankLanes[targetIndex] = BM.PlayerTankLanes[startIndex];
                    LanePos = targetIndex;
                    IsTanking = true;
                    BM.PlayerTankLanes[startIndex] = null;

                    BM.PlayerTankLanes[targetIndex].transform.position = BM.PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                }
                else if (BM.PlayerLanes[targetIndex] != null && BM.PlayerTankLanes[targetIndex] != null)
                {
                    GameObject startGO = BM.PlayerTankLanes[startIndex];
                    GameObject targetGO = BM.PlayerTankLanes[targetIndex];

                    BM.PlayerTankLanes[targetIndex] = startGO;
                    LanePos = targetIndex;
                    BM.PlayerTankLanes[startIndex] = targetGO;
                    BM.PlayerTankLanes[startIndex].GetComponent<Character>().LanePos = startIndex;
                    BM.PlayerTankLanes[startIndex].transform.position = BM.PlayerLanePos[startIndex] + new Vector3(1.5f, 0, 0);
                    BM.PlayerTankLanes[targetIndex].transform.position = BM.PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);

                    SortLayers(BM.PlayerLanes[targetIndex], targetIndex);
                    SortLayers(BM.PlayerLanes[startIndex], startIndex);
                }
                else
                {
                    BM.PlayerLanes[targetIndex] = BM.PlayerTankLanes[startIndex];
                    BM.PlayerTankLanes[startIndex] = null;
                    LanePos = targetIndex;
                    IsTanking = false;

                    gameObjectTargetPosition = BM.PlayerLanePos[targetIndex];
                    gameObjectToMove = BM.PlayerLanes[targetIndex];
                    HasToMove = true;

                    //BM.PlayerLanes[targetIndex].transform.position = BM.PlayerLanePos[targetIndex];
                    SortLayers(BM.PlayerLanes[targetIndex], targetIndex);
                }
            }
            else if (Class == "Tank" && !IsTanking && !SwitchingPlaces)
            {

                if (BM.PlayerLanes[targetIndex] != null && BM.PlayerTankLanes[targetIndex] == null)
                {
                    BM.PlayerTankLanes[targetIndex] = BM.PlayerLanes[startIndex];
                    LanePos = targetIndex;
                    IsTanking = true;
                    BM.PlayerLanes[startIndex] = null;

                    gameObjectOriginalPosition = BM.PlayerLanePos[startIndex];
                    gameObjectTargetPosition = BM.PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                    gameObjectToMove = BM.PlayerTankLanes[targetIndex];
                    HasToMove = true;

                    //BM.PlayerTankLanes[targetIndex].transform.position = BM.PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                }
                else if (BM.PlayerLanes[targetIndex] != null && BM.PlayerTankLanes[targetIndex] != null)
                {
                    GameObject startGO = BM.PlayerLanes[startIndex];
                    GameObject targetGO = BM.PlayerTankLanes[targetIndex];

                    BM.PlayerTankLanes[targetIndex] = startGO;
                    LanePos = targetIndex;
                    IsTanking = true;
                    BM.PlayerLanes[startIndex] = targetGO;
                    BM.PlayerLanes[startIndex].GetComponent<Character>().LanePos = startIndex;
                    BM.PlayerLanes[startIndex].GetComponent<Character>().IsTanking = false;

                    BM.PlayerLanes[startIndex].transform.position = BM.PlayerLanePos[startIndex];
                    BM.PlayerTankLanes[targetIndex].transform.position = BM.PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                }
                else
                {
                    BM.PlayerLanes[targetIndex] = BM.PlayerLanes[startIndex];
                    BM.PlayerLanes[startIndex] = null;
                    LanePos = targetIndex;
                    IsTanking = false;

                    gameObjectOriginalPosition = BM.PlayerLanePos[startIndex];
                    gameObjectTargetPosition = BM.PlayerLanePos[targetIndex];
                    gameObjectToMove = BM.PlayerLanes[targetIndex];
                    HasToMove = true;

                    SortLayers(BM.PlayerLanes[targetIndex], targetIndex);
                }

            }
            else if (BM.PlayerLanes[targetIndex] != null)//If two character game objects need to switch places
            {
                GameObject startGO = BM.PlayerLanes[startIndex];
                GameObject targetGO = BM.PlayerLanes[targetIndex];

                BM.PlayerLanes[targetIndex] = startGO;
                LanePos = targetIndex;
                BM.PlayerLanes[startIndex] = targetGO;
                BM.PlayerLanes[startIndex].GetComponent<Character>().LanePos = startIndex;


                BM.PlayerLanes[startIndex].GetComponent<Character>().gameObjectTargetPosition = BM.PlayerLanePos[startIndex];
                BM.PlayerLanes[startIndex].GetComponent<Character>().gameObjectToMove = BM.PlayerLanes[startIndex];
                BM.PlayerLanes[startIndex].GetComponent<Character>().HasToMove = true;

                gameObjectTargetPosition = BM.PlayerLanePos[targetIndex];
                gameObjectToMove = gameObject;
                HasToMove = true;
                //BM.PlayerLanes[startIndex].transform.position = BM.PlayerLanePos[startIndex];
                //BM.PlayerLanes[targetIndex].transform.position = BM.PlayerLanePos[targetIndex];

                SortLayers(BM.PlayerLanes[targetIndex], targetIndex);
                SortLayers(BM.PlayerLanes[startIndex], startIndex);

                //Debug
                targetGO.GetComponent<Character>().moveStartPos = BM.PlayerLanePos[targetGO.GetComponent<Character>().LanePos];
                targetGO.GetComponent<Character>().moveTargetPos = BM.PlayerLanePos[targetGO.GetComponent<Character>().LanePos];
            }

            else //If the character game object needs to switch to a empty index
            {
                BM.PlayerLanes[targetIndex] = BM.PlayerLanes[startIndex];
                LanePos = targetIndex;
                BM.PlayerLanes[startIndex] = null;

                gameObjectTargetPosition = BM.PlayerLanePos[targetIndex];
                gameObjectToMove = gameObject;
                HasToMove = true;

                //BM.PlayerLanes[targetIndex].transform.position = BM.PlayerLanePos[targetIndex];

                SortLayers(BM.PlayerLanes[targetIndex], targetIndex);

            }
            //Debug
            moveStartPos = BM.PlayerLanePos[targetIndex];
            moveTargetPos = BM.PlayerLanePos[targetIndex];
        }
        else
        {
            if (BM.EnemyLanes[targetIndex] != null)
            {
                return;
            }
            //if (BM.EnemyLanes[targetIndex] != null)
            //{
                //GameObject startGO = BM.EnemyLanes[startIndex];
                GameObject targetGO = BM.EnemyLanes[targetIndex];

            //    BM.EnemyLanes[targetIndex] = startGO;
            //    LanePos = targetIndex;
            //    BM.EnemyLanes[startIndex] = targetGO;
            //    BM.EnemyLanes[startIndex].GetComponent<Character>().LanePos = startIndex;


            //    BM.EnemyLanes[startIndex].transform.position = BM.EnemyLanePos[startIndex];
            //    BM.EnemyLanes[targetIndex].transform.position = BM.EnemyLanePos[targetIndex];
            //}

            //Debug
            //targetGO.GetComponent<Character>().moveStartPos = BM.EnemyLanePos[targetGO.GetComponent<Character>().LanePos];
            //targetGO.GetComponent<Character>().moveTargetPos = BM.EnemyLanePos[targetGO.GetComponent<Character>().LanePos];            

            //else
            //{

            BM.EnemyLanes[targetIndex] = BM.EnemyLanes[startIndex];
            LanePos = targetIndex;
            BM.EnemyLanes[startIndex] = null;

            gameObjectTargetPosition = BM.EnemyLanePos[targetIndex];
            gameObjectToMove = gameObject;
            HasToMove = true;

            //BM.EnemyLanes[targetIndex].transform.position = BM.EnemyLanePos[targetIndex];

            SortLayers(BM.EnemyLanes[targetIndex], targetIndex);
            //}
            //Debug
            moveStartPos = BM.EnemyLanePos[targetIndex];
            moveTargetPos = BM.EnemyLanePos[targetIndex];
        }
    }
    public void Attack(GameObject target)
    {
        CalculateDamage(target);

        //Damage dealt accordingly, if stamina goes negative remaining damage is dealt to strength instead, also instatiates damgage number
        //}
        if (BM.PlayerTankLanes[target.GetComponent<Character>().LanePos] != null && !Player) //Tank is Protecting the Target and is hit instead
        {
            DealDamage(BM.PlayerTankLanes[target.GetComponent<Character>().LanePos], 1);
            return;
        }
        else
        {
            //Normal Damage
            DealDamage(target, 1);
        }

    }
    public void PerformSkill(GameObject agent, string Skill)
    {
        if (Skill == "TankSkill")
        {
            if (BM.EnemyLanes[agent.GetComponent<Character>().LanePos] != null)
            {
                CalculateDamage(BM.EnemyLanes[agent.GetComponent<Character>().LanePos]);
                DealDamage(BM.EnemyLanes[agent.GetComponent<Character>().LanePos], 2);
            }

            if (agent.GetComponent<Character>().LanePos - 1 > -1)
            {
                if (BM.EnemyLanes[agent.GetComponent<Character>().LanePos - 1] != null)
                {
                    CalculateDamage(BM.EnemyLanes[agent.GetComponent<Character>().LanePos - 1]);
                    DealDamage(BM.EnemyLanes[agent.GetComponent<Character>().LanePos - 1], 1);
                }
            }

            if (agent.GetComponent<Character>().LanePos + 1 < 6)
            {
                if (BM.EnemyLanes[agent.GetComponent<Character>().LanePos + 1] != null)
                {
                    CalculateDamage(BM.EnemyLanes[agent.GetComponent<Character>().LanePos + 1]);
                    DealDamage(BM.EnemyLanes[agent.GetComponent<Character>().LanePos + 1], 1);
                }
            }

        }

        else if (Skill == "")
        {
            return;
        }
    }

    public void SetMove(Vector3 targetPos, float pause, float speed, float margin)
    {
        movePause = pause;
        moveTargetPos = targetPos;
        if (Player)
        {
            moveStartPos = BM.PlayerLanePos[LanePos];
        }
        else
        {
            moveStartPos = BM.EnemyLanePos[LanePos];
        }
        moveMargin = margin;
        moveSpeed = speed;
    }
    public void SortLayers(GameObject agent, int targetIndex)
    {
        components = agent.GetComponentsInChildren<SpriteMeshInstance>();

        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.sortingLayerName = "Lane" + targetIndex;
        }

        foreach (SpriteMeshInstance spritemesh in components)
        {
            spritemesh.sortingLayerName = "Lane" + targetIndex;
        }
    }

    private void moveUpdate()
    {
        Vector3 prevPos;
        Vector3 stepPos;

        Vector2 compareAgent = new Vector2(transform.position.x, transform.position.y);
        Vector2 compareTarget = new Vector2(moveTargetPos.x, moveTargetPos.y);

        //This is to remove overhead from Update(), position is not affected if it falls into the margin
        if ((compareTarget - compareAgent).magnitude > moveMargin)
        {
            prevPos = transform.position;

            //Interpolates a step between current and target. Adds step to current.
            stepPos = new Vector3((moveTargetPos.x - prevPos.x) * moveSpeed * Time.deltaTime, (moveTargetPos.y - prevPos.y) * moveSpeed * Time.deltaTime, 0);

            transform.position += stepPos;
        }
        else if ((compareTarget - compareAgent).magnitude <= moveMargin)
        {
            if (BM.TurnDelayRemaining <= 0.1)
            {
                if (Player)
                {
                    transform.Find("Normal").gameObject.SetActive(true);
                    transform.Find("Attack").gameObject.SetActive(false);
                }
                moveTargetPos = moveStartPos;
            }
            else
            {
                movePause -= Time.deltaTime;
            }
        }


    }
}
