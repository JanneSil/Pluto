using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAnimator : MonoBehaviour
{
    public BattleManager BM;

    public float CameraMargin;
    public float CameraSpeed;
    public float MoveMargin;
    public float MoveSpeed;

    private float cameraDefaultSize;
    private float cameraTargetSize;

    public Vector3 AttackingOffset;

    private Vector3 cameraDefaultPos;
    private Vector3 cameraTargetPos;

    //Unity functions
    void Start()
    {
        BM = GetComponent<BattleManager>();

        cameraDefaultPos = Camera.main.transform.position;
        cameraDefaultSize = Camera.main.orthographicSize;

        //Debug
        cameraTargetPos.z = -10;
        cameraTargetSize = 5;
    }
    void Update()
    {
        cameraUpdate();
    }

    //Camera functions
    public void CameraMove(Vector3 targetPosition, float size)
    {
        float tempX = targetPosition.x;
        float tempY = targetPosition.y + 0.5f;

        //if (tempY < -2.3f)
        //{
        //    tempY = -2.3f;
        //}
        //if (tempX < -4)
        //{
        //    tempX = -4;
        //}
        //if (tempX > 3.7)
        //{
        //    tempX = 3.7f;
        //}

        cameraTargetPos = new Vector3(tempX, tempY, -10);
        cameraTargetSize = size;
    }
    public void CameraReset()
    {
        //Sets camera's target to what it was at Start()
        cameraTargetPos = cameraDefaultPos;
        cameraTargetSize = cameraDefaultSize;
    }

    private void cameraUpdate()
    {
        float prevSize;
        float stepSize;

        Vector3 prevPos;
        Vector3 stepPos;

        //This is to remove overhead from Update(), position and size are not affected if they fall into the margin
        if ((cameraTargetPos - Camera.main.transform.position).magnitude > CameraMargin || Mathf.Abs(cameraTargetSize - Camera.main.orthographicSize) > CameraMargin)
        {
            prevPos = Camera.main.transform.position;
            prevSize = Camera.main.orthographicSize;

            //Interpolates a step between current and target. Adds step to current.
            stepPos = new Vector3((cameraTargetPos.x - prevPos.x) * CameraSpeed * Time.deltaTime, (cameraTargetPos.y - prevPos.y) * CameraSpeed * Time.deltaTime, 0);
            stepSize = (cameraTargetSize - prevSize) * CameraSpeed * Time.deltaTime;

            Camera.main.transform.position += stepPos;
            Camera.main.orthographicSize += stepSize;
        }
    }

    //Character functions
    public void MoveAttack(GameObject agent, GameObject target, float pause, Vector3 Offsett)
    {
        if (agent.GetComponent<Character>().UsingSkill)
        {
            agent.GetComponent<Character>().SetMove(BM.EnemyLanePos[agent.GetComponent<Character>().LanePos] - Offsett, pause, MoveSpeed, MoveMargin);
        }
        //Player character
        else if (agent.GetComponent<Character>().Player)
        {
            agent.GetComponent<Character>().SetMove(BM.EnemyLanePos[target.GetComponent<Character>().LanePos] - Offsett, pause, MoveSpeed, MoveMargin);
        }
        //Enemy character
        else
        {
            agent.GetComponent<Character>().SetMove(BM.PlayerLanePos[target.GetComponent<Character>().LanePos] + Offsett, pause, MoveSpeed, MoveMargin);
        }
    }
}
