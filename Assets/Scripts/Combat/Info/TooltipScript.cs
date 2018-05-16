using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipScript : MonoBehaviour {

    public float offsetX;
    public float offsetY;

    public void SetPosition(GameObject thisObject)
    {
        transform.position = new Vector3(thisObject.transform.position.x + offsetX, thisObject.transform.position.y - offsetY, thisObject.transform.position.z);
        //transform.position = Input.mousePosition + offset;
    }

}
