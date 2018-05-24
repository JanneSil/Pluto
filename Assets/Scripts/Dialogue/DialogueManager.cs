using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public CityManager CM;

	public Text nameText;
    private string firstName;
    private string otherName;
    private string skipToThis;
    private bool OtherSpeaker;
	public Text dialogueText;

	public Animator animator;

    private bool AdvancingPlot;
    private bool exiting;
    private GameController GC;
	private Queue<string> sentences;
    private Queue<string> otherSentences;
    private GameObject nextDialogue;
    private Image blackScreenDialogue;
    private GameObject skipButton;
    private GameObject skipDebugButton;
    private Text skipButtonText;

    private Image ibofangIcon;
    private Image lianneIcon;

    // Use this for initialization
    void Start ()
    {
        Initialize();
    }

    public void Initialize()
    {
        sentences = new Queue<string>();
        otherSentences = new Queue<string>();
        blackScreenDialogue = GameObject.Find("BlackscreenDialogue").GetComponent<Image>();
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        CM = GameObject.Find("GameManager").GetComponent<CityManager>();
        lianneIcon = GameObject.Find("LianneIcon").GetComponent<Image>();
        ibofangIcon = GameObject.Find("IbofangIcon").GetComponent<Image>();
        skipButton = GameObject.Find("SkipButton");
        skipDebugButton = GameObject.Find("SkipDialogueDebug");
        skipButtonText = GameObject.Find("SkipButtonText").GetComponent<Text>();
        skipButton.SetActive(false);
        skipDebugButton.SetActive(false);
    }

	public void StartDialogue (Dialogue dialogue)
	{
        animator.SetBool("IsOpen", true);
        if (skipDebugButton == null)
        {
            skipDebugButton = GameObject.Find("SkipDialogueDebug");
        }
        skipDebugButton.SetActive(true);

        OtherSpeaker = dialogue.AnotherSpeaker;
        if (dialogue.NextDialogue != null)
        {
            nextDialogue = dialogue.NextDialogue;
        }
        else
        {
            nextDialogue = null;
        }

        AdvancingPlot = dialogue.AdvanceGameState;
        skipButtonText.text = dialogue.SkipAction;
        exiting = dialogue.exitingCity;
        nameText.text = dialogue.Name;
        firstName = dialogue.Name;
        GameObject.Find(firstName + "Icon").GetComponent<Image>().enabled = true;
        sentences.Clear();
        otherSentences.Clear();
        skipToThis = dialogue.DialogueToSkipTo;


        foreach (string sentence in dialogue.sentences)
		{
            sentences.Enqueue(sentence);
		}
        if (OtherSpeaker)
        {
            otherName = dialogue.OtherName;
            foreach (string sentence in dialogue.OtherSentences)
            {
                otherSentences.Enqueue(sentence);
            }
        }


        DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
        if (skipToThis != "")
        {
            skipButton.SetActive(true);
        }
        else
        {
            skipButton.SetActive(false);
        }
		if (sentences.Count == 0)
		{
            if (OtherSpeaker && otherSentences.Count != 0)
            {
                string othersentence = otherSentences.Dequeue();
                nameText.text = otherName;
                GameObject.Find(firstName + "Icon").GetComponent<Image>().enabled = false;
                GameObject.Find(otherName + "Icon").GetComponent<Image>().enabled = true;
                ibofangIcon.enabled = true;

                StopAllCoroutines();
                StartCoroutine(TypeSentence(othersentence));
                return;
            }
            else
            {
                if (nextDialogue != null)
                {
                    GameObject.Find(firstName + "Icon").GetComponent<Image>().enabled = false;
                    if (OtherSpeaker)
                    {
                        GameObject.Find(otherName + "Icon").GetComponent<Image>().enabled = false;
                    }
                    StartDialogue(nextDialogue.GetComponent<DialogueTrigger>().dialogue);
                    return;
                }
                else
                {
                    EndDialogue();
                    return;
                }

            }
		}

		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
            yield return null;
		}
	}

	void EndDialogue()
	{
        GameObject.Find(firstName + "Icon").GetComponent<Image>().enabled = false;

        if (OtherSpeaker)
        {
            GameObject.Find(otherName + "Icon").GetComponent<Image>().enabled = false;
        }

        skipDebugButton.SetActive(false);
        animator.SetBool("IsOpen", false);
        blackScreenDialogue.color = Color.clear;
        blackScreenDialogue.gameObject.SetActive(false);

        if (AdvancingPlot)
        {
            GC.GameState += 10;
        }


        if (GC.GameState == 10)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
            GameObject.Find("Ambient").GetComponent<AudioSource>().Play();
            
        }
        if (GC.GameState == 50)
        {
            GameObject.Find("GameStartAnimation").SetActive(false);
            GameObject.Find("GameStartAnimation").SetActive(true);
        }


        if (exiting)
        {
            CM.DialogueUpdate();
        }



    }

    public void Skip()
    {
        GameObject.Find(firstName + "Icon").GetComponent<Image>().enabled = false;
        if (OtherSpeaker)
        {
            GameObject.Find(otherName + "Icon").GetComponent<Image>().enabled = false;
        }
        GameObject.Find(skipToThis).GetComponent<DialogueTrigger>().TriggerDialogue();
    }
    public void SkipDebug()
    {
        Debug.Log("Dialogue Skipped!");
        GameObject.Find(firstName + "Icon").GetComponent<Image>().enabled = false;
        if (OtherSpeaker)
        {
            GameObject.Find(otherName + "Icon").GetComponent<Image>().enabled = false;
        }
        GameObject.Find("Mors6").GetComponent<DialogueTrigger>().TriggerDialogue();
    }

}
