using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public Text nameText;
    private string firstName;
    private string otherName;
    private bool OtherSpeaker;
	public Text dialogueText;

	public Animator animator;

    private GameController GC;
	private Queue<string> sentences;
    private Queue<string> otherSentences;
    private GameObject nextDialogue;
    private Image blackScreenDialogue;

    private Image ibofangIcon;
    private Image lianneIcon;

    // Use this for initialization
    void Start () {
		sentences = new Queue<string>();
        otherSentences = new Queue<string>();
        blackScreenDialogue = GameObject.Find("BlackscreenDialogue").GetComponent<Image>();
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        lianneIcon = GameObject.Find("LianneIcon").GetComponent<Image>();
        ibofangIcon = GameObject.Find("IbofangIcon").GetComponent<Image>();
    }

	public void StartDialogue (Dialogue dialogue)
	{
        animator.SetBool("IsOpen", true);

        OtherSpeaker = dialogue.AnotherSpeaker;
        if (dialogue.NextDialogue != null)
        {
            nextDialogue = dialogue.NextDialogue;
        }
        else
        {
            nextDialogue = null;
        }

        nameText.text = dialogue.Name;
        firstName = dialogue.Name;
        GameObject.Find(firstName + "Icon").GetComponent<Image>().enabled = true;
        sentences.Clear();
        otherSentences.Clear();

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
			yield return null;
		}
	}

	void EndDialogue()
	{
		animator.SetBool("IsOpen", false);
        blackScreenDialogue.color = Color.clear;
        blackScreenDialogue.gameObject.SetActive(false);

        if (GC.GameState < 10)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
            GameObject.Find("Ambient").GetComponent<AudioSource>().Play();
        }



    }

}
