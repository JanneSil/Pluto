using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAnimator : MonoBehaviour
{
    public BattleManager BM;

    public bool CameraMoving;

    public float CameraSpeed;
    public float CameraTargetSize;

    private float cameraDefaultSize;
    private float cameraMargin = 0.01f;

    public Vector3 CameraTargetPosition;

    private Vector3 cameraDefaultPos;

    //Unity functions
    void Start()
    {
        BM = GetComponent<BattleManager>();

        cameraDefaultPos = Camera.main.transform.position;
        cameraDefaultSize = Camera.main.orthographicSize;

        //Debug
        CameraTargetPosition.z = -10;
        CameraTargetSize = 5;
    }
    void Update()
    {
        CameraUpdate();
    }

    //Camera functions
    public void CameraMove(Vector3 targetPosition, float size)
    {
        CameraTargetPosition = new Vector3(targetPosition.x, targetPosition.y, -10);
        CameraTargetSize = size;

        BM.ActionDelayRemaining += 3;
    }
    public void CameraReset()
    {
        //Sets camera's target to what it was at Start()
        CameraTargetPosition = cameraDefaultPos;
        CameraTargetSize = cameraDefaultSize;
    }
    public void CameraUpdate()
    {
        float prevSize;
        float stepSize;

        Vector3 prevPos;
        Vector3 stepPos;

            Debug.Log("" + (CameraTargetPosition - Camera.main.transform.position).magnitude);

        //This is to remove overhead from Update(), position and size are not affected if they fall into the margin
        if ((CameraTargetPosition - Camera.main.transform.position).magnitude > cameraMargin || Mathf.Abs(CameraTargetSize - Camera.main.orthographicSize) > cameraMargin)
        {
            CameraMoving = true;

            prevPos = Camera.main.transform.position;
            prevSize = Camera.main.orthographicSize;

            //Interpolates a step between current and target. Adds step to current.
            stepPos = new Vector3((CameraTargetPosition.x - prevPos.x) * CameraSpeed * Time.deltaTime, (CameraTargetPosition.y - prevPos.y) * CameraSpeed * Time.deltaTime, 0);
            stepSize = (CameraTargetSize - prevSize) * CameraSpeed * Time.deltaTime;

            Camera.main.transform.position += stepPos;
            Camera.main.orthographicSize += stepSize;
        }
        else
        {
            CameraMoving = false;
        }
    }

    //Character functions
    public void CharAttak(GameObject agent, GameObject target, bool player)
    {

    }
    public void CharMove(GameObject agent, int targetLane)
    {

    }
    public void CharMove(GameObject agent, GameObject secondAgent)
    {

    }
}
