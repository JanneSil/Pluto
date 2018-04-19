using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private ClickingScript CS;

    private bool gameOver = false;

    //Lane data arrays
    public GameObject[] EnemyLanes = new GameObject[6];
    public GameObject[] PlayerLanes = new GameObject[6];
    public GameObject[] EnemyTankLanes = new GameObject[6];
    public GameObject[] PlayerTankLanes = new GameObject[6];

    public Vector3[] EnemyLanePos = new Vector3[] { new Vector3(   3,  0.5f,  0),
                                                     new Vector3(3.4f, -0.3f, -1),
                                                     new Vector3(3.8f, -1.1f, -2),
                                                     new Vector3(4.2f, -1.9f, -3),
                                                     new Vector3(4.6f, -2.7f, -4),
                                                     new Vector3(   5, -3.5f, -5) };
    public Vector3[] PlayerLanePos = new Vector3[] { new Vector3(   -3,  0.5f,  0),
                                                      new Vector3(-3.4f, -0.3f, -1),
                                                      new Vector3(-3.8f, -1.1f, -2),
                                                      new Vector3(-4.2f, -1.9f, -3),
                                                      new Vector3(-4.6f, -2.7f, -4),
                                                      new Vector3(   -5, -3.5f, -5) };

    //Lists
    private List<CombatAction> ActionList = new List<CombatAction>();
    private List<CombatAction> MovementList = new List<CombatAction>();

    //Selection variables
    public bool ChoosingAttack;
    public string ChoosingSkill;
    public bool SelectingAttack;
    public bool SelectingMove;

    public GameObject SelectedCharacter;
    public GameObject SelectedEnemyCharacter;

    public int SelectedLanePos;

    public Text InfoText;

    //Turn variables
    private bool actionTurn;
    private bool movementTurn;
    private bool playerTurn;

    private int actionsDone;
    private int nextActionIndex;

    //UI buttons
    private GameObject attackButton;
    private GameObject defendButton;
    private GameObject endTurnButton;
    private GameObject moveButton;
    private GameObject resetButton;
    private GameObject restButton;
    private GameObject tankSkillButton;

    //UI other
    private GameObject canvas;
    private GameObject infoTextObject;

    //Unity functions
    private void Start()
    {
        InitializeCombat();
        playerTurn = true;
    }
    private void Update()
    {
        CheckCombatResult();
        displayActionButtons();

        ActionTurnUpdate();
        MovementTurnUpdate();
    }

    //Character functions, should consider moving these to Character.cs except for ChooseCharacter()
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

    //Combat functions
    private void CheckCombatResult()
    {
        if (EnemyLanes[0] == null && EnemyLanes[1] == null && EnemyLanes[2] == null && EnemyLanes[3] == null && EnemyLanes[4] == null && EnemyLanes[5] == null)
        {
            gameOver = true;
            InfoText.text = "You Win!";
            endTurnButton.SetActive(false);
        }
        if (PlayerLanes[0] == null && PlayerLanes[1] == null && PlayerLanes[2] == null && PlayerLanes[3] == null && PlayerLanes[4] == null && PlayerLanes[5] == null)
        {
            gameOver = true;
            InfoText.text = "You Lose!";
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
        CombatAction attack = new CombatAction();
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
        if (SelectedCharacter.GetComponent<Character>().LanePos == SelectedLanePos)
        {
            return;
        }
        CombatAction move = new CombatAction();
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
        CombatAction skill = new CombatAction();
        skill.Agent = SelectedCharacter;
        skill.Target = SelectedEnemyCharacter;
        skill.Skill = ChoosingSkill;

        CS.ResetSelection();

        skill.ActionSpeed = SelectedCharacter.GetComponent<Character>().Speed;
        skill.StaminaCost = StaminaCostSkill(ChoosingSkill);
        ChoosingSkill = "";

        if (skill.Agent.GetComponent<Character>().StaminaPoints >= skill.StaminaCost)
        {
            ActionList.Add(skill);
        }
    }

    private void AddAttack(GameObject enemyAgent)
    {
        //Creates a new action and adds it to the action list
        CombatAction attack = new CombatAction();
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
        CombatAction move = new CombatAction();
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
    private int StaminaCostSkill(string skill)
    {
        if (skill == "TankSkill")
        {
            return 15;
        }
        else
        {
            return 0;
        }
    }
    private int StaminaCostMovement(int numberOfLanesMoved, GameObject agent)
    {
        //Stamina cost = 5 * Number of lanes moved * (1 - Character speed / 100)
        return 5 * Mathf.Abs(numberOfLanesMoved) * (1 - (agent.GetComponent<Character>().Speed / 100));
    }

    //Check lane to remove Tank after depleting all stamina
    private void CheckLane(int number)
    {
        if (PlayerLanes[number].GetComponent<Character>().LanePos == 5)
        {
            for (int i = 1; i < 5; ++i)
            {
                if (PlayerLanes[number - i] == null)
                {
                    PlayerLanes[number - i] = PlayerTankLanes[number];
                    PlayerLanes[number - i].GetComponent<Character>().LanePos = number - i;
                    PlayerTankLanes[number] = null;
                    PlayerLanes[number - i].GetComponent<Character>().IsTanking = false;
                    PlayerLanes[number - i].transform.position = PlayerLanePos[number - i];
                    return;
                }
            }

        }
        else if (PlayerLanes[number].GetComponent<Character>().LanePos == 0)
        {
            for (int i = 1; i < 5; ++i)
            {
                if (PlayerLanes[number + i] == null)
                {
                    PlayerLanes[number + i] = PlayerTankLanes[number];
                    PlayerLanes[number + i].GetComponent<Character>().LanePos = number + i;
                    PlayerTankLanes[number] = null;
                    PlayerLanes[number + i].GetComponent<Character>().IsTanking = false;
                    PlayerLanes[number + i].transform.position = PlayerLanePos[number + i];
                    return;
                }
            }

        }

        else if (PlayerLanes[number].GetComponent<Character>().LanePos != 5 && PlayerLanes[number].GetComponent<Character>().LanePos != 0)
        {
            for (int i = 1; i < 5; ++i)
            {
                if (PlayerLanes[number + i] == null)
                {
                    PlayerLanes[number + i] = PlayerTankLanes[number];
                    PlayerLanes[number + i].GetComponent<Character>().LanePos = number + i;
                    PlayerTankLanes[number] = null;
                    PlayerLanes[number + i].GetComponent<Character>().IsTanking = false;
                    PlayerLanes[number + i].transform.position = PlayerLanePos[number + i];
                    return;
                }
                else if (PlayerLanes[number - i] == null)
                {
                    PlayerLanes[number - i] = PlayerTankLanes[number];
                    PlayerLanes[number - i].GetComponent<Character>().LanePos = number - i;
                    PlayerTankLanes[number] = null;
                    PlayerLanes[number - i].GetComponent<Character>().IsTanking = false;
                    PlayerLanes[number - i].transform.position = PlayerLanePos[number - i];
                    return;
                }
            }

        }
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
        resetButton = GameObject.Find("ResetButton");
        restButton = GameObject.Find("RestButton");
        infoTextObject = GameObject.Find("InfoText");
        tankSkillButton = GameObject.Find("TankSkillButton");
        InfoText = infoTextObject.GetComponent<Text>();
        InfoText.text = "";
        attackButton.SetActive(false);//Setting buttons inactive at start in code seems arbitrary
        defendButton.SetActive(false);
        endTurnButton.SetActive(false);
        moveButton.SetActive(false);
        resetButton.SetActive(false);
        restButton.SetActive(false);
        tankSkillButton.SetActive(false);
        actionTurn = false;
        movementTurn = false;
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

        for (int i = 0; i < PlayerTankLanes.Length; ++i)
        {
            if (PlayerTankLanes[i] != null)
            {
                if (PlayerTankLanes[i].GetComponent<Character>().Resting)
                {
                    PlayerTankLanes[i].GetComponent<Character>().StaminaPoints += 20;
                }
                else if (PlayerTankLanes[i].GetComponent<Character>().Defending)
                {
                    PlayerTankLanes[i].GetComponent<Character>().StaminaPoints -= PlayerTankLanes[i].GetComponent<Character>().DefendingStamina;
                }

                PlayerTankLanes[i].GetComponent<Character>().ActionPoints = 4;
                PlayerTankLanes[i].GetComponent<Character>().Attacking = false;
                PlayerTankLanes[i].GetComponent<Character>().Defending = false;
                PlayerTankLanes[i].GetComponent<Character>().Moving = false;
                PlayerTankLanes[i].GetComponent<Character>().Resting = false;

                PlayerTankLanes[i].GetComponent<Character>().AvailableStamina = PlayerTankLanes[i].GetComponent<Character>().StaminaPoints;

                if (PlayerTankLanes[i].GetComponent<Character>().AvailableStamina <= 0)
                {
                    if (PlayerLanes[i] == null)
                    {
                        PlayerLanes[i] = PlayerTankLanes[i];
                        PlayerLanes[i].GetComponent<Character>().LanePos = i;
                        PlayerTankLanes[i] = null;
                        PlayerLanes[i].GetComponent<Character>().IsTanking = false;
                        PlayerLanes[i].transform.position = PlayerLanePos[i];
                    }
                    else
                    {
                        CheckLane(i);
                    }
                }
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
        actionTurn = false;
        movementTurn = false;
        playerTurn = true;

        //Pelaaja saa kotrollit, valitsee toiminnot ja päättää vuoron.
    }
    private void EnemyTurn()
    {
        playerTurn = false;
        movementTurn = false;
        actionTurn = false;

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
        actionTurn = false;
        movementTurn = true;
        playerTurn = false;

        actionsDone = 0;
        nextActionIndex = 0;
    }
    private void MovementTurnUpdate()
    {
        if (movementTurn)
        {
            //Movement is performed if delay has passed and its index is not out of bounds
            if (actionDelayRemaining <= 0 && nextActionIndex < MovementList.Count)
            {
                MovementList[nextActionIndex].Agent.GetComponent<Character>().SwitchPlaces(MovementList[nextActionIndex].Agent.GetComponent<Character>().LanePos, MovementList[nextActionIndex].TargetIndex);

                MovementList[nextActionIndex].Agent.GetComponent<Character>().StaminaPoints -= MovementList[nextActionIndex].StaminaCost;

                actionsDone += 1;
                nextActionIndex += 1;
                actionDelayRemaining = ActionDelay;
            }
            else
            {
                actionDelayRemaining -= Time.deltaTime;
            }

            if (actionsDone == MovementList.Count)
            {
                actionDelayRemaining = 0;

                MovementList.Clear();

                ActionTurn();
            }
        }
    }
    private void ActionTurn()
    {
        actionTurn = true;
        movementTurn = false;
        playerTurn = false;

        actionsDone = 0;
        nextActionIndex = 0;

        //Sorts action list accronding to the action speed and executes the action list's actions
        ActionList.Sort((x, y) => -1 * x.ActionSpeed.CompareTo(y.ActionSpeed));//Sorts actions int DESC order by action's speed
    }
    private void ActionTurnUpdate()
    {
        if (actionTurn)
        {
            //Action is performed if delay has passed and its index is not out of bounds
            if (actionDelayRemaining <= 0 && nextActionIndex < ActionList.Count)
            {
                //Dead characters actions are not performed, also if character has no stamina at this point action is not performed. FOR NOW DEAD TARGETS ARE NOT ATTACKED!
                if (ActionList[nextActionIndex].Agent != null && ActionList[nextActionIndex].Target != null && ActionList[nextActionIndex].StaminaCost <= ActionList[nextActionIndex].Agent.GetComponent<Character>().StaminaPoints)
                {
                    ActionList[nextActionIndex].Agent.GetComponent<Character>().Attack(ActionList[nextActionIndex].Target);
                    ActionList[nextActionIndex].Agent.GetComponent<Character>().PerformSkill(ActionList[nextActionIndex].Target, ActionList[nextActionIndex].Skill);

                    ActionList[nextActionIndex].Agent.GetComponent<Character>().StaminaPoints -= ActionList[nextActionIndex].StaminaCost;
                }
                actionsDone += 1;
                nextActionIndex += 1;
                actionDelayRemaining = ActionDelay;
            }
            else
            {
                actionDelayRemaining -= Time.deltaTime;
            }

            if (actionsDone == ActionList.Count)
            {
                actionDelayRemaining = 0;

                ActionList.Clear();

                PlayerTurn();
            }
        }
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

            if (!SelectedCharacter.GetComponent<Character>().Attacking && SelectedCharacter.GetComponent<Character>().ActionPoints >= 2 && SelectedCharacter.GetComponent<Character>().AvailableStamina >= 10 && SelectedCharacter.GetComponent<Character>().Class == "Tank")
            {
                tankSkillButton.SetActive(true);
            }
            else
            {
                tankSkillButton.SetActive(false);
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
                restButton.SetActive(false);
            }
        }
        else
        {
            attackButton.SetActive(false);
            defendButton.SetActive(false);
            moveButton.SetActive(false);
            restButton.SetActive(false);
            tankSkillButton.SetActive(false);
        }

        //Display reset and end turn button
        if (playerTurn && !gameOver)
        {
            endTurnButton.SetActive(true);
            resetButton.SetActive(true);
        }
        else
        {
            endTurnButton.SetActive(false);
            resetButton.SetActive(false);
        }
    }

    //Debug
    public float ActionDelay;
    private float actionDelayRemaining;
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
