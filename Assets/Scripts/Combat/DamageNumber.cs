using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    //If damage numbers cause slowdown when they load, consider object pooling
    public float FloatingSpeed;
    public float TimeUntilDestroy;

    private void Start()
    {
        Destroy(this.gameObject, TimeUntilDestroy);
    }

    private void Update()
    {
        gameObject.transform.Translate(0, FloatingSpeed * Time.deltaTime, 0);
    }
}
