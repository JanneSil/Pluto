using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LianneDialogue : MonoBehaviour {

    public int LianneQuestStage = 10;

    private CityManager MC;

    private void Start()
    {
        MC = GameObject.Find("GameManager").GetComponent<CityManager>();
    }

    public void UpdateDialogue()
    {
        if (LianneQuestStage == 10)
        {
            MC.DialogueText.text = "Lianne: Hey.";
            LianneQuestStage = 20;
            return;
        }
        if (LianneQuestStage == 20)
        {
            MC.DialogueText.text = "Lianne: Hey..";
            LianneQuestStage = 30;
            return;
        }
        if (LianneQuestStage == 30)
        {
            MC.DialogueText.text = "Lianne: Hey...";
            LianneQuestStage = 40;
            return;
        }
        if (LianneQuestStage == 40)
        {
            MC.DialogueText.text = "...";
            MC.DialogueBox.SetActive(false);
            MC.InDialogue = false;
            MC.DialogueText.text = "";
            return;
        }
    }

}
