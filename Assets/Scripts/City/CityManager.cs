using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CityManager : MonoBehaviour
{

    private GameObject exit;
    private GameObject storedClickable;
    private bool leavingCity;

    [HideInInspector]
    public Text DialogueText;
    public bool InDialogue;
    public GameObject DialogueBox;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        exit = GameObject.Find("CityExit");
        DialogueBox = GameObject.Find("Dialogue");
        DialogueText = DialogueBox.GetComponentInChildren<Text>();
        DialogueText.text = "";
        DialogueBox.SetActive(false);
        exit.SetActive(false);
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
                    storedClickable = hit.collider.gameObject;
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
        InDialogue = true;

        if (clickable.GetComponent<ClickableInfo>().ClickableType == "Exit")
        {
            exit.SetActive(true);
        }
        else if (clickable.GetComponent<ClickableInfo>().ClickableType == "Lianne")
        {
            DialogueBox.SetActive(true);
            DialogueText.text = "You find Lianne.";
        }
    }


    public void DialogueUpdate()
    {
        if (storedClickable.GetComponent<ClickableInfo>().ClickableType == "Lianne")
        {
            storedClickable.GetComponent<LianneDialogue>().UpdateDialogue();
        }
        if (leavingCity)
        {
            leavingCity = false;
            StartCoroutine(LoadScene("Main"));
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
