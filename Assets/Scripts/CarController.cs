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
        rayForward = new Ray(transform.position + new Vector3(0, 0, 2.5f), -transform.up);
        rayBackward = new Ray(transform.position + new Vector3(0, 0, -2.5f), -transform.up);
        rayLeft = new Ray(transform.position + new Vector3(-2.5f, 0, 0), -transform.up);
        rayRight = new Ray(transform.position + new Vector3(2.5f, 0, 0), -transform.up);

        if (!Physics.Raycast(rayForward, out RHit, 1.1f))
        {

        }
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

    //private void Acce(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    //{
    //    direction = 1;
    //    if (isGround == false) return;
    //    if(RGas.delta > 0.05f)
    //    {
    //        GiveForce();
    //    }
    //    else
    //    {
    //        direction = 0;
    //        rb.velocity *= 0.9f;
    //    }
    //    TurnAround();
    //}

    //private void GoBack(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    //{
    //    direction = -1;
    //    if (isGround == false) return;
    //    if (LGas.delta > 0.05f)
    //    {
    //        GiveForce();
    //    }
    //    else
    //    {
    //        direction = 0;
    //        rb.velocity *= 0.9f;
    //    }
    //    TurnAround();
    //}

    //private void Jump(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    //{
    //    if (isGround == false) return;
    //    rb.AddForce(transform.up * 1500, ForceMode.Impulse);
    //}

    //private void StartDrift(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    //{
    //    //不在地上或速度小於5就不飄移
    //    if (isGround == false || rb.velocity.z <= 5) return;   
    //    if (turn != 0)
    //    {
    //        isDrifting = true;
    //        TurnAround();
    //        if(turn > 0)
    //        {
    //            DragWay = -1 * (transform.forward * direction) + (transform.right);
    //        }
    //        else
    //        {
    //            DragWay = -1 * (transform.forward * direction) + (-transform.right);
    //        }
    //        rb.AddForce(DragWay * 1500);
    //    }
    //}

    //private void EndDrift(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    //{
    //    isDrifting = false;
    //}

    //private void GiveForce()
    //{
    //    //施力
    //    if (Mathf.Abs(transform.GetComponent<Rigidbody>().velocity.z) < maxspeed)
    //    {
    //        rb.AddForce(transform.forward * maxForce * direction * (right.GetComponent<Control>().accelator() + left.GetComponent<Control>().goback()) * Mathf.Cos(turn));
    //    }
    //    else
    //    {
    //        rb.velocity = transform.forward * direction * maxspeed;
    //    }
    //}

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            maxForce = 15000;
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            Debug.Log("exit ground");
            isGround = false;
            direction = 0;
            rb.AddForce(-transform.up * rb.mass * 1.5f);
            maxForce = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            transform.GetComponent<Rigidbody>().mass = 100;
            if (left.GetComponent<Control>().Drift() && right.transform.localRotation.y != 0)
            {
                isDrifting = true;
            }
        }
    }
}
