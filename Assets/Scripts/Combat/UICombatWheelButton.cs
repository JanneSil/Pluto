using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICombatWheelButton : MonoBehaviour {

    public float AlphaThreshold = 0.1f;

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
    }
}
