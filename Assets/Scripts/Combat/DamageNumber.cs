using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    //If damage numbers cause slowdown when they load, consider object pooling.
    public float FadingSpeed;
    public float FloatingSpeed;
    public float TimeUntilDestroy;

    private Outline outl;

    private Text text;

    private Vector4 nextColorOutline;
    private Vector4 nextColorText;
    private Vector4 previousColorOutline;
    private Vector4 previousColorText;

    private void Start()
    {
        text = GetComponent<Text>();
        outl = GetComponent<Outline>();

        Destroy(this.gameObject, TimeUntilDestroy);
    }

    private void Update()
    {
        //Fades colors
        previousColorOutline = outl.effectColor;
        previousColorText = text.color;
        nextColorOutline = previousColorOutline - new Vector4(0, 0, 0, FadingSpeed * Time.deltaTime);
        nextColorText = previousColorText - new Vector4(0, 0, 0, FadingSpeed * Time.deltaTime);

        outl.effectColor = nextColorOutline;
        text.color = nextColorText;

        //Moves text along y-axis
        gameObject.transform.Translate(0, FloatingSpeed * Time.deltaTime, 0, Space.Self);
    }
}
