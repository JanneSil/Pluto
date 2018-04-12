using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private ClickingScript CS;

    private bool playerTurn;

    //Lane data arrays
    public GameObject[] EnemyLanes = new GameObject[6];
    public GameObject[] PlayerLanes = new GameObject[6];
    public GameObject[] EnemyTankLanes = new GameObject[6];
    public GameObject[] PlayerTankLanes = new GameObject[6];

    private Vector3[] EnemyLanePos = new Vector3[] { new Vector3(3.5f,  3.5f, 0),
                                                     new Vector3(3.5f,  2,    0),
                                                     new Vector3(3.5f,  0.5f, 0),
                                                     new Vector3(3.5f, -1,    0),
                                                     new Vector3(3.5f, -2.5f, 0),
                                                     new Vector3(3.5f, -4,    0) };
    private Vector3[] PlayerLanePos = new Vector3[] { new Vector3(-3.5f,  3.5f, 0),
                                                      new Vector3(-3.5f,  2,    0),
                                                      new Vector3(-3.5f,  0.5f, 0),
                                                      new Vector3(-3.5f, -1,    0),
                                                      new Vector3(-3.5f, -2.5f, 0),
                                                      new Vector3(-3.5f, -4,    0) };

    //Lists
    private List<Action> ActionList = new List<Action>();
    private List<Action> MovementList = new List<Action>();

    //Selection variables
    public bool ChoosingAttack;
    public string ChoosingSkill;
    public bool SelectingAttack;
    public bool SelectingMove;

    public GameObject SelectedCharacter;
    public GameObject SelectedEnemyCharacter;

    public int SelectedLanePos;

    public Text InfoText;

    //UI buttons
    private GameObject attackButton;
    private GameObject defendButton;
    private GameObject endTurnButton;
    private GameObject moveButton;
    private GameObject restButton;

    //UI other
    private GameObject canvas;
    private GameObject infoTextObject;

    //Unity functions
    void Start()
    {
        InitializeCombat();
        playerTurn = true;
    }
    void Update()
    {
        CheckCombatResult();
        displayActionButtons();
    }

    //Character functions, should consider moving these to Character.cs except for ChooseCharacter()
    private void Attack(GameObject attacker, GameObject target, string Skill)
    {
        float damageOutput;
        float damageToStamina;
        float damageToStrength;
        float defendedBySpeed;
        float defendedByStamina;
        float defendedTotalAmount;
        float distanceFactor;
        float staminaOverkill;
        float strengthPortion;

        //Attacker damgage output = (Attacker strenght / 2) * Distance factor * (1 + (Attacker speed / 100))
        //Distance factor = 100% if same lanes, 50% if 1 lane away, 25% if 2 or 3 lane away, 12.5% if 4 or 5 lanes away

        //Distance factor
        if (Mathf.Abs(target.GetComponent<Character>().LanePos - attacker.GetComponent<Character>().LanePos) == 0)
        {
            //Distance between lanes is 0
            distanceFactor = 1;
        }
        else if (Mathf.Abs(target.GetComponent<Character>().LanePos - attacker.GetComponent<Character>().LanePos) == 1)
        {
            //Distance between lanes is 1
            distanceFactor = 0.5f;
        }
        else if (Mathf.Abs(target.GetComponent<Character>().LanePos - attacker.GetComponent<Character>().LanePos) == 2 || Mathf.Abs(target.GetComponent<Character>().LanePos - attacker.GetComponent<Character>().LanePos) == 3)
        {
            //Distance between lanes is 2 or 3
            distanceFactor = 0.25f;
        }
        else
        {
            //Distance between lanes is 4 or larger
            distanceFactor = 0.125f;
        }

        //Attacker damage output
        damageOutput = ((float)attacker.GetComponent<Character>().StrengthPoints / 2) * distanceFactor * (1 + (attacker.GetComponent<Character>().Speed / 100));

        //Factoring critical hit
        //Attack deals double the damage by a random factor, critical chance-% = Dexterity / 100
        if (Random.Range(0, 100) < attacker.GetComponent<Character>().Dexterity)
        {
            damageOutput = damageOutput * 2;

            Debug.Log("CRITICAL HIT PERFORMED BY: " + attacker);
        }

        //Damage is dealt to strenght and stamina points by a random factor affected by attackers dexterity
        //Damage to strength = Damage output * Random number between 0 and 25 * Attacker Dexterity / 100
        //Rest from damage output is dealt to stamina
        strengthPortion = (Random.Range(0, 25) + attacker.GetComponent<Character>().Dexterity) / 100;
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
                damageToStamina += damageToStrength - defendedBySpeed;
                damageToStrength = 0;
            }
            //If defending staminapoints deplete
            else
            {
                //Defending staminapoints equal 0, only the amount that was remaining is dealt to stamina points instead of strength
                damageToStrength -= defendedTotalAmount;
                damageToStamina += defendedTotalAmount - defendedBySpeed;
                target.GetComponent<Character>().DefendingStamina = 0;
            }
        }
        else if (target.GetComponent<Character>().Resting)
        {
            defendedBySpeed = (Random.Range(1, target.GetComponent<Character>().Speed)) / 10;

            damageToStrength -= defendedBySpeed;
            damageToStamina += defendedBySpeed;
        }
        //Attacking damage can't be negative
        if (damageToStrength < 0) damageToStrength = 0;
        if (damageToStamina < 0) damageToStamina = 0;

        //Damage dealt accordingly, if stamina goes negative remaining damage is dealt to strength instead, also instatiates damgage number
        if (Skill == "TankSkill")
        {

            target.GetComponent<Character>().StrengthPoints -= (int)damageToStrength * 2;
            if ((float)target.GetComponent<Character>().StaminaPoints - damageToStamina * 2 >= 0)
            {
                target.GetComponent<Character>().StaminaPoints -= (int)damageToStamina * 2;

                InstantiateDamageNumber((int)damageToStrength * 2, (int)damageToStamina * 2, target.transform);
            }
            else
            {
                staminaOverkill = -(target.GetComponent<Character>().StaminaPoints - damageToStamina * 2);

                target.GetComponent<Character>().StaminaPoints = 0;
                target.GetComponent<Character>().StrengthPoints -= (int)staminaOverkill;

                InstantiateDamageNumber((int)(damageToStrength * 2 + staminaOverkill), (int)(Mathf.Abs(damageToStamina - staminaOverkill * 2)), target.transform);
            }

            if (target.GetComponent<Character>().LanePos != 0)
            {
                if (EnemyLanes[target.GetComponent<Character>().LanePos - 1] != null)
                {
                    target.GetComponent<Character>().StrengthPoints -= (int)damageToStrength;
                    if ((float)EnemyLanes[target.GetComponent<Character>().LanePos - 1].GetComponent<Character>().StaminaPoints - damageToStamina >= 0)
                    {
                        EnemyLanes[target.GetComponent<Character>().LanePos - 1].GetComponent<Character>().StaminaPoints -= (int)damageToStamina;

                        InstantiateDamageNumber((int)damageToStrength, (int)damageToStamina, EnemyLanes[target.GetComponent<Character>().LanePos - 1].GetComponent<Character>().transform);
                    }
                    else
                    {
                        staminaOverkill = -(EnemyLanes[target.GetComponent<Character>().LanePos - 1].GetComponent<Character>().StaminaPoints - damageToStamina);

                        EnemyLanes[target.GetComponent<Character>().LanePos - 1].GetComponent<Character>().StaminaPoints = 0;
                        EnemyLanes[target.GetComponent<Character>().LanePos - 1].GetComponent<Character>().StrengthPoints -= (int)staminaOverkill;

                        InstantiateDamageNumber((int)(damageToStrength + staminaOverkill), (int)(Mathf.Abs(damageToStamina - staminaOverkill)), EnemyLanes[target.GetComponent<Character>().LanePos - 1].transform);
                    }

                }
            }

            if (target.GetComponent<Character>().LanePos != 5)
            {
                if (EnemyLanes[target.GetComponent<Character>().LanePos + 1] != null)
                {
                    EnemyLanes[target.GetComponent<Character>().LanePos + 1].GetComponent<Character>().StrengthPoints -= (int)damageToStrength;
                    if ((float)EnemyLanes[target.GetComponent<Character>().LanePos + 1].GetComponent<Character>().StaminaPoints - damageToStamina >= 0)
                    {
                        EnemyLanes[target.GetComponent<Character>().LanePos + 1].GetComponent<Character>().StaminaPoints -= (int)damageToStamina;

                        InstantiateDamageNumber((int)damageToStrength, (int)damageToStamina, EnemyLanes[target.GetComponent<Character>().LanePos + 1].transform);
                    }
                    else
                    {
                        staminaOverkill = -(EnemyLanes[target.GetComponent<Character>().LanePos + 1].GetComponent<Character>().StaminaPoints - damageToStamina);

                        EnemyLanes[target.GetComponent<Character>().LanePos + 1].GetComponent<Character>().StaminaPoints = 0;
                        EnemyLanes[target.GetComponent<Character>().LanePos + 1].GetComponent<Character>().StrengthPoints -= (int)staminaOverkill;

                        InstantiateDamageNumber((int)(damageToStrength + staminaOverkill), (int)(Mathf.Abs(damageToStamina - staminaOverkill)), EnemyLanes[target.GetComponent<Character>().LanePos + 1].transform);
                    }
                }
            }



        }
        else if (PlayerTankLanes[target.GetComponent<Character>().LanePos] != null) //Tank is Protecting the Target and is hit instead
        {
            PlayerTankLanes[target.GetComponent<Character>().LanePos].GetComponent<Character>().StrengthPoints -= (int)damageToStrength;

            if ((float)PlayerTankLanes[target.GetComponent<Character>().LanePos].GetComponent<Character>().StaminaPoints - damageToStamina >= 0)
            {
                PlayerTankLanes[target.GetComponent<Character>().LanePos].GetComponent<Character>().StaminaPoints -= (int)damageToStamina;

                InstantiateDamageNumber((int)damageToStrength, (int)damageToStamina, PlayerTankLanes[target.GetComponent<Character>().LanePos].transform);
            }
            else
            {
                staminaOverkill = -(PlayerTankLanes[target.GetComponent<Character>().LanePos].GetComponent<Character>().StaminaPoints - damageToStamina);

                PlayerTankLanes[target.GetComponent<Character>().LanePos].GetComponent<Character>().StaminaPoints = 0;
                PlayerTankLanes[target.GetComponent<Character>().LanePos].GetComponent<Character>().StrengthPoints -= (int)staminaOverkill;

                InstantiateDamageNumber((int)(damageToStrength + staminaOverkill), (int)(Mathf.Abs(damageToStamina - staminaOverkill)), PlayerTankLanes[target.GetComponent<Character>().LanePos].transform);
            }

            return;
        }
        else if (attacker.GetComponent<Character>().IsTanking) //The attacker is Tanking so damage dealt is halved
        {
            target.GetComponent<Character>().StrengthPoints -= (int)(damageToStrength / 2);

            if ((float)target.GetComponent<Character>().StaminaPoints - (damageToStamina / 2) >= 0)
            {
                target.GetComponent<Character>().StaminaPoints -= (int)damageToStamina / 2;

                InstantiateDamageNumber((int)(damageToStrength / 2), (int)(damageToStamina / 2), target.transform);
            }
            else
            {
                staminaOverkill = -(target.GetComponent<Character>().StaminaPoints - damageToStamina);

                target.GetComponent<Character>().StaminaPoints = 0;
                target.GetComponent<Character>().StrengthPoints -= (int)staminaOverkill;

                InstantiateDamageNumber((int)(damageToStrength + staminaOverkill), (int)(Mathf.Abs(damageToStamina - staminaOverkill)), target.transform);
            }
            return;
        }
        else
        {
            //Normal Damage
            target.GetComponent<Character>().StrengthPoints -= (int)damageToStrength;

            if ((float)target.GetComponent<Character>().StaminaPoints - damageToStamina >= 0)
            {
                target.GetComponent<Character>().StaminaPoints -= (int)damageToStamina;

                InstantiateDamageNumber((int)damageToStrength, (int)damageToStamina, target.transform);
            }
            else
            {
                staminaOverkill = -(target.GetComponent<Character>().StaminaPoints - damageToStamina);

                target.GetComponent<Character>().StaminaPoints = 0;
                target.GetComponent<Character>().StrengthPoints -= (int)staminaOverkill;

                InstantiateDamageNumber((int)(damageToStrength + staminaOverkill), (int)(Mathf.Abs(damageToStamina - staminaOverkill)), target.transform);
            }
        }

    }
    public void ChooseCharacter(int laneIndex, bool player, bool Tanking)
    {
        if (player)
        {

            if (Tanking)
            {
                Debug.Log("Selected: " + PlayerTankLanes[laneIndex]);
                SelectedCharacter = PlayerTankLanes[laneIndex];
            }

            else
            {
                Debug.Log("Selected: " + PlayerLanes[laneIndex]);
                SelectedCharacter = PlayerLanes[laneIndex];
            }

        }
        else
        {
            Debug.Log("Selected: " + EnemyLanes[laneIndex]);
            SelectedEnemyCharacter = EnemyLanes[laneIndex];
        }

    }
    public void SwitchPlaces(int startIndex, int targetIndex, bool isPlayer, bool Tanking, string Class)
    {
        //Switches two character game objects index and position.
        if (isPlayer)
        {
            if (Class == "Tank" && Tanking) //Moving in the tanking lanes
            {
                if (PlayerLanes[targetIndex] != null && PlayerTankLanes[targetIndex] == null)
                {
                    PlayerTankLanes[targetIndex] = PlayerTankLanes[startIndex];
                    PlayerTankLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                    PlayerTankLanes[targetIndex].GetComponent<Character>().IsTanking = true;
                    PlayerTankLanes[startIndex] = null;

                    PlayerTankLanes[targetIndex].transform.position = PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                }
                else if (PlayerLanes[targetIndex] != null && PlayerTankLanes[targetIndex] != null)
                {
                    GameObject startGO = PlayerTankLanes[startIndex];
                    GameObject targetGO = PlayerTankLanes[targetIndex];

                    PlayerTankLanes[targetIndex] = startGO;
                    PlayerTankLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                    PlayerTankLanes[startIndex] = targetGO;
                    PlayerTankLanes[startIndex].GetComponent<Character>().LanePos = startIndex;


                    PlayerTankLanes[startIndex].transform.position = PlayerLanePos[startIndex] + new Vector3(1.5f, 0, 0);
                    PlayerTankLanes[targetIndex].transform.position = PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                }
                else
                {
                    PlayerLanes[targetIndex] = PlayerTankLanes[startIndex];
                    PlayerLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                    PlayerLanes[targetIndex].GetComponent<Character>().IsTanking = false;
                    PlayerTankLanes[startIndex] = null;

                    PlayerLanes[targetIndex].transform.position = PlayerLanePos[targetIndex];
                }
            }
            else if (Class == "Tank" && !Tanking)
            {

                if (PlayerLanes[targetIndex] != null && PlayerTankLanes[targetIndex] == null)
                {
                    PlayerTankLanes[targetIndex] = PlayerLanes[startIndex];
                    PlayerTankLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                    PlayerTankLanes[targetIndex].GetComponent<Character>().IsTanking = true;
                    PlayerLanes[startIndex] = null;

                    PlayerTankLanes[targetIndex].transform.position = PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                }
                else if (PlayerLanes[targetIndex] != null && PlayerTankLanes[targetIndex] != null)
                {
                    GameObject startGO = PlayerLanes[startIndex];
                    GameObject targetGO = PlayerTankLanes[targetIndex];

                    PlayerTankLanes[targetIndex] = startGO;
                    PlayerTankLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                    PlayerLanes[startIndex] = targetGO;
                    PlayerLanes[startIndex].GetComponent<Character>().LanePos = startIndex;


                    PlayerLanes[startIndex].transform.position = PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                    PlayerTankLanes[targetIndex].transform.position = PlayerLanePos[targetIndex] + new Vector3(1.5f, 0, 0);
                }
                else
                {
                    PlayerLanes[targetIndex] = PlayerLanes[startIndex];
                    PlayerLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                    PlayerLanes[targetIndex].GetComponent<Character>().IsTanking = false;
                    PlayerLanes[startIndex] = null;

                    PlayerLanes[targetIndex].transform.position = PlayerLanePos[targetIndex];
                }

            }
            else if (PlayerLanes[targetIndex] != null)//If two character game objects need to switch places
            {
                GameObject startGO = PlayerLanes[startIndex];
                GameObject targetGO = PlayerLanes[targetIndex];

                PlayerLanes[targetIndex] = startGO;
                PlayerLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                PlayerLanes[startIndex] = targetGO;
                PlayerLanes[startIndex].GetComponent<Character>().LanePos = startIndex;


                PlayerLanes[startIndex].transform.position = PlayerLanePos[startIndex];
                PlayerLanes[targetIndex].transform.position = PlayerLanePos[targetIndex];
            }

            else//If the character game object needs to switch to a empty index
            {
                PlayerLanes[targetIndex] = PlayerLanes[startIndex];
                PlayerLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                PlayerLanes[startIndex] = null;
                PlayerLanes[targetIndex].transform.position = PlayerLanePos[targetIndex];

            }
        }
        else
        {
            if (EnemyLanes[targetIndex] != null)
            {
                GameObject startGO = EnemyLanes[startIndex];
                GameObject targetGO = EnemyLanes[targetIndex];

                EnemyLanes[targetIndex] = startGO;
                EnemyLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                EnemyLanes[startIndex] = targetGO;
                EnemyLanes[startIndex].GetComponent<Character>().LanePos = startIndex;


                EnemyLanes[startIndex].transform.position = EnemyLanePos[startIndex];
                EnemyLanes[targetIndex].transform.position = EnemyLanePos[targetIndex];
            }
            else
            {
                EnemyLanes[targetIndex] = EnemyLanes[startIndex];
                EnemyLanes[targetIndex].GetComponent<Character>().LanePos = targetIndex;
                EnemyLanes[startIndex] = null;

                EnemyLanes[targetIndex].transform.position = EnemyLanePos[targetIndex];
            }
        }
    }

    //Combat functions
    private void CheckCombatResult()
    {
        if (EnemyLanes[0] == null && EnemyLanes[1] == null && EnemyLanes[2] == null && EnemyLanes[3] == null && EnemyLanes[4] == null && EnemyLanes[5] == null)
        {
            Debug.Log("VICTORY!");
            endTurnButton.SetActive(false);
        }
        if (PlayerLanes[0] == null && PlayerLanes[1] == null && PlayerLanes[2] == null && PlayerLanes[3] == null && PlayerLanes[4] == null && PlayerLanes[5] == null)
        {
            Debug.Log("DEFEAT!");
            endTurnButton.SetActive(false);
        }
    }
    public void ChooseAttack(string skill)
    {
        SelectingAttack = true;
        ChoosingSkill = skill;
        InfoText.text = "Choose a target!";
    }
    public void ChooseDefend()
    {
        //Sets character to defend, reduces action points, resets selection
        SelectedCharacter.GetComponent<Character>().Defending = true;
        SelectedCharacter.GetComponent<Character>().DefendingStamina = 10;
        SelectedCharacter.GetComponent<Character>().ActionPoints -= 2;
        SelectedCharacter.GetComponent<Character>().AvailableStamina -= 10;
        CS.ResetSelection();
    }
    public void ChooseMove()
    {
        SelectingMove = true;
        InfoText.text = "Choose a lane!";
    }
    public void ChooseRest()
    {
        //Sets character to rest, reduces action points, resets selection
        SelectedCharacter.GetComponent<Character>().Resting = true;
        SelectedCharacter.GetComponent<Character>().ActionPoints -= 3;
        CS.ResetSelection();
    }
    public void EndTurn()
    {
        EnemyTurn();
    }

    //List functions
    public void AddAttack()
    {
        //Creates a new action and adds it to the action list
        Action attack = new Action();
        attack.Agent = SelectedCharacter;
        attack.Target = SelectedEnemyCharacter;

        attack.ActionSpeed = SelectedCharacter.GetComponent<Character>().Speed;
        attack.StaminaCost = StaminaCostAttack();
        attack.Agent.GetComponent<Character>().AvailableStamina -= attack.StaminaCost;

        ActionList.Add(attack);

        CS.ResetSelection();
    }
    public void AddMove()
    {
        Action move = new Action();
        move.Agent = SelectedCharacter;
        move.TargetIndex = SelectedLanePos;
        move.IsPlayer = true;
        move.IsTanking = SelectedCharacter.GetComponent<Character>().IsTanking;
        move.Class = SelectedCharacter.GetComponent<Character>().Class;

        move.StaminaCost = StaminaCostMovement(move.TargetIndex - move.Agent.GetComponent<Character>().LanePos, move.Agent);
        move.Agent.GetComponent<Character>().AvailableStamina -= move.StaminaCost;

            MovementList.Add(move);

        CS.ResetSelection();
    }
    public void AddSkill()
    {
        Action attack = new Action();
        attack.Agent = SelectedCharacter;
        attack.Target = SelectedEnemyCharacter;
        attack.Skill = ChoosingSkill;
        ChoosingSkill = "";

        CS.ResetSelection();

        //Debug
        attack.ActionSpeed = SelectedCharacter.GetComponent<Character>().Speed;
        attack.StaminaCost = StaminaCostAttack();
        if (attack.Agent.GetComponent<Character>().StaminaPoints >= attack.StaminaCost)
        {
            ActionList.Add(attack);
        }
    }

    private void AddAttack(GameObject enemyAgent)
    {
        //Creates a new action and adds it to the action list
        Action attack = new Action();
        attack.Agent = enemyAgent;
        while (attack.Target == null)//Possible crash at game over, if player ends turn after defeat!
        {
            attack.Target = PlayerLanes[Random.Range(0, 5)];
        }

        attack.ActionSpeed = enemyAgent.GetComponent<Character>().Speed;
        attack.StaminaCost = StaminaCostAttack();
        attack.Agent.GetComponent<Character>().AvailableStamina -= attack.StaminaCost;

        if (attack.Agent.GetComponent<Character>().StaminaPoints >= attack.StaminaCost)//Could be arbituary, check later date
        {
            ActionList.Add(attack);
        }
    }
    private void AddMove(GameObject enemyAgent)
    {
        Action move = new Action();
        move.Agent = enemyAgent;
        move.TargetIndex = Random.Range(0, 5);

        move.StaminaCost = StaminaCostMovement(move.TargetIndex - move.Agent.GetComponent<Character>().LanePos, move.Agent);
        move.Agent.GetComponent<Character>().AvailableStamina -= move.StaminaCost;

        if (move.Agent.GetComponent<Character>().StaminaPoints >= move.StaminaCost)//Could be arbituary, check later date
        {
            MovementList.Add(move);
        }
    }

    //Math fuctions
    private int StaminaCostAttack()
    {
        return 10;
    }
    private int StaminaCostMovement(int numberOfLanesMoved, GameObject agent)
    {
        //Stamina cost = 5 * Number of lanes moved * (1 - Character speed / 100)
        return 5 * Mathf.Abs(numberOfLanesMoved) * (1 - (agent.GetComponent<Character>().Speed / 100));
    }

    //Turns functions
    private void InitializeCombat()
    {
        CS = GameObject.Find("BattleManager").GetComponent<ClickingScript>();
        canvas = GameObject.Find("Canvas");
        attackButton = GameObject.Find("AttackButton");
        defendButton = GameObject.Find("DefendButton");
        endTurnButton = GameObject.Find("EndTurnButton");
        moveButton = GameObject.Find("MoveButton");
        restButton = GameObject.Find("RestButton");
        infoTextObject = GameObject.Find("InfoText");
        InfoText = infoTextObject.GetComponent<Text>();
        InfoText.text = "";
        attackButton.SetActive(false);//Setting buttons inactive at start in code seems arbitrary
        defendButton.SetActive(false);
        moveButton.SetActive(false);
        restButton.SetActive(false);
        playerTurn = true;

        //Initialize enemy lanes
        for (int i = 0; i < EnemyLanes.Length; ++i)
        {
            if (EnemyLanes[i] != null)
            {
                EnemyLanes[i] = Instantiate(EnemyLanes[i], EnemyLanePos[i], transform.rotation) as GameObject;
                EnemyLanes[i].GetComponent<Character>().LanePos = i;
                EnemyLanes[i].GetComponent<Character>().Player = false;
            }
            else continue;
        }
        //Initialize player lanes
        for (int i = 0; i < PlayerLanes.Length; ++i)
        {
            if (PlayerLanes[i] != null)
            {
                PlayerLanes[i] = Instantiate(PlayerLanes[i], PlayerLanePos[i], transform.rotation) as GameObject;
                PlayerLanes[i].GetComponent<Character>().LanePos = i;
                PlayerLanes[i].GetComponent<Character>().Player = true;
            }
            else continue;
        }
    }
    private void InitializeRound()
    {
        //Resets character states and adds earned points accordingly (from ie. resting)
        for (int i = 0; i < PlayerLanes.Length; ++i)
        {
            if (PlayerLanes[i] != null)
            {
                if (PlayerLanes[i].GetComponent<Character>().Resting)
                {
                    PlayerLanes[i].GetComponent<Character>().StaminaPoints += 20;
                }
                else if (PlayerLanes[i].GetComponent<Character>().Defending)
                {
                    PlayerLanes[i].GetComponent<Character>().StaminaPoints -= PlayerLanes[i].GetComponent<Character>().DefendingStamina;
                }

                PlayerLanes[i].GetComponent<Character>().ActionPoints = 4;
                PlayerLanes[i].GetComponent<Character>().Attacking = false;
                PlayerLanes[i].GetComponent<Character>().Defending = false;
                PlayerLanes[i].GetComponent<Character>().Moving = false;
                PlayerLanes[i].GetComponent<Character>().Resting = false;

                PlayerLanes[i].GetComponent<Character>().AvailableStamina = PlayerLanes[i].GetComponent<Character>().StaminaPoints;
            }
        }
        for (int i = 0; i < EnemyLanes.Length; ++i)
        {
            if (EnemyLanes[i] != null)
            {
                if (EnemyLanes[i] != null)
                {
                    if (EnemyLanes[i].GetComponent<Character>().Resting)
                    {
                        EnemyLanes[i].GetComponent<Character>().StaminaPoints += 20;
                    }

                    EnemyLanes[i].GetComponent<Character>().ActionPoints = 4;
                    EnemyLanes[i].GetComponent<Character>().Attacking = false;
                    EnemyLanes[i].GetComponent<Character>().Defending = false;
                    EnemyLanes[i].GetComponent<Character>().Moving = false;
                    EnemyLanes[i].GetComponent<Character>().Resting = false;

                    EnemyLanes[i].GetComponent<Character>().AvailableStamina = EnemyLanes[i].GetComponent<Character>().StaminaPoints;
                }
            }
        }
    }
    private void PlayerTurn()
    {
        InitializeRound();
        CS.ResetSelection();
        playerTurn = true;

        //Pelaaja saa kotrollit, valitsee toiminnot ja päättää vuoron.
    }
    private void EnemyTurn()
    {
        playerTurn = false;

        for (int i = 0; i < EnemyLanes.Length; ++i)
        {
            if (EnemyLanes[i] != null)
            {
                //Enemy randomly chooses action, adds to MovementList and ActionList
                AddMove(EnemyLanes[i]);
                AddAttack(EnemyLanes[i]);
            }
        }

        MovementTurn();
    }
    private void MovementTurn()
    {
        playerTurn = false;

        //Suorittaa move listin toiminnot
        for (int i = 0; i < MovementList.Count; ++i)
        {
            //Liikekomennon "omistajan" paikka ja liikekomennon kohde vaihtavat paikkaa
            SwitchPlaces(MovementList[i].Agent.GetComponent<Character>().LanePos, MovementList[i].TargetIndex, MovementList[i].IsPlayer, MovementList[i].IsTanking, MovementList[i].Class);

            //Debug
            MovementList[i].Agent.GetComponent<Character>().StaminaPoints -= MovementList[i].StaminaCost;
        }

        MovementList.Clear();

        ActionTurn();
    }
    private void ActionTurn()
    {
        playerTurn = false;

        //Sorts action list accronding to the action speed and executes the action list's actions
        ActionList.Sort((x, y) => -1 * x.ActionSpeed.CompareTo(y.ActionSpeed));//Sorts actions int DESC order by action's speed

        for (int i = 0; i < ActionList.Count; ++i)
        {
            if (ActionList[i].Agent.GetComponent<Character>().Alive && ActionList[i].StaminaCost <= ActionList[i].Agent.GetComponent<Character>().StaminaPoints)//Dead characters actions are not performed, also if character has no stamina at this point
            {
                Attack(ActionList[i].Agent, ActionList[i].Target, ActionList[i].Skill);

                ActionList[i].Agent.GetComponent<Character>().StaminaPoints -= ActionList[i].StaminaCost;
            }
        }

        ActionList.Clear();

        PlayerTurn();
    }

    //UI functions
    private void displayActionButtons()
    {
        //Enabling and disabling character action buttons
        //Player character is clicked
        if (CS.CharacterClicked && !SelectingAttack && !SelectingMove)
        {
            //Display attack button
            //True if character is not already attacking, has enough action points and has enough unused stamina points
            if (!SelectedCharacter.GetComponent<Character>().Attacking && SelectedCharacter.GetComponent<Character>().ActionPoints >= 2 && SelectedCharacter.GetComponent<Character>().AvailableStamina >= 10)
            {
                attackButton.SetActive(true);
            }
            else
            {
                attackButton.SetActive(false);
            }

            //Display defend button
            //True if character is not already defending, has enough action points and has enough unsued stamina points
            if (!SelectedCharacter.GetComponent<Character>().Defending && SelectedCharacter.GetComponent<Character>().ActionPoints >= 2 && SelectedCharacter.GetComponent<Character>().AvailableStamina >= 10)
            {
                defendButton.SetActive(true);
            }
            else
            {
                defendButton.SetActive(false);
            }

            //Display move button
            //True if character is not already moving, has enough action points and has enough unused stamina to atleast move a single lane factoring in the characters speed stat
            if (!SelectedCharacter.GetComponent<Character>().Moving && SelectedCharacter.GetComponent<Character>().ActionPoints >= 1 && SelectedCharacter.GetComponent<Character>().AvailableStamina >= StaminaCostMovement(1, SelectedCharacter))
            {
                moveButton.SetActive(true);
            }
            else
            {
                moveButton.SetActive(false);
            }

            //Display rest button
            //True if character is not already resting and has enough action points
            if (!SelectedCharacter.GetComponent<Character>().Resting && SelectedCharacter.GetComponent<Character>().ActionPoints >= 3)
            {
                restButton.SetActive(true);
            }
            else
            {
                moveButton.SetActive(false);
            }
        }
        else
        {
            attackButton.SetActive(false);
            defendButton.SetActive(false);
            moveButton.SetActive(false);
            restButton.SetActive(false);
        }
    }

    //Debug
    public void InstantiateDamageNumber(int dmgToStr, int dmgToSta, Transform targetLocation)
    {
        DamageNumber dN;
        DamageNumber dNInstance;

        Text t;

        Vector2 screenPosition;

        dN = Resources.Load<DamageNumber>("Prefabs/Combat/DamageNumber");
        screenPosition = Camera.main.WorldToScreenPoint(targetLocation.position);

        dNInstance = Instantiate(dN);
        dNInstance.transform.SetParent(canvas.transform, false);
        dNInstance.transform.position = screenPosition;

        t = dNInstance.GetComponent<Text>();

        if (dmgToStr < 1)
        {
            t.text = "-" + dmgToSta;
            t.color = Color.yellow;
        }
        else if (dmgToSta < 1)
        {
            t.text = "-" + dmgToStr;
            t.color = Color.red;
        }
        else
        {
            t.text = "-" + dmgToStr + "(" + dmgToSta + ")";
            t.color = new Vector4(1, 0.5f, 0, 1);
        }
    }
    public void ResetTurn()
    {
        if (playerTurn)
        {
            ActionList.Clear();
            MovementList.Clear();

            for (int i = 0; i < PlayerLanes.Length; ++i)
            {
                if (PlayerLanes[i] != null)
                {
                    PlayerLanes[i].GetComponent<Character>().ActionPoints = 4;
                    PlayerLanes[i].GetComponent<Character>().Attacking = false;
                    PlayerLanes[i].GetComponent<Character>().Defending = false;
                    PlayerLanes[i].GetComponent<Character>().Moving = false;
                    PlayerLanes[i].GetComponent<Character>().Resting = false;

                    PlayerLanes[i].GetComponent<Character>().AvailableStamina = PlayerLanes[i].GetComponent<Character>().StaminaPoints;
                }
            }
            for (int i = 0; i < EnemyLanes.Length; ++i)
            {
                if (EnemyLanes[i] != null)
                {
                    EnemyLanes[i].GetComponent<Character>().ActionPoints = 4;
                    EnemyLanes[i].GetComponent<Character>().Attacking = false;
                    EnemyLanes[i].GetComponent<Character>().Defending = false;
                    EnemyLanes[i].GetComponent<Character>().Moving = false;
                    EnemyLanes[i].GetComponent<Character>().Resting = false;

                    EnemyLanes[i].GetComponent<Character>().AvailableStamina = EnemyLanes[i].GetComponent<Character>().StaminaPoints;
                }
            }
        }

    }
}
