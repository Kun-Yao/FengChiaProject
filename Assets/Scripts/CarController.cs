using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CarController : MonoBehaviour
{
    private SteamVR_Action_Single RGas = SteamVR_Input.GetSingleAction("RPull");
    private SteamVR_Action_Single LGas = SteamVR_Input.GetSingleAction("LPull");
    private SteamVR_Action_Boolean Reset = SteamVR_Input.GetBooleanAction("Reset");
    private SteamVR_Action_Boolean Drift = SteamVR_Input.GetBooleanAction("Drift");

    GameObject left;
    GameObject right;

    int direction = 0;
    float maxspeed = 60;
    float maxForce = 15000;
    float turn = 0;
    bool isGround = false;
    bool isDrifting = false;

    Vector3 checkPoint;
    Vector3 DragWay;
    Rigidbody rb;

    Ray rayForward;
    Ray rayBackward;
    Ray rayRight;
    Ray rayLeft;
    RaycastHit RHit;

    private void Awake()
    {
        //RGas.onAxis += Acce;
        //LGas.onAxis += GoBack;
        Reset.onStateUp += Relife;
        //Drift.onStateDown += Jump;
        //Drift.onState += StartDrift;
        //Drift.onStateUp += EndDrift;
    }

    private void OnDestroy()
    {
        //RGas.onAxis -= Acce;
        //LGas.onAxis -= GoBack;
        Reset.onStateUp -= Relife;
        //Drift.onStateDown -= Jump;
        //Drift.onState -= StartDrift;
        //Drift.onStateUp -= EndDrift;
    }

// Start is called before the first frame update
    void Start()
    {
        print("inStart");
        if(carevent.canMove == false)
        {
            StartCoroutine(wait());
        }
        checkPoint = transform.position;
        rb = transform.GetComponent<Rigidbody>();
        left = transform.GetChild(0).transform.GetChild(0).gameObject;
        right = transform.GetChild(0).transform.GetChild(1).gameObject;
        if(right == null)
        {
            print("isNull");
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(3);
        print("end");
        startRace();
    }

    void startRace()
    {
        carevent.canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundNormal();
        if (isGround && carevent.canMove)
        {
            if (isDrifting)
            {
                StartDrift();
            }
            else
            {
                Move();
            }
        }
        print(right.name);

    }

    private void Relife(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("break");
        Debug.Log(gameObject.name);
        string[] N = gameObject.name.Split('(');
        carevent.ResetCar(N[0], checkPoint);
    }
    private void Move()
    {
        //前進/後退
        if (right.GetComponent<Control>().accelator() > 0)
        {
            direction = 1;
        }
        else if (left.GetComponent<Control>().goback() > 0)
        {
            direction = -1;
        }
        else
        {
            direction = 0;
            rb.velocity *= 0.9f;
        }

        TurnAround();

        if (left.GetComponent<Control>().Drift())
        {
            //跳躍
            rb.AddForce(transform.up * 150, ForceMode.Impulse);
        }

        GiveForce();
    }

    private void GiveForce()
    {
        //施力
        if (Mathf.Abs(transform.GetComponent<Rigidbody>().velocity.z) < maxspeed)
        {
            rb.AddForce(transform.forward * maxForce * direction * (right.GetComponent<Control>().accelator() + left.GetComponent<Control>().goback()) * Mathf.Cos(turn));
        }
        else
        {
            rb.velocity = transform.forward * direction * maxspeed;
        }
    }
    private void StartDrift()
    {
        //不在地上或速度小於5就不飄移
        if (isGround == false || rb.velocity.z <= 5) return;
        if (turn != 0)
        {
            isDrifting = true;
            TurnAround();
            //if (turn > 0)
            //{
            //    DragWay = -1 * (transform.forward * direction) + (transform.right);
            //}
            //else
            //{
            //    DragWay = -1 * (transform.forward * direction) + (-transform.right);
            //}
            //rb.AddForce(DragWay * 1500);
        }
    }

    private void TurnAround()
    {
        //轉彎
        turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        if(Mathf.Abs(turn) > 45)
        {
            turn = 45 * turn / Mathf.Abs(turn);
        }
        transform.Rotate(0, turn * direction, 0);

        if (!isDrifting)
        {
            rb.AddForce(transform.right * (turn / Mathf.Abs(turn)) * Mathf.Abs(rb.velocity.z) * rb.mass);
        }
    }

    //偵測是否在地上
    public void CheckGroundNormal()
    {
        Quaternion VStream = transform.GetComponent<Rigidbody>().rotation;
        Quaternion HStream = transform.GetComponent<Rigidbody>().rotation;

        RaycastHit frontHit;
        RaycastHit rearHit;
        RaycastHit rightHit;
        RaycastHit leftHit;

        bool hasfront = Physics.Raycast(transform.position + new Vector3(0, 0, 2), -transform.up, out frontHit, 1.1f);
        bool hasrear = Physics.Raycast(transform.position + new Vector3(0, 0, -2), -transform.up, out rearHit, 1.1f);
        bool hasright = Physics.Raycast(transform.position + new Vector3(1, 0, 0), -transform.up, out rightHit, 1.1f);
        bool hasleft = Physics.Raycast(transform.position + new Vector3(-1, 0, 0), -transform.up, out leftHit, 1.1f);

        print(frontHit.transform.gameObject.name);
        if (hasfront || hasrear || hasright || hasleft)
            isGround = true;
        //Debug.DrawLine(transform.position + new Vector3(0, 0, 2), frontHit.point);
        else
            isGround = false;
        //Debug.Log("no no no");

        //垂直方向與地面水平
        Vector3 VNormal = (frontHit.normal + rearHit.normal).normalized;
        Quaternion VQuaternion = Quaternion.FromToRotation(transform.up, VNormal);
        Vector3 HNormal = (frontHit.normal + rearHit.normal).normalized;
        Quaternion HQuaternion = Quaternion.FromToRotation(transform.up, HNormal);

        //水平方向與地面水平
        VStream = VQuaternion * VStream;
        transform.GetComponent<Rigidbody>().MoveRotation(VStream);
        HStream = HQuaternion * HStream;
        transform.GetComponent<Rigidbody>().MoveRotation(HStream);
    }
}
