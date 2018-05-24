using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CityManager : MonoBehaviour
{

    private GameObject exit;
    //private GameObject storedClickable;
    private GameObject lianne;
    private bool leavingCity;
    private int lianneStage;

    [HideInInspector]
    public Text DialogueText;
    public bool InDialogue;
    public GameObject DialogueBox;
    private GameObject mapUI;

    private GameController GC;
    private DialogueTrigger DT;

    private AudioSource AS;
    public AudioClip ClickSound;
    public AudioClip FootSteps;

    private float gameStart = 0f;
    private float gameFade = 16f;
    private bool gameStarting;
    private bool fading;
    private float fadingVariable = 0f;
    private float fadingDuration = 4f;

    private Image blackScreen;
    private Image blackScreenDialogue;

    private float musicStart = 0f;
    private float musicFade = 2f;
    private bool startingMusic;

    private SpriteRenderer citySelectionFatDude;
    private SpriteRenderer citySelectionLianne;
    private SpriteRenderer citySelectionIbofang;
    private SpriteRenderer citySelectionStore;
    private SpriteRenderer citySelectionTavern;

    private GameObject tempObject;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        exit = GameObject.Find("CityExit");
        //DialogueBox = GameObject.Find("OldDialogue");
        //DialogueText = DialogueBox.GetComponentInChildren<Text>();
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        //DialogueText.text = "";
        exit.SetActive(false);
        lianne = GameObject.Find("Lianne");
        //storedClickable = lianne;
        //DialogueBox.SetActive(false);
        AS = GameObject.Find("Music").GetComponent<AudioSource>();
        //mapUI = GameObject.Find("MapUI");
        //mapUI.SetActive(false);

        musicStart = 0f;
        musicFade = 2f;
        startingMusic = false;

        citySelectionFatDude = GameObject.Find("City_Selection_Fatdude").GetComponent<SpriteRenderer>();
        citySelectionLianne = GameObject.Find("City_Selection_Lianne").GetComponent<SpriteRenderer>();
        citySelectionIbofang = GameObject.Find("City_Selection_ibo").GetComponent<SpriteRenderer>();
        citySelectionStore = GameObject.Find("City_Selection_Store").GetComponent<SpriteRenderer>();
        citySelectionTavern = GameObject.Find("City_Selection_Tavern").GetComponent<SpriteRenderer>();
        citySelectionFatDude.enabled = false;
        citySelectionLianne.enabled = false;
        citySelectionIbofang.enabled = false;
        citySelectionStore.enabled = false;
        citySelectionTavern.enabled = false;

        blackScreen = GameObject.Find("Blackscreen").GetComponent<Image>();
        blackScreenDialogue = GameObject.Find("BlackscreenDialogue").GetComponent<Image>();
        blackScreenDialogue.enabled = false;

        AS.PlayOneShot(FootSteps);

        if (GC.GameState < 10)
        {
            blackScreen.enabled = true;
            blackScreenDialogue.enabled = true;
        }
        Debug.Log("GameState is: " + GC.GameState);

    }

    void Update()
    {
        if (!gameStarting && GC.GameState < 10)
        {
            if (gameStart < 1)
            {
                gameStart += Time.deltaTime / gameFade;
            }
            else
            {
                AS.Stop();
                fading = true;
                gameStarting = true;
                GC.GameState += 10;
                lianneStage += 1;
                blackScreen.gameObject.SetActive(false);
                DT = GameObject.Find("Lianne" + lianneStage).GetComponent<DialogueTrigger>();
                DT.TriggerDialogue();
            }
        }

        if (!startingMusic)
        {
            if (GC.GameState == 20)
            {
                if (musicStart < 1)
                {
                    musicStart += Time.deltaTime / musicFade;
                }
                else
                {
                    GameObject.Find("Stultus1").GetComponent<DialogueTrigger>().TriggerDialogue();
                    startingMusic = true;
                }
            }

            else if (GC.GameState == 40)
            {
                if (musicStart < 1)
                {
                    musicStart += Time.deltaTime / musicFade;
                }
                else
                {
                    GameObject.Find("DialogueAfterCombat1").GetComponent<DialogueTrigger>().TriggerDialogue();
                    startingMusic = true;
                }
            }

            else if (GC.GameState == 50)
            {
                if (musicStart < 1)
                {
                    musicStart += Time.deltaTime / musicFade;
                }
                else
                {
                    GameObject.Find("DialogueAfterCombat2").GetComponent<DialogueTrigger>().TriggerDialogue();
                    startingMusic = true;
                }
            }
        }



        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetButtonDown("Fire1") && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {

            if (Physics.Raycast(ray, out hit, 1000))
            {

                if (hit.collider.gameObject.GetComponent<ClickableInfo>() && !InDialogue)
                {
                    AS.PlayOneShot(ClickSound);
                    ClickableClick(hit.collider.gameObject);
                }

            }

            else
            {
                //Debug.Log("Nothing Clicked ");
            }
        }


        if (Physics.Raycast(ray, out hit, 5000) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (tempObject != null)
            {
                if (hit.collider.gameObject != tempObject)
                {
                    ResetSelect();
                    tempObject = null;
                    return;
                }
                else
                {
                    return;
                }

            }

            if (hit.collider.tag == "FatDudeHouse" && !InDialogue)
            {
                tempObject = hit.collider.gameObject;
                citySelectionFatDude.enabled = true;
            }
            else if (hit.collider.tag == "IbofangHouse" && !InDialogue)
            {
                tempObject = hit.collider.gameObject;
                citySelectionIbofang.enabled = true;
            }
            else if (hit.collider.tag == "LianneHouse" && !InDialogue)
            {
                tempObject = hit.collider.gameObject;
                citySelectionLianne.enabled = true;
            }
            else if (hit.collider.tag == "Store" && !InDialogue)
            {
                tempObject = hit.collider.gameObject;
                citySelectionStore.enabled = true;
            }
            else if (hit.collider.tag == "Tavern" && !InDialogue)
            {
                tempObject = hit.collider.gameObject;
                citySelectionTavern.enabled = true;
            }
            else
            {
                if (tempObject == null)
                {
                    return;
                }
                ResetSelect();
                tempObject = null;
            }

        }


    }

    private void ResetSelect()
    {
        citySelectionFatDude.enabled = false;
        citySelectionLianne.enabled = false;
        citySelectionIbofang.enabled = false;
        citySelectionStore.enabled = false;
        citySelectionTavern.enabled = false;
    }
    private void ClickableClick(GameObject clickable)
    {
        //InDialogue = true;

        //Debug.Log(clickable.GetComponent<ClickableInfo>().ClickableType);

        if (clickable.GetComponent<ClickableInfo>().ClickableType == "Exit")
        {
            ResetSelect();
            //mapUI.SetActive(true);
            leavingCity = true;
            //DialogueBox.SetActive(true);
            exit.SetActive(true);
        }
        else if (clickable.GetComponent<ClickableInfo>().ClickableType == "Lianne")
        {
            if (GC.GameState < 10)
            {
                ResetSelect();
                GC.GameState += 10;
                lianneStage += 1;
                DT = GameObject.Find("Lianne" + lianneStage).GetComponent<DialogueTrigger>();
                DT.TriggerDialogue();
            }
            else if (GC.GameState == 30)
            {
                ResetSelect();
                DT = GameObject.Find("LianneSecond1").GetComponent<DialogueTrigger>();
                DT.TriggerDialogue();
            }
            else
            {
                return;
            }
        }
        else if (clickable.GetComponent<ClickableInfo>().ClickableType == "Stultus")
        {
            if (GC.GameState == 20)
            {
                ResetSelect();
                GC.GameState += 10;
                DT = GameObject.Find("StultusSecond1").GetComponent<DialogueTrigger>();
                DT.TriggerDialogue();
            }
            else
            {
                return;
            }

        }
        else
        {
            Debug.Log("Nothing");
        }
    }


    public void DialogueUpdate()
    {
        if (leavingCity && GC.GameState == 10)
        {
            leavingCity = false;
            GC.FadeToLevel(1);
        }
        if (leavingCity && GC.GameState > 10 && GC.GameState < 40)
        {
            leavingCity = false;
            GC.FadeToLevel(2);
        }
        if (leavingCity && GC.GameState == 40)
        {
            leavingCity = false;
            GC.FadeToLevel(3);
        }
        else
        {
            Debug.Log("Not Leaving");
        }

    }

    public void ExitCity(bool exited)
    {
        if (exited)
        {
            //mapUI.SetActive(true);
            leavingCity = true;
            //DialogueBox.SetActive(true);
            //DialogueText.text = "Prepare for combat.";
            exit.SetActive(false);

            if (GC.GameState == 10)
            {
                blackScreenDialogue.gameObject.SetActive(true);
                blackScreenDialogue.enabled = true;
                blackScreenDialogue.color = Color.black;
                DT = GameObject.Find("ExitDialogue1").GetComponent<DialogueTrigger>();
                DT.TriggerDialogue();
            }
            else
            {
                DialogueUpdate();
            }
        }
        else
        {
            //mapUI.SetActive(false);
            leavingCity = false;
            //DialogueBox.SetActive(false);
            exit.SetActive(false);
            InDialogue = false;
        }
    }

}
