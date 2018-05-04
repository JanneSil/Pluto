using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anima2D;

public class BattleManager : MonoBehaviour
{
    private CombatAnimator CA;
    private ClickingScript CS;
    private CombatUIScript UI;

    private bool gameOver = false;
    private GameObject tempUnit;
    private Component[] components;

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
    public List<CombatAction> ActionList = new List<CombatAction>();
    private List<CombatAction> MovementList = new List<CombatAction>();

    //Selection variables
    public bool ChoosingAttack;
    public bool SelectingAttack;
    public bool SelectingMove;

    public GameObject SelectedCharacter;
    public GameObject SelectedEnemyCharacter;

    public int SelectedLanePos;

    public string ChoosingSkill;

    public Text InfoText;

    //Turn variables
    public bool actionTurn;
    private bool characterMoved = false;
    private bool characterAnimated = false;
    private bool movementTurn;
    public bool playerTurn;
    public bool SuccesfulAttack;

    public float ActionDelay;
    public float MovementTurnDelay;
    public float TurnDelay;
    public float TurnDelayRemaining;
    public bool TurnDelayOn;

    private float actionDelayRemaining;
    private float movementTurnDelayRemaining;

    private int actionsDone;
    private int nextActionIndex;
    private int tempIntHolder;
    private bool once;
    private bool removeMovement;

    //UI buttons
    private GameObject selectAttackButton;
    private GameObject selectSkillButton;
    private GameObject tankSkillButton;

    private GameObject attackButton;
    private GameObject defendButton;
    private GameObject endTurnButton;
    private GameObject moveButton;
    private GameObject resetButton;
    private GameObject restButton;
    private GameObject skillButtons;

    //UI other
    public float CameraAttackSize;

    private float cameraWait;

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
        displayActionButtons();
        ActionTurnUpdate();
        MovementTurnUpdate();
        CheckCombatResult();
    }

    //Character functions
    public void ChooseCharacter(int laneIndex, bool player, bool Tanking)
    {
        if (player)
        {

            if (Tanking)
            {
                //Debug.Log("Selected: " + PlayerTankLanes[laneIndex]);
                SelectedCharacter = PlayerTankLanes[laneIndex];
            }

            else
            {
                //Debug.Log("Selected: " + PlayerLanes[laneIndex]);
                SelectedCharacter = PlayerLanes[laneIndex];
            }

        }
        else
        {
            //Debug.Log("Selected: " + EnemyLanes[laneIndex]);
            SelectedEnemyCharacter = EnemyLanes[laneIndex];
        }

    }

    //Combat functions
    private void CheckCombatResult()
    {
        if (playerTurn)
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

    }
    public void ChooseAttack(string skill)
    {
        if (SelectedCharacter.GetComponent<Character>().Attacking == true)
        {
            for (int i = 0; i < ActionList.Count; ++i)
            {
                if (ActionList[i].Agent == SelectedCharacter)
                {
                    ActionList[i].Agent.GetComponent<Character>().AvailableStamina += ActionList[i].StaminaCost;
                    ActionList[i].Target.GetComponent<Character>().Targeted = false;

                    if (ActionList[i].SkillInUse)
                    {
                        SelectedCharacter.GetComponent<Character>().ActionPoints += 3;
                    }
                    else
                    {
                        SelectedCharacter.GetComponent<Character>().ActionPoints += 2;
                    }
                    ActionList.RemoveAt(i);
                }
            }

            SelectedCharacter.GetComponent<Character>().Attacking = false;

            //if (skill == "")
            //{
            //    SelectedCharacter.GetComponent<Character>().ActionPoints += 2;
            //}
            //else
            //{
            //    SelectedCharacter.GetComponent<Character>().ActionPoints += 3;
            //}

            return;
        }
        if (skill == "")
        {
            if (SelectedCharacter.GetComponent<Character>().ActionPoints < 2)
            {
                Debug.Log("Out of Action Points!");
                return;
            }
            if (SelectedCharacter.GetComponent<Character>().AvailableStamina < 10)
            {
                Debug.Log("Out of Stamina!");
                return;
            }
        }
        if (skill != "")
        {
            if (SelectedCharacter.GetComponent<Character>().ActionPoints < 3)
            {
                Debug.Log("Out of Action Points!");
                return;
            }
            if (SelectedCharacter.GetComponent<Character>().AvailableStamina < 15)
            {
                Debug.Log("Out of Stamina!");
                return;
            }
        }
        SelectingAttack = true;
        ChoosingSkill = skill;
        InfoText.text = "Choose a target!";
    }
    public void ChooseDefend()
    {
        if (SelectedCharacter.GetComponent<Character>().Defending == true)
        {
            SelectedCharacter.GetComponent<Character>().Defending = false;
            //SelectedCharacter.GetComponent<Animator>().SetBool("RaiseGuard", false);
            SelectedCharacter.transform.Find("Normal").GetComponent<Animator>().SetBool("RaiseGuard", false);
            SelectedCharacter.GetComponent<Character>().DefendingStamina = 0;
            SelectedCharacter.GetComponent<Character>().ActionPoints += 2;
            SelectedCharacter.GetComponent<Character>().AvailableStamina += 10;
            return;
        }
        if (SelectedCharacter.GetComponent<Character>().ActionPoints < 2 || SelectedCharacter.GetComponent<Character>().AvailableStamina < 10)
        {
            Debug.Log("Out of Action Points/Stamina");
            return;
        }
        //Sets character to defend, reduces action points, resets selection
        SelectedCharacter.GetComponent<Character>().Defending = true;
        //SelectedCharacter.GetComponent<Animator>().SetBool("RaiseGuard", true);
        SelectedCharacter.transform.Find("Normal").GetComponent<Animator>().SetBool("RaiseGuard", true);
        SelectedCharacter.GetComponent<Character>().DefendingStamina = 10;
        SelectedCharacter.GetComponent<Character>().ActionPoints -= 2;
        SelectedCharacter.GetComponent<Character>().AvailableStamina -= 10;
        CS.ResetSelection();
    }
    public void ChooseMove()
    {
        if (SelectedCharacter.GetComponent<Character>().Moving)
        {
            for (int i = 0; i < MovementList.Count; ++i)
            {
                if (MovementList[i].TargetIndex == SelectedCharacter.GetComponent<Character>().LanePos)
                {
                    MovementList[i].Agent.GetComponent<Character>().AvailableStamina += MovementList[i].StaminaCost;
                    MovementList[i].Agent.GetComponent<Character>().Moving = false;
                    MovementList[i].Agent.GetComponent<Character>().ActionPoints += 1;
                    GameObject.Find("Lane" + MovementList[i].TargetIndex).GetComponent<LaneInfo>().LaneChosen = false;
                    GameObject.Find("Lane" + MovementList[i].TargetIndex).GetComponent<LaneInfo>().UnitOnLane = null;
                    removeMovement = true;
                }
                if (MovementList[i].Agent == SelectedCharacter)
                {
                    MovementList[i].Agent.GetComponent<Character>().AvailableStamina += MovementList[i].StaminaCost;
                    GameObject.Find("Lane" + MovementList[i].TargetIndex).GetComponent<LaneInfo>().LaneChosen = false;
                    if (MovementList[i].SwitchingPlaces)
                    {
                        MovementList[i].OtherAgent.GetComponent<Character>().AvailableStamina += MovementList[i].StaminaCost;
                        MovementList[i].OtherAgent.GetComponent<Character>().Moving = false;
                        MovementList[i].OtherAgent.GetComponent<Character>().ActionPoints += 1;
                        GameObject.Find("Lane" + MovementList[i].OtherAgentTargetIndex).GetComponent<LaneInfo>().LaneChosen = false;
                        GameObject.Find("Lane" + MovementList[i].OtherAgentTargetIndex).GetComponent<LaneInfo>().UnitOnLane = null;

                    }
                    removeMovement = true;
                }
                if (removeMovement)
                {
                    MovementList.RemoveAt(i);
                    i -= 1;
                    removeMovement = false;
                }

            }

            SelectedCharacter.GetComponent<Character>().Moving = false;
            SelectedCharacter.GetComponent<Character>().ActionPoints += 1;
            return;
        }

        if (SelectedCharacter.GetComponent<Character>().ActionPoints < 1)
        {
            Debug.Log("Out of Action Points!");
            return;
        }
        if (SelectedCharacter.GetComponent<Character>().AvailableStamina < StaminaCostMovement(1, SelectedCharacter))
        {
            Debug.Log("Out of Stamina!");
            tempUnit = null;
            return;
        }

        SelectingMove = true;
        InfoText.text = "Choose a lane!";


    }
    public void ChooseRest()
    {
        if (SelectedCharacter.GetComponent<Character>().Resting)
        {
            SelectedCharacter.GetComponent<Character>().Resting = false;
            SelectedCharacter.GetComponent<Character>().ActionPoints += 3;
            return;
        }

        if (SelectedCharacter.GetComponent<Character>().ActionPoints < 3)
        {
            Debug.Log("Out of Action Points!");
            return;
        }
        //Sets character to rest, reduces action points, resets selection
        SelectedCharacter.GetComponent<Character>().Resting = true;
        SelectedCharacter.GetComponent<Character>().ActionPoints -= 3;
        CS.ResetSelection();
    }
    public void EnemyRest(GameObject agent)
    {
        agent.GetComponent<Character>().Resting = true;
        agent.GetComponent<Character>().ActionPoints -= 3;
    } 
    public void EnemyDefend(GameObject agent)
    {
        agent.GetComponent<Character>().Defending = true;
        agent.GetComponent<Animator>().SetBool("RaiseGuard", true);
        agent.GetComponent<Character>().DefendingStamina = 10;
        agent.GetComponent<Character>().ActionPoints -= 2;
        agent.GetComponent<Character>().AvailableStamina -= 10;
    }
    public void EndTurn()
    {
        CS.ResetSelection();
        EnemyTurn();
    }

    //List functions
    public void AddAttack()
    {
        //Creates a new action and adds it to the action list
        CombatAction attack = new CombatAction();
        attack.Agent = SelectedCharacter;
        attack.Target = SelectedEnemyCharacter;
        attack.Target.GetComponent<Character>().Targeted = true;
        SelectedCharacter.GetComponent<Character>().Attacking = true;
        SelectedCharacter.GetComponent<Character>().ActionPoints -= 2;

        attack.ActionSpeed = SelectedCharacter.GetComponent<Character>().Speed;
        attack.StaminaCost = StaminaCostAttack();
        attack.Agent.GetComponent<Character>().AvailableStamina -= attack.StaminaCost;

        ActionList.Add(attack);

        CS.ResetSelection();
    }
    public void AddMove()
    {
        if (SelectedCharacter.GetComponent<Character>().LanePos == SelectedLanePos && SelectedCharacter.GetComponent<Character>().Class != "Tank")
        {
            return;
        }

        CombatAction move = new CombatAction();
        move.Agent = SelectedCharacter;
        move.TargetIndex = SelectedLanePos;
        move.StaminaCost = StaminaCostMovement(move.TargetIndex - move.Agent.GetComponent<Character>().LanePos, move.Agent);

        if (SelectedCharacter.GetComponent<Character>().Class != "Tank")
        {
            for (int i = 0; i < MovementList.Count; ++i)
            {
                if (MovementList[i].TargetIndex == move.TargetIndex)
                {
                    Debug.Log("Cannot move there as a another unit is already moving there.");
                    return;
                }
                if (MovementList[i].OtherAgentTargetIndex == move.TargetIndex)
                {
                    Debug.Log(MovementList[i].OtherAgentTargetIndex);
                    Debug.Log(move.TargetIndex);
                    Debug.Log("Cannot move there as a another unit is already moving there.");
                    return;
                }

            }
        }

        if (move.Agent.GetComponent<Character>().AvailableStamina < move.StaminaCost)
        {
            Debug.Log("Not Enough Stamina");
            return;
        }

        if (PlayerLanes[move.TargetIndex] != null && tempUnit == null && SelectedCharacter.GetComponent<Character>().Class != "Tank")
        {
            if (PlayerLanes[move.TargetIndex].GetComponent<Character>().AvailableStamina >= StaminaCostMovement(1, SelectedCharacter))
            {
                if (!PlayerLanes[move.TargetIndex].GetComponent<Character>().Moving)
                {
                    CS.ResetSelection();
                    CS.ObjectClicked = false;
                    CS.EnemyCharacterClicked = false;

                    if (CS.TempEnemyUnitHolder != null)
                    {
                        CS.TempEnemyUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }

                    if (CS.TempUnitHolder != null)
                    {
                        CS.TempUnitHolder.GetComponent<Character>().UnitChosen = false;
                    }
                    tempUnit = move.Agent;
                    CS.CharacterClicked = true;
                    CS.TempUnitHolder = PlayerLanes[move.TargetIndex];
                    PlayerLanes[move.TargetIndex].GetComponent<Character>().CharacterClick();
                    return;
                }
            }
            else
            {
                Debug.Log("Friendly unit doesn't have enough stamina to move away.");
                return;
            }

        }

        if (tempUnit != null)
        {
            if (move.TargetIndex == tempUnit.GetComponent<Character>().LanePos)
            {
                move.OtherAgent = tempUnit;
                if (move.OtherAgent.GetComponent<Character>().ActionPoints < 1 || move.OtherAgent.GetComponent<Character>().AvailableStamina < move.StaminaCost)
                {
                    Debug.Log("Other unit doesn't have enough actions points/stamina");
                    tempUnit = null;
                    return;
                }
                move.Agent.GetComponent<Character>().SwitchingPlaces = true;
                move.OtherAgent.GetComponent<Character>().SwitchingPlaces = true;
                move.SwitchingPlaces = true;
                move.OtherAgentTargetIndex = SelectedCharacter.GetComponent<Character>().LanePos;
                GameObject.Find("Lane" + move.OtherAgentTargetIndex).GetComponent<LaneInfo>().LaneChosen = true;
                move.OtherAgent.GetComponent<Character>().Moving = true;
                move.OtherAgent.GetComponent<Character>().ActionPoints -= 1;
                move.OtherAgent.GetComponent<Character>().AvailableStamina -= move.StaminaCost;
            }
        }

        tempUnit = null;
        move.IsPlayer = true;
        move.IsTanking = SelectedCharacter.GetComponent<Character>().IsTanking;
        move.Class = SelectedCharacter.GetComponent<Character>().Class;

        SelectedCharacter.GetComponent<Character>().Moving = true;
        SelectedCharacter.GetComponent<Character>().ActionPoints -= 1;
        GameObject.Find("Lane" + move.TargetIndex).GetComponent<LaneInfo>().LaneChosen = true;
        GameObject.Find("Lane" + move.TargetIndex).GetComponent<LaneInfo>().UnitOnLane = SelectedCharacter;

        move.Agent.GetComponent<Character>().AvailableStamina -= move.StaminaCost;

        MovementList.Add(move);

        SuccesfulAttack = true;
    }
    public void AddSkill(string newSkill)
    {

        if (newSkill == "TankSkill" && SelectedCharacter.GetComponent<Character>().UsingSkill)
        {
            for (int i = 0; i < ActionList.Count; ++i)
            {
                if (ActionList[i].Agent == SelectedCharacter)
                {
                    ActionList[i].Agent.GetComponent<Character>().AvailableStamina += ActionList[i].StaminaCost;
                    ActionList[i].Target.GetComponent<Character>().Targeted = false;
                    ActionList.RemoveAt(i);
                }

            }
            SelectedCharacter.GetComponent<Character>().ActionPoints += 3;
            SelectedCharacter.GetComponent<Character>().UsingSkill = false;
            SelectedCharacter.GetComponent<Character>().SkillBeingUsed = "";
            return;
        }


        CombatAction skill = new CombatAction();
        skill.Agent = SelectedCharacter;
        if (SelectedEnemyCharacter != null)
        {
            skill.Target = SelectedEnemyCharacter;
        }
        else
        {
            skill.Target = skill.Agent;
        }

        skill.Skill = newSkill;

        skill.StaminaCost = StaminaCostSkill(skill.Skill);

        if (skill.Agent.GetComponent<Character>().AvailableStamina < skill.StaminaCost)
        {
            Debug.Log("Not Enough Stamina");
            return;
        }

        skill.Agent.GetComponent<Character>().AvailableStamina -= skill.StaminaCost;
        SelectedCharacter.GetComponent<Character>().ActionPoints -= 3;
        skill.SkillInUse = true;
        SelectedCharacter.GetComponent<Character>().SkillBeingUsed = newSkill;

        skill.ActionSpeed = SelectedCharacter.GetComponent<Character>().Speed;

        SelectedCharacter.GetComponent<Character>().UsingSkill = true;
        if (SelectedEnemyCharacter != null)
        {
            SelectedEnemyCharacter.GetComponent<Character>().Targeted = true;
        }

        ChoosingSkill = "";

        ActionList.Add(skill);
        CS.ResetSelection();
    }

    private void AddAttack(GameObject enemyAgent)
    {
        //Creates a new action and adds it to the action list
        CombatAction attack = new CombatAction();
        attack.Agent = enemyAgent;

        //while (attack.Target == null)//Possible crash at game over, if player ends turn after defeat!
        //{
        //    attack.Target = PlayerLanes[Random.Range(0, 5)];
        //}
        if (attack.Agent.GetComponent<Character>().ActionPoints < 2)
        {
            return;
        }

        if (PlayerLanes[enemyAgent.GetComponent<Character>().LanePos] != null)
        {
            attack.Target = PlayerLanes[enemyAgent.GetComponent<Character>().LanePos];
        }
        else
        {
            attack.Target = PlayerLanes[CheckLaneForAttack(enemyAgent.GetComponent<Character>().LanePos)];

            if (attack.Target == null)
            {
                    attack.Target = PlayerLanes[Random.Range(0, 5)];
            }
        }

        attack.ActionSpeed = enemyAgent.GetComponent<Character>().Speed;
        attack.StaminaCost = StaminaCostAttack();
        attack.Agent.GetComponent<Character>().AvailableStamina -= attack.StaminaCost;

        if (attack.Agent.GetComponent<Character>().StaminaPoints >= attack.StaminaCost)//Could be arbituary, check later date
        {
            attack.Agent.GetComponent<Character>().Attacking = true;
            attack.Agent.GetComponent<Character>().ActionPoints -= 2;
            ActionList.Add(attack);
        }
    }
    private void AddMove(GameObject enemyAgent)
    {
        CombatAction move = new CombatAction();
        move.Agent = enemyAgent;

        if (move.Agent.GetComponent<Character>().ActionPoints < 1)
        {
            return;
        }

        if (PlayerLanes[enemyAgent.GetComponent<Character>().LanePos] != null && PlayerLanes[enemyAgent.GetComponent<Character>().LanePos].GetComponent<Character>().Class == "" && PlayerTankLanes[enemyAgent.GetComponent<Character>().LanePos] == null)
        {
            return;
        }
        else
        {
            for (int i = 0; i < 6; ++i)
                {

                if (enemyAgent.GetComponent<Character>().LanePos + i <= 5)
                    {
                    
                        if (PlayerLanes[enemyAgent.GetComponent<Character>().LanePos + i] != null)
                        {
                            if (EnemyLanes[enemyAgent.GetComponent<Character>().LanePos + i] == null)
                            {
                                move.TargetIndex = enemyAgent.GetComponent<Character>().LanePos + i;
                                break;
                            }
                        }
                    }
                  if (enemyAgent.GetComponent<Character>().LanePos - i >= 0)
                    {
                   
                    if (PlayerLanes[enemyAgent.GetComponent<Character>().LanePos - i] != null)
                        {
                            if (EnemyLanes[enemyAgent.GetComponent<Character>().LanePos - i] == null)
                            {
                                move.TargetIndex = enemyAgent.GetComponent<Character>().LanePos - i;
                            break;
                            }
                        }

                    }
                }

            
            //move.TargetIndex = Random.Range(0, 5);
        }
        if (PlayerLanes[move.TargetIndex] != null)
        {
            if (PlayerLanes[move.TargetIndex].GetComponent<Character>().Class == "Tank")
            {
                return;
            }
        }

        for (int i = 0; i < MovementList.Count; ++i)
        {
            if (MovementList[i].TargetIndex == move.TargetIndex)
            {
                //move.TargetIndex = Random.Range(0, 5);
                return;

            }
        }

        move.StaminaCost = StaminaCostMovement(move.TargetIndex - move.Agent.GetComponent<Character>().LanePos, move.Agent);

        move.Agent.GetComponent<Character>().AvailableStamina -= move.StaminaCost;


        if (move.Agent.GetComponent<Character>().StaminaPoints >= move.StaminaCost)//Could be arbituary, check later date
        {
            move.Agent.GetComponent<Character>().Moving = true;
            move.Agent.GetComponent<Character>().ActionPoints -= 1;
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
    public int StaminaCostMovement(int numberOfLanesMoved, GameObject agent)
    {
        //Stamina cost = 5 * Number of lanes moved * (1 - Character speed / 100)
        return 5 * Mathf.Abs(numberOfLanesMoved) * (1 - (agent.GetComponent<Character>().Speed / 100));
    }

    //Check lane to remove Tank after depleting all stamina
    private void CheckLane(int number)
    {
            for (int i = 1; i < 6; ++i)
            {
                if (PlayerLanes[number].GetComponent<Character>().LanePos + i <= 5)
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

                if (PlayerLanes[number].GetComponent<Character>().LanePos - i >= 0)
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

    }
    private int CheckLaneForAttack(int number)
    {

        for (int i = 0; i < 6; ++i)
            {
                if (number + i <= 5)
                {
                    if (PlayerLanes[number + i] != null) 
                        {
                            return PlayerLanes[number + i].GetComponent<Character>().LanePos;
                        }   

                }
                if (number - i >= 0)
                {
                    if (PlayerLanes[number - i] != null)
                    {
                        return PlayerLanes[number - i].GetComponent<Character>().LanePos;
                    }

                }

            }
        
            return EnemyLanes[number].GetComponent<Character>().LanePos;
    }

    //Turns functions
    private void InitializeCombat()
    {
        CA = GetComponent<CombatAnimator>();
        CS = GameObject.Find("BattleManager").GetComponent<ClickingScript>();
        UI = GameObject.Find("CombatButtons").GetComponent<CombatUIScript>();
        canvas = GameObject.Find("Canvas");
        TurnDelayRemaining = TurnDelay;
        actionDelayRemaining = ActionDelay;
        //movementTurnDelayRemaining = MovementTurnDelay;

        selectAttackButton = GameObject.Find("SelectAttackAction");
        selectSkillButton = GameObject.Find("SelectSkillButton");
        tankSkillButton = GameObject.Find("TankSkillButton");

        attackButton = GameObject.Find("AttackButton");
        defendButton = GameObject.Find("DefendButton");
        endTurnButton = GameObject.Find("EndTurnButton");
        moveButton = GameObject.Find("MoveButton");
        resetButton = GameObject.Find("ResetButton");
        restButton = GameObject.Find("RestButton");
        infoTextObject = GameObject.Find("InfoText");
        skillButtons = GameObject.Find("Skills");

        InfoText = infoTextObject.GetComponent<Text>();
        InfoText.text = "";

        attackButton.SetActive(false);//Setting buttons inactive at start in code seems arbitrary
        defendButton.SetActive(false);
        endTurnButton.SetActive(false);
        moveButton.SetActive(false);
        resetButton.SetActive(false);
        restButton.SetActive(false);
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
                components = EnemyLanes[i].GetComponentsInChildren<SpriteMeshInstance>();

                foreach (SpriteMeshInstance spritemesh in components)
                {
                    spritemesh.sortingLayerName = "Lane" + i;
                }
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
                components = PlayerLanes[i].transform.Find("Normal").GetComponentsInChildren<SpriteMeshInstance>();

                foreach (SpriteMeshInstance spritemesh in components)
                {
                    spritemesh.sortingLayerName = "Lane" + i;
                }
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
                PlayerLanes[i].transform.Find("Normal").GetComponent<Animator>().SetBool("RaiseGuard", false);
                PlayerLanes[i].GetComponent<Character>().Moving = false;
                PlayerLanes[i].GetComponent<Character>().Resting = false;
                PlayerLanes[i].GetComponent<Character>().UsingSkill = false;
                PlayerLanes[i].GetComponent<Character>().SkillBeingUsed = "";
                PlayerLanes[i].GetComponent<Character>().SwitchingPlaces = false;

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
                //PlayerTankLanes[i].GetComponent<Animator>().SetBool("RaiseGuard", false);
                PlayerTankLanes[i].transform.Find("Normal").GetComponent<Animator>().SetBool("RaiseGuard", false);
                PlayerTankLanes[i].GetComponent<Character>().Moving = false;
                PlayerTankLanes[i].GetComponent<Character>().Resting = false;
                PlayerTankLanes[i].GetComponent<Character>().SwitchingPlaces = false;

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

                if (PlayerLanes[i] == null)
                {
                    PlayerLanes[i] = PlayerTankLanes[i];
                    PlayerLanes[i].GetComponent<Character>().LanePos = i;
                    PlayerTankLanes[i] = null;
                    PlayerLanes[i].GetComponent<Character>().IsTanking = false;

                    PlayerLanes[i].GetComponent<Character>().gameObjectTargetPosition = PlayerLanePos[i];
                    PlayerLanes[i].GetComponent<Character>().gameObjectToMove = PlayerLanes[i];
                    PlayerLanes[i].GetComponent<Character>().HasToMove = true;
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
                    EnemyLanes[i].GetComponent<Animator>().SetBool("RaiseGuard", false);
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
                if (EnemyLanes[i].GetComponent<Character>().AvailableStamina < 10)
                {
                    Debug.Log("Enemy is Resting");
                    EnemyRest(EnemyLanes[i]);
                }
                if (EnemyLanes[i].GetComponent<Character>().Targeted && EnemyLanes[i].GetComponent<Character>().ActionPoints >= 2)
                {
                    float randomValue = Random.value;
                    if (EnemyLanes[i].GetComponent<Character>().StrengthPoints < 20)
                    {
                        if (randomValue >= 0.3f)
                        {
                            Debug.Log("Enemy defended");
                            EnemyDefend(EnemyLanes[i]);
                        }
                    }
                    else
                    {
                        if (randomValue >= 0.7f)
                        {
                            Debug.Log("Enemy defended");
                            EnemyDefend(EnemyLanes[i]);
                        }
                    }
                }
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
        once = false;

        actionDelayRemaining = ActionDelay;
        actionsDone = 0;
        nextActionIndex = 0;
        tempIntHolder = 0;
    }
    private void MovementTurnUpdate()
    {
        if (movementTurn)
        {
            //Movement is performed if delay has passed and its index is not out of bounds
            if (MovementList.Count == 0)
            {
                movementTurnDelayRemaining = 0;
                MovementList.Clear();
                ActionTurn();
                return;
            }
            else if (MovementList[tempIntHolder].Agent == null)
            {
                tempIntHolder = nextActionIndex;
            }

            else if (nextActionIndex < MovementList.Count && movementTurnDelayRemaining <= 0)
            {
                if (!MovementList[nextActionIndex].Agent.GetComponent<Character>().Player && once == false)
                {
                    movementTurnDelayRemaining = MovementTurnDelay;
                    once = true;
                    return;
                }
                tempIntHolder = nextActionIndex;
                MovementList[nextActionIndex].Agent.GetComponent<Character>().SwitchPlaces(MovementList[nextActionIndex].Agent.GetComponent<Character>().LanePos, MovementList[nextActionIndex].TargetIndex);

                MovementList[nextActionIndex].Agent.GetComponent<Character>().StaminaPoints -= MovementList[nextActionIndex].StaminaCost;
                if (MovementList[nextActionIndex].OtherAgent != null)
                {
                    MovementList[nextActionIndex].OtherAgent.GetComponent<Character>().StaminaPoints -= MovementList[nextActionIndex].StaminaCost;
                }
                actionsDone += 1;
                nextActionIndex += 1;
                if (actionsDone == MovementList.Count)
                {
                    movementTurnDelayRemaining = MovementTurnDelay;
                }
            }
            else
            {
                movementTurnDelayRemaining -= Time.deltaTime;
            }

            if (actionsDone == MovementList.Count && !MovementList[tempIntHolder].Agent.GetComponent<Character>().HasToMove && movementTurnDelayRemaining <= 0)
            {
                movementTurnDelayRemaining = 0;

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

        actionDelayRemaining = ActionDelay;
        actionsDone = 0;
        nextActionIndex = 0;

        //Sorts action list accronding to the action speed and executes the action list's actions
        ActionList.Sort((x, y) => -1 * x.ActionSpeed.CompareTo(y.ActionSpeed));//Sorts actions int DESC order by action's speed
    }
    private void ActionTurnUpdate()
    {
        if (actionTurn)
        {
            //nextActionIndex is in bounds
            if (nextActionIndex < ActionList.Count)
            {
                //Dead characters actions do not move the camera or the character, also if character has no stamina at this point action is not performed.
                if (ActionList[nextActionIndex].Agent != null && ActionList[nextActionIndex].Target != null && ActionList[nextActionIndex].StaminaCost <= ActionList[nextActionIndex].Agent.GetComponent<Character>().StaminaPoints)
                {
                    //Camera wait time has passed
                    if (cameraWait <= 0)
                    {
                        if (ActionList[nextActionIndex].SkillInUse)
                        {
                            if (!characterAnimated)
                            {
                                characterAnimated = true;
                                if (ActionList[nextActionIndex].Agent.GetComponent<Character>().Player)
                                {
                                    ActionList[nextActionIndex].Agent.transform.Find("Normal").gameObject.SetActive(false);
                                    ActionList[nextActionIndex].Agent.transform.Find("Attack").gameObject.SetActive(true);
                                }
                                else
                                {
                                    ActionList[nextActionIndex].Agent.GetComponent<Animator>().SetTrigger("Attacking");
                                }

                                if (PlayerTankLanes[ActionList[nextActionIndex].Target.GetComponent<Character>().LanePos] != null)
                                {
                                    PlayerTankLanes[ActionList[nextActionIndex].Target.GetComponent<Character>().LanePos].GetComponent<Animator>().SetTrigger("TakeHit");
                                }
                                else
                                {
                                    if (ActionList[nextActionIndex].Target != ActionList[nextActionIndex].Agent)
                                    {
                                        if (ActionList[nextActionIndex].Target.GetComponent<Character>().Player)
                                        {
                                            ActionList[nextActionIndex].Target.transform.Find("Normal").GetComponent<Animator>().SetTrigger("TakeHit");
                                        }
                                        else
                                        {
                                            ActionList[nextActionIndex].Target.GetComponent<Animator>().SetTrigger("TakeHit");
                                        }
                                    }

                                }
                                CA.CameraMove(EnemyLanePos[ActionList[nextActionIndex].Agent.GetComponent<Character>().LanePos], CameraAttackSize);
                            }
                        }
                        else
                        {
                            if (characterAnimated == false)
                            {
                                characterAnimated = true;
                                if (ActionList[nextActionIndex].Agent.GetComponent<Character>().Player)
                                {
                                    ActionList[nextActionIndex].Agent.transform.Find("Normal").gameObject.SetActive(false);
                                    ActionList[nextActionIndex].Agent.transform.Find("Attack").gameObject.SetActive(true);

                                    components = ActionList[nextActionIndex].Agent.transform.Find("Attack").GetComponentsInChildren<SpriteMeshInstance>();

                                    foreach (SpriteMeshInstance spritemesh in components)
                                    {
                                        spritemesh.sortingLayerName = "Lane" + (ActionList[nextActionIndex].Target.GetComponent<Character>().LanePos + 1);
                                    }
                                }
                                else
                                {
                                    ActionList[nextActionIndex].Agent.GetComponent<Animator>().SetTrigger("Attacking");
                                }

                                if (PlayerTankLanes[ActionList[nextActionIndex].Target.GetComponent<Character>().LanePos] != null)
                                {
                                    PlayerTankLanes[ActionList[nextActionIndex].Target.GetComponent<Character>().LanePos].GetComponent<Animator>().SetTrigger("TakeHit");
                                }
                                else
                                {
                                    if (ActionList[nextActionIndex].Target != ActionList[nextActionIndex].Agent)
                                    {
                                        if (ActionList[nextActionIndex].Target.GetComponent<Character>().Player)
                                        {
                                            ActionList[nextActionIndex].Target.transform.Find("Normal").GetComponent<Animator>().SetTrigger("TakeHit");
                                        }
                                        else
                                        {
                                            ActionList[nextActionIndex].Target.GetComponent<Animator>().SetTrigger("TakeHit");
                                        }
                                    }

                                }

                                CA.CameraMove(ActionList[nextActionIndex].Target.transform.position + new Vector3(0,1), CameraAttackSize);
                            }

                        }
                    }

                    //Character hasn't moved
                    if (!characterMoved && cameraWait <= 0)
                    {
                        if (ActionList[nextActionIndex].SkillInUse)
                        {
                            CA.MoveAttack(ActionList[nextActionIndex].Agent, null, ActionDelay * 0.9f);
                        }
                        else
                        {
                            
                            CA.MoveAttack(ActionList[nextActionIndex].Agent, ActionList[nextActionIndex].Target, ActionDelay * 0.9f);
                        }
                        characterMoved = true;
                        TurnDelayOn = true;
                    }
                }

                //Action is performed if delay has passed
                if (actionDelayRemaining <= 0)
                {
                    TurnDelayRemaining -= Time.deltaTime;
                    //Dead characters actions are not performed, also if character has no stamina at this point action is not performed.
                    if (ActionList[nextActionIndex].Agent != null && ActionList[nextActionIndex].StaminaCost <= ActionList[nextActionIndex].Agent.GetComponent<Character>().StaminaPoints && TurnDelayOn)
                    {
                        if (ActionList[nextActionIndex].Target == null && !ActionList[nextActionIndex].SkillInUse)
                        {

                        }
                        else if (ActionList[nextActionIndex].SkillInUse)
                        {
                            ActionList[nextActionIndex].Agent.GetComponent<Character>().PerformSkill(ActionList[nextActionIndex].Agent, ActionList[nextActionIndex].Skill);
                        }
                        else
                        {
                            ActionList[nextActionIndex].Agent.GetComponent<Character>().Attack(ActionList[nextActionIndex].Target);
                        }

                        //Stamina cost is subtracted
                        ActionList[nextActionIndex].Agent.GetComponent<Character>().StaminaPoints -= ActionList[nextActionIndex].StaminaCost;
                        TurnDelayOn = false;
                    }
                    if (ActionList[nextActionIndex].Agent == null)
                    {
                        TurnDelayRemaining = 0;
                    }

                    if (TurnDelayRemaining <= 0)
                    {
                        actionDelayRemaining = ActionDelay;
                        actionsDone += 1;
                        cameraWait = ActionDelay * 0.2f;
                        characterMoved = false;
                        characterAnimated = false;
                        if (ActionList[nextActionIndex].Agent != null)
                        {
                            if (ActionList[nextActionIndex].Agent.GetComponent<Character>().Player)
                            {
                                components = ActionList[nextActionIndex].Agent.transform.Find("Attack").GetComponentsInChildren<SpriteMeshInstance>();
                                foreach (SpriteMeshInstance spritemesh in components)
                                {
                                    spritemesh.sortingLayerName = "Lane" + ActionList[nextActionIndex].Agent.GetComponent<Character>().LanePos;
                                }
                            }
                        }
                        CA.CameraReset();
                        nextActionIndex += 1;
                        TurnDelayRemaining = TurnDelay;
                    }

                }
                else
                {
                    actionDelayRemaining -= Time.deltaTime;
                    cameraWait -= Time.deltaTime;

                    if (ActionList[nextActionIndex].Agent == null)
                    {
                        actionDelayRemaining = 0f;
                        cameraWait = 0f;
                    }
                }
            }

            //All actions on the list are completed, and camera wait time is over: action turn ends
            if (actionsDone == ActionList.Count)
            {
                if (cameraWait <= 0)
                {
                    actionDelayRemaining = 0;
                    ActionList.Clear();
                    CA.CameraReset();
                    PlayerTurn();
                }
                else
                {
                    cameraWait -= Time.deltaTime;
                }
            }
        }
    }

    //UI functions
    public void InstantiateDamageNumber(int dmgToStr, int dmgToSta, Transform targetLocation, bool crit)
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
            if (crit)
            {
                t.text = dmgToStr + "(" + dmgToSta + ")";
                t.color = Color.magenta;
            }
            else
            {
                t.text = "-" + dmgToStr + "(" + dmgToSta + ")";
                t.color = new Vector4(1, 0.5f, 0, 1);
            }
        }
    }

    private void displayActionButtons()
    {
        //Enabling and disabling character action buttons
        //Player character is clicked
        if (CS.CharacterClicked && !SelectingAttack && !SelectingMove)
        {
            //Display attack button
            selectAttackButton.SetActive(true);

            if (UI.SelectAttack && !UI.SelectSkill)
            {
                attackButton.SetActive(true);
                selectSkillButton.SetActive(true);
                moveButton.SetActive(false);
                restButton.SetActive(false);
                defendButton.SetActive(false);
            }
            else if (UI.SelectAttack && UI.SelectSkill)
            {
                attackButton.SetActive(false);
            }
            else
            {
                attackButton.SetActive(false);
                selectSkillButton.SetActive(false);
                UI.SelectSkill = false;
                defendButton.SetActive(true);
                moveButton.SetActive(true);
                restButton.SetActive(true);
            }


            if (!SelectedCharacter.GetComponent<Character>().Attacking && UI.SelectAttack)
            {
                attackButton.GetComponent<Image>().color = Color.green;
            }
            else if (SelectedCharacter.GetComponent<Character>().Attacking && UI.SelectAttack && SelectedCharacter.GetComponent<Character>().UsingSkill == false)
            {
                attackButton.GetComponent<Image>().color = Color.black;
            }

            if (UI.SelectSkill)
            {
                skillButtons.SetActive(true);
            }
            else
            {
                skillButtons.SetActive(false);
            }

            if (SelectedCharacter.GetComponent<Character>().UsingSkill)
            {
                selectSkillButton.GetComponent<Image>().color = Color.black;
            }
            else
            {
                selectSkillButton.GetComponent<Image>().color = Color.green;
            }

            if (SelectedCharacter.GetComponent<Character>().Attacking || SelectedCharacter.GetComponent<Character>().UsingSkill)
            {
                selectAttackButton.GetComponent<Image>().color = Color.black;
            }
            else
            {
                selectAttackButton.GetComponent<Image>().color = Color.yellow;
            }
            if (SelectedCharacter.GetComponent<Character>().SkillBeingUsed == "TankSkill")
            {
                tankSkillButton.GetComponent<Image>().color = Color.black;
            }
            else
            {
                tankSkillButton.GetComponent<Image>().color = Color.green;
            }

            //Display defend button
            if (!SelectedCharacter.GetComponent<Character>().Defending)
            {
                defendButton.GetComponent<Image>().color = Color.green;
            }

            else if (SelectedCharacter.GetComponent<Character>().Defending)
            {
                defendButton.GetComponent<Image>().color = Color.black;
            }


            //Display move button
            //True if character is not already moving, has enough action points and has enough unused stamina to atleast move a single lane factoring in the characters speed stat
            if (!SelectedCharacter.GetComponent<Character>().Moving)
            {
                moveButton.GetComponent<Image>().color = Color.green;
            }
            else if (SelectedCharacter.GetComponent<Character>().Moving)
            {
                moveButton.GetComponent<Image>().color = Color.black;
            }

            //Display rest button
            if (!SelectedCharacter.GetComponent<Character>().Resting)
            {
                restButton.GetComponent<Image>().color = Color.green;
            }
            else if (SelectedCharacter.GetComponent<Character>().Resting)
            {
                restButton.GetComponent<Image>().color = Color.black;
            }

        }
        else
        {
            selectAttackButton.SetActive(false);
            attackButton.SetActive(false);
            defendButton.SetActive(false);
            moveButton.SetActive(false);
            restButton.SetActive(false);
            skillButtons.SetActive(false);
            selectSkillButton.SetActive(false);

            UI.SelectAttack = false;
            UI.SelectSkill = false;
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
