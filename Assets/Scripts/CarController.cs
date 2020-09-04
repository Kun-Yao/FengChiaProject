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

    // Start is called before the first frame update
    void Start()
    {
        checkPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("break");
            carevent.ResetCar(this.gameObject, checkPoint);
            Debug.Log(gameObject.name);
        }
    }

    private void Move()
    {
        transform.GetComponent<Rigidbody>().AddForce(transform.forward * maxForce);
        //移動
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction = 1;
            if (transform.GetComponent<Rigidbody>().velocity.magnitude < maxspeed)
            {
                maxForce = 15000;
            }
            else
            {
                transform.GetComponent<Rigidbody>().velocity = transform.forward * direction * maxspeed;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) || left.GetComponent<Control>().getSide())
        {
            direction = -1;
            if (transform.GetComponent<Rigidbody>().velocity.magnitude < maxspeed)
            {
                maxForce = -15000;
            }
            else
            {
                transform.GetComponent<Rigidbody>().velocity = transform.forward * direction * maxspeed;
            }
        }
        else
        {
            direction = 0;
            transform.GetComponent<Rigidbody>().velocity *= 0.93f;
            maxForce = 0;
        }

        //轉彎
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //Debug.Log("右轉");
            transform.Rotate(0, 0.75f * direction, 0);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //Debug.Log("左轉");
            transform.Rotate(0, -0.75f * direction, 0);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("break");
            carevent.ResetCar(this.gameObject, checkPoint);
            Debug.Log(gameObject.name);
        }
        if (collision.gameObject.CompareTag("ground"))
        {
            if (Mathf.Abs(transform.rotation.x) > 45 || Mathf.Abs(transform.rotation.z) > 45 || Input.GetKey(KeyCode.R))
            {
                Debug.Log("break");
                carevent.ResetCar(this.gameObject, checkPoint);
                Debug.Log(gameObject.name);
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("break");
            carevent.ResetCar(this.gameObject, checkPoint);
            Debug.Log(gameObject.name);
        }
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
