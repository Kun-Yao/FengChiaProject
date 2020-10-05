using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public GameObject left;
    public GameObject right;

    int direction = 0;
    float maxspeed = 60;
    Vector3 checkPoint;
    float maxForce = 15000;
    float turn = 0;
    Vector3 DriftWay;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        checkPoint = transform.position;
        rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R) || right.GetComponent<Control>().setReset())
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

        ////飄移
        //if (left.GetComponent<Control>().Drift() && turn != 0)
        //{
        //    //持續增加角度並飄移
        //    turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        //    while (Mathf.Abs(rb.velocity.z) > 10 && !left.GetComponent<Control>().unDrift())
        //    {
        //        transform.Rotate(0, turn * direction, 0);
        //        DriftWay = transform.forward * direction + transform.right * (turn / Mathf.Abs(turn));
        //        rb.AddForce(DriftWay * maxForce);
        //    }

        //    //以現在的角度繼續飄移
        //    float angle = turn;
        //    //換方向就停止飄移
        //    while (Mathf.Abs(turn + angle) > Mathf.Abs(angle) && Mathf.Abs(rb.velocity.z) > 10) ;
        //}

        //轉彎
        turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        if(Mathf.Abs(turn) < 45)
        {
            transform.Rotate(0, turn * direction, 0);
            rb.AddForce(transform.right * (turn / Mathf.Abs(turn)) * Mathf.Abs(rb.velocity.z) * rb.mass);
        }
        transform.Rotate(0, turn * direction, 0);
        rb.AddForce(transform.right * (turn / Mathf.Abs(turn)) * Mathf.Abs(rb.velocity.z) * rb.mass);

        //施力
        if (Mathf.Abs(transform.GetComponent<Rigidbody>().velocity.z) < maxspeed)
        {
            rb.AddForce(transform.forward * maxForce * direction * (right.GetComponent<Control>().accelator()+left.GetComponent<Control>().goback()) * Mathf.Cos(turn));
        }
        else
        {
            rb.velocity = transform.forward * direction * maxspeed;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            transform.GetComponent<Rigidbody>().mass = 500;
            Move();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            Debug.Log("exit ground");
            direction = 0;
            rb.velocity *= 0.93f;
            rb.mass = Mathf.Infinity;
            maxForce = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            transform.GetComponent<Rigidbody>().mass = 500;
            Move();
        }
    }
}
