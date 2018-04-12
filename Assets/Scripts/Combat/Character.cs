using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private BattleManager BM;
    //private ClickingScript CS;
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

    [Header("Stats:")]
    [Range(1, 100)]
    public int Strength;
    [Range(1, 100)]
    public int Stamina;
    [Range(1, 100)]
    public int Speed;
    [Range(1, 100)]
    public int Dexterity;

    //Turn saved stats
    [HideInInspector]
    public bool Player;

    public int AvailableStamina;
    public int DefendingStamina;
    public int LanePos;

    private int staminaPointsMax;
    private int strengthPointsMax;

    void Start()
    {
        BM = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        //CS = GameObject.Find("BattleManager").GetComponent<ClickingScript>();
        sprite = GetComponent<SpriteRenderer>();

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

        if (!UnitChosen && Player)
        {
            sprite.color = Color.yellow;
        }
        else if (!UnitChosen && !Player)
        {
            sprite.color = Color.red;
        }

        //Update UI elements
        if (healthBar != null)
        {
            healthBar.GetComponent<Slider>().value = ((float)StrengthPoints / (float)strengthPointsMax);
        }
        if (staminaBar != null)
        {
            staminaBar.GetComponent<Slider>().value = ((float)StaminaPoints / (float)staminaPointsMax);
        }
        if(healthText != null)
        {
            healthText.text = StrengthPoints + "/" + strengthPointsMax;
        }
        if(staminaText != null)
        {
            staminaText.text = StaminaPoints + "/" + staminaPointsMax;
        }

        //Death, consider moving to damaging method
        if (StrengthPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Alive = false;
        Destroy(gameObject);
        if (Player)
        {
            BM.PlayerLanes[LanePos] = null;
        }
        else
        {
            BM.EnemyLanes[LanePos] = null;
        }
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
        sprite.color = Color.green;
        UnitChosen = true;
    }

    //Debug
    //private void OnMouseDown()
    //{
    //    if (Player)
    //    {
    //        BM.ChooseCharacter(LanePos);
    //        unitChosen = true;
    //    }
    //}
}
