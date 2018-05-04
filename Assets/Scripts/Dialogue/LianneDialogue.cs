using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LianneDialogue : MonoBehaviour {

    private CityManager MC;
    private GameController GC;

    private void Start()
    {
        MC = GameObject.Find("GameManager").GetComponent<CityManager>();
        GC = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void UpdateDialogue()
    {
        if (GC.GameState == 10)
        {
            MC.DialogueText.text = "Lianne: Ibofang, my greatest warrior, my oldest son. I cherish the moments we meet." + 
                " Your creator, your only loving god, your only god, asks of you. No… Orders you to kill once more. The madness has breached my borders. They are making my land and my people sick.";
            GC.GameState = 20;
            return;
        }
        if (GC.GameState == 20)
        {
            MC.DialogueText.text = "Ibofang: Ok :P";
            GC.GameState = 30;
            return;
        }
        if (GC.GameState == 30)
        {
            MC.DialogueText.text = "...";
            MC.DialogueBox.SetActive(false);
            MC.InDialogue = false;
            MC.DialogueText.text = "";
            return;
        }
        if (GC.GameState == 40)
        {
            MC.DialogueText.text = "...";
            MC.DialogueBox.SetActive(false);
            MC.InDialogue = false;
            MC.DialogueText.text = "";
            return;
        }
    }

}
