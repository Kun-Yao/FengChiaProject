using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public GameObject left;
    public GameObject right;

    int direction = 0;
    float maxspeed = 300;
    Vector3 checkPoint;
    float maxForce = 0;
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
        //transform.GetComponent<Rigidbody>().AddForce(transform.forward * maxForce);
        ////移動
        //if (Input.GetKey(KeyCode.UpArrow) || right.GetComponent<Control>().accelator() > 0)
        //{
        //    direction = 1;
        //    if (transform.GetComponent<Rigidbody>().velocity.magnitude < maxspeed)
        //    {
        //        maxForce = 15000 * right.GetComponent<Control>().accelator();
        //        print(right.GetComponent<Control>().accelator());
        //    }
        //    else
        //    {
        //        transform.GetComponent<Rigidbody>().velocity = transform.forward * direction * maxspeed;
        //    }
        //}
        //else if (Input.GetKey(KeyCode.DownArrow) || left.GetComponent<Control>().goback() > 0)
        //{
        //    direction = -1;
        //    if (transform.GetComponent<Rigidbody>().velocity.magnitude < maxspeed)
        //    {
        //        maxForce = -15000 * left.GetComponent<Control>().goback();
        //    }
        //    else
        //    {
        //        transform.GetComponent<Rigidbody>().velocity = transform.forward * direction * maxspeed;
        //    }
        //}
        //else
        //{
        //    direction = 0;
        //    transform.GetComponent<Rigidbody>().velocity *= 0.93f;
        //    maxForce = 0;
        //}

        ////轉彎
        //if (Input.GetKey(KeyCode.RightArrow) || right.transform.localRotation.y > 0)
        //{
        //    Debug.Log("右轉");
        //    float angle = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        //    transform.Rotate(0, angle * direction, 0);
        //}
        //else if (Input.GetKey(KeyCode.LeftArrow) || right.transform.localRotation.y < 0)
        //{
        //    Debug.Log("左轉");
        //    float angle = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        //    transform.Rotate(0, angle * direction, 0);
        //}

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

        //飄移
        if (Input.GetKeyDown(KeyCode.LeftControl) && turn != 0)
        {
            //持續增加角度並飄移
            turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
            while(Mathf.Abs(rb.velocity.z) > 10 && !Input.GetKeyUp(KeyCode.LeftControl))
            {
                transform.Rotate(0, turn * direction, 0);
                DriftWay = transform.forward * direction + transform.right * (turn / Mathf.Abs(turn));
                rb.AddForce(DriftWay * maxForce);
            }

            //以現在的角度繼續飄移
            float angle = turn;
            //換方向就停止飄移
            while (Mathf.Abs(turn + angle) > Mathf.Abs(angle) && Mathf.Abs(rb.velocity.z) > 10) ;
        }

        //轉彎
        turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        transform.Rotate(0, turn * direction, 0);
        rb.AddForce(transform.right * (turn / Mathf.Abs(turn)) * Mathf.Abs(rb.velocity.z) * rb.mass);

        //施力
        if (Mathf.Abs(transform.GetComponent<Rigidbody>().velocity.z) < maxspeed)
        {
            rb.AddForce(transform.forward * maxForce * direction * Mathf.Cos(turn));
        }
        else
        {
            rb.velocity = transform.forward * direction * maxspeed;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Debug.Log("break");
        //    Debug.Log(gameObject.name + " s");
        //    carevent.ResetCar(this.gameObject.name, checkPoint);
        //}
        if (collision.gameObject.CompareTag("ground"))
        {
            if (Mathf.Abs(transform.rotation.x) > 45 || Mathf.Abs(transform.rotation.z) > 45 || Input.GetKey(KeyCode.R))
            {
                Debug.Log("break");
                Debug.Log(gameObject.name);
                carevent.ResetCar(this.gameObject.name, checkPoint);
            }
            else
            {
                transform.GetComponent<Rigidbody>().mass = 500;
                Move();
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Debug.Log("break");
        //    Debug.Log(gameObject.name + " e");
        //    carevent.ResetCar(this.gameObject.name, checkPoint);
            
        //}
        if (collision.gameObject.CompareTag("ground"))
        {
            Debug.Log("exit ground");
            direction = 0;
            transform.GetComponent<Rigidbody>().velocity *= 0.93f;
            transform.GetComponent<Rigidbody>().mass = Mathf.Infinity;
            maxForce = 0;
        }
    }
}
