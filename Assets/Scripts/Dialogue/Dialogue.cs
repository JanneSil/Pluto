using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

	public string Name;
    public string OtherName;
    public string DialogueToSkipTo;
    public string SkipAction;
    public bool AnotherSpeaker;
    public bool exitingCity;
    public bool AdvanceGameState;
    public GameObject NextDialogue;

	[TextArea(3, 10)]
	public string[] sentences;

    [TextArea(3, 10)]
    public string[] OtherSentences;

}
