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

        //if (GC.GameState == 0)
        //{
        //    GC.GameState = 10;
        //    ClickableClick(lianne);
        //}

    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetButtonDown("Fire1") && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {

            if (Physics.Raycast(ray, out hit, 1000))
            {

                if (hit.collider.tag == "Clickable" && !InDialogue)
                {
                    //storedClickable = hit.collider.gameObject;
                    ClickableClick(hit.collider.gameObject);
                }

            }

            else
            {
                Debug.Log("Nothing Clicked ");
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
