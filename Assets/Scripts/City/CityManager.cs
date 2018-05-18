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

    private GameController GC;
    private DialogueTrigger DT;

    private AudioSource AS;
    public AudioClip ClickSound;
    public AudioClip FootSteps;

    private float gameStart = 0f;
    private float gameFade = 4f;
    private bool gameStarting;
    private bool fading;
    private float fadingVariable = 0f;
    private float fadingDuration = 4f;

    private Image blackScreen;
    private Image blackScreenDialogue;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        exit = GameObject.Find("CityExit");
        DialogueBox = GameObject.Find("OldDialogue");
        DialogueText = DialogueBox.GetComponentInChildren<Text>();
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        DialogueText.text = "";
        exit.SetActive(false);
        lianne = GameObject.Find("Lianne");
        //storedClickable = lianne;
        DialogueBox.SetActive(false);
        AS = GameObject.Find("Music").GetComponent<AudioSource>();

        blackScreen = GameObject.Find("Blackscreen").GetComponent<Image>();
        blackScreenDialogue = GameObject.Find("BlackscreenDialogue").GetComponent<Image>();

        AS.PlayOneShot(FootSteps);


        if (GC.GameState < 10)
        {
            blackScreen.enabled = true;
            blackScreenDialogue.enabled = true;
        }

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

                //    //blackScreen.color = Color.clear;
                //    if (!fading)
                //    {
                //        AS.Stop();

                //       blackScreen.color = Color.Lerp(Color.black, Color.clear, fadingVariable);


                //        if (fadingVariable < 1)
                //        {
                //            fadingVariable += Time.deltaTime / fadingDuration;
                //        }
                //        else
                //        {
                //            fading = true;
                //            gameStarting = true;
                //            GC.GameState += 10;
                //            lianneStage += 1;
                //            blackScreen.gameObject.SetActive(false);
                //            DT = GameObject.Find("Lianne" + lianneStage).GetComponent<DialogueTrigger>();
                //            DT.TriggerDialogue();
                //        }

                //    }
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetButtonDown("Fire1") && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {

            if (Physics.Raycast(ray, out hit, 1000))
            {

                if (hit.collider.tag == "Clickable" && !InDialogue)
                {
                    AS.PlayOneShot(ClickSound);
                    //storedClickable = hit.collider.gameObject;
                    ClickableClick(hit.collider.gameObject);
                }

            }

            else
            {
                //Debug.Log("Nothing Clicked ");
            }
        }

    }

    private void ClickableClick(GameObject clickable)
    {
        //InDialogue = true;

        //Debug.Log(clickable.GetComponent<ClickableInfo>().ClickableType);

        if (clickable.GetComponent<ClickableInfo>().ClickableType == "Exit")
        {
            exit.SetActive(true);
        }
        else if (clickable.GetComponent<ClickableInfo>().ClickableType == "Lianne")
        {
            if (GC.GameState > 10)
            {
                return;
            }
            GC.GameState += 10;
            lianneStage += 1;
            DT = GameObject.Find("Lianne"+ lianneStage).GetComponent<DialogueTrigger>();
            DT.TriggerDialogue();
        }
    }


    public void DialogueUpdate()
    {
        if (leavingCity)
        {
            leavingCity = false;
            StartCoroutine(LoadScene("CombatScene"));
        }

    }

    public void ExitCity(bool exited)
    {
        if (exited)
        {
            leavingCity = true;
            DialogueBox.SetActive(true);
            DialogueText.text = "Prepare for combat.";
            exit.SetActive(false);
        }
        else
        {
            exit.SetActive(false);
            InDialogue = false;
        }
    }

    IEnumerator LoadScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}
