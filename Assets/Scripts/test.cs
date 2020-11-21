using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class test : MonoBehaviour
{
    private SteamVR_Action_Boolean Reset = SteamVR_Input.GetBooleanAction("Reset");

    GameManager GM;
    GameObject left;
    GameObject right;
    Vector3 relifePoint;
    Vector3 H_Direction;
    Vector3 localVelocity;
    Vector3 forceDir;
    Vector3 Force;
    Vector3 DragForce;
    Rigidbody rb;

    float maxForce = 3000;
    float tempForce;
    float RightRotationY;
    float LeftRotationY;
    float tempAngle;
    float turn;
    float maxSpeed = 150;
    bool isGround;
    bool isDrifting;
    int VectorZ;

    private void Awake()
    {
        GM = FindObjectOfType<GameManager>();
        Reset.onStateUp += Relife;
    }

    private void OnDestroy()
    {
        Reset.onStateUp -= Relife;
    }
    private void Start()
    {
        if (!GM.canMove)
            StartCoroutine(Wait());

        tempForce = maxForce * VectorZ;
        relifePoint = transform.position;
        rb = transform.GetComponent<Rigidbody>();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        startRace();
    }

    void startRace()
    {
        GM.canMove = true;
    }

    private void Update()
    {
        if (left.GetComponent<Control>().Jump())
        {
            if (isGround)
            {
                Jump();
            }
        }

        if (left.GetComponent<Control>().Drift() && turn != 0)
        {
            if (isGround && !isDrifting && localVelocity.z > 20)
            {
                StartDrift();
            }
        }
        if (left.GetComponent<Control>().unDrift())
        {
            if (isDrifting)
            {
                StopDrift();
            }
        }
    }

    private void FixedUpdate()
    {
        if (left == null)
        {
            print("left is null");
            left = GameObject.Find("Controller (left)");
        }

        if (right == null)
        {
            print("right is null");
            right = GameObject.Find("Controller (right)");
        }

        if (GM == null)
        {
            print("GM is null");
            GM = FindObjectOfType<GameManager>();
        }

        CheckGroundNormal();
        if (isGround == false) return;

        TurnAround();

        CalculateForceDir();

        GiveForce();
    }

    void CheckGroundNormal()
    {
        //在車底的四個方向設置laser
        RaycastHit frontHit;
        RaycastHit rearHit;
        RaycastHit rightHit;
        RaycastHit leftHit;

        //laser是否接觸地面
        Physics.Raycast(transform.position + new Vector3(0, 0, 1f), -transform.up, out frontHit);
        Physics.Raycast(transform.position + new Vector3(0, 0, -1f), -transform.up, out rearHit);
        Physics.Raycast(transform.position + new Vector3(1, 0, 0), -transform.up, out rightHit);
        Physics.Raycast(transform.position + new Vector3(-1, 0, 0), -transform.up, out leftHit);

        if (frontHit.distance < 1.1 || rearHit.distance < 1.1 || rightHit.distance < 1.1 || leftHit.distance < 1.1)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
            VectorZ = 0;
        }

        float deltaZ = frontHit.distance - rearHit.distance;
        float deltaX = rightHit.distance - leftHit.distance;
        transform.Rotate(Mathf.Rad2Deg * Mathf.Atan(deltaZ / 2), 0, Mathf.Rad2Deg * Mathf.Atan(deltaX / 2) * 180 / Mathf.PI);
    }

    void TurnAround()
    {
        if (right.GetComponent<Control>().accelator() < 0.1 && left.GetComponent<Control>().goback() < 0.1) return;

        RightRotationY = checkAngle(right.transform.localEulerAngles.y);
        LeftRotationY = checkAngle(left.transform.localEulerAngles.y);
        tempAngle = (RightRotationY + LeftRotationY) / 2 / 100;
        turn += tempAngle;

        if (turn > 0.05)
            H_Direction = transform.right;
        else if (turn < -0.05)
            H_Direction = -transform.right;
        else
            H_Direction = new Vector3(0, 0, 0);

        print("turn = " + turn);
        transform.localRotation = Quaternion.Euler(0, turn, 0);
    }

    float checkAngle(float angle)
    {
        float finalAngle = angle - 180;
        if (finalAngle > 0)
        {
            return finalAngle - 180;
        }
        else
        {
            return finalAngle + 180;
        }
    }

    void CalculateForceDir()
    {
        localVelocity = transform.InverseTransformDirection(rb.velocity);

        if (right.GetComponent<Control>().accelator() > 0.1)
            VectorZ = 1;
        else if (left.GetComponent<Control>().goback() > 0.1)
            VectorZ = -1;
        else
        {
            VectorZ = 0;
            rb.velocity *= 0.98f;
        }

        forceDir = transform.forward * VectorZ * Mathf.Cos(turn);
    }

    void GiveForce()
    {
        localVelocity = transform.InverseTransformDirection(rb.velocity);

        if (localVelocity.z > maxSpeed)
            tempForce = 0;
        else
            tempForce = maxForce;

        Force = tempForce * (right.GetComponent<Control>().accelator() - left.GetComponent<Control>().goback()) * transform.forward * VectorZ;
        if (!isGround)
            Force = rb.mass * 9.8f * (-transform.up);
        print("go");
        rb.AddForce(Force);

        //localVelocity.y = 0;
        //DragForce = transform.TransformVector(localVelocity);

        //if (isDrifting)
        //    rb.AddForce(-DragForce / 5 * rb.mass);
        //else
        //    rb.AddForce(-DragForce * 50 * rb.mass);
    }

    void Jump()
    {
        rb.AddForce(transform.up * rb.mass * 9.8f);
    }

    void StartDrift()
    {
        isDrifting = true;
    }

    void StopDrift()
    {
        isDrifting = false;
    }

    private void Relife(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!GM.canMove) return;
        Debug.Log("break");
        Debug.Log(gameObject.name);
        string[] N = gameObject.name.Split('(');
        //ResetCar(車名, 生成位置, 車頭方向);
        GM.ResetCar(N[0]);
    }
}