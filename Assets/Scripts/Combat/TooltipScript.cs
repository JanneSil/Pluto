using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipScript : MonoBehaviour {

	
    public void SetPosition(GameObject thisObject)
    {
        transform.position = new Vector3(thisObject.transform.position.x + 30, thisObject.transform.position.y - 30, thisObject.transform.position.z);
    }

}
