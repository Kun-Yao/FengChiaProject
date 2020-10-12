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
    Vector3 DriftWay;
    Rigidbody rb;

    private void Awake()
    {
        RGas.onAxis += Acce;
        LGas.onAxis += GoBack;
        Reset.onStateUp += Relife;
        Drift.onState += StartDrift;
        Drift.onStateUp += EndDrift;
    }

    private void OnDestroy()
    {
        RGas.onAxis -= Acce;
        LGas.onAxis -= GoBack;
        Reset.onStateUp -= Relife;
        Drift.onState -= StartDrift;
        Drift.onStateUp -= EndDrift;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(carevent.canMove == false)
        {
            StartCoroutine(wait());
        }
        checkPoint = transform.position;
        rb = transform.GetComponent<Rigidbody>();
        left = GameObject.Find("Controller (left)");
        right = GameObject.Find("Controller (right)");
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
        print("canMove is " + carevent.canMove);

        if(isGround && carevent.canMove)
        {
            if (isDrifting)
            {
                //StartDrift();
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
        if (Input.GetKeyUp(KeyCode.R)/* || right.GetComponent<Control>().setReset()*/)
        {
            Debug.Log("break");
            Debug.Log(gameObject.name);
            string[] N = gameObject.name.Split('(');
            carevent.ResetCar(N[0], checkPoint);

        }
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


        //轉彎
        turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        if (Mathf.Abs(turn) < 45)
        {
            transform.Rotate(0, turn * direction, 0);
            rb.AddForce(transform.right * (turn / Mathf.Abs(turn)) * Mathf.Abs(rb.velocity.z) * rb.mass);
        }
        transform.Rotate(0, turn * direction, 0);
        rb.AddForce(transform.right * (turn / Mathf.Abs(turn)) * Mathf.Abs(rb.velocity.z) * rb.mass);

        if (left.GetComponent<Control>().Drift())
        {
            //跳躍
            rb.AddForce(transform.up * 1500, ForceMode.Impulse);
        }
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

    void StartDrift(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (left.GetComponent<Control>().unDrift())
        {
            isDrifting = false;
        }
    }

    private void EndDrift(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

    }

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
            if(left.GetComponent<Control>().Drift() && right.transform.localRotation.y != 0)
            {
                isDrifting = true;
            }
        }
    }

    private void Acce(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    {

    }

    private void GoBack(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    {

    }
}
