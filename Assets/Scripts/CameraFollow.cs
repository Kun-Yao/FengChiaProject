using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameManager gameManager;
    BoxCollider Box;
    GameObject kart;
    Vector3 CurrCarPos;
    Vector3 LastCarPos;
    Vector3 PosOfCam;
    Quaternion LastCarRot;
    Quaternion CurrCarRot;


    float distance;
    float height;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        kart = GameObject.Find(gameManager.CarName);
        Box = kart.GetComponent<BoxCollider>();
        distance = 5;
        height = Box.size.y / Box.size.z;
        transform.position = kart.transform.position + new Vector3(0, height, -distance);
        //transform.rotation = Quaternion.FromToRotation(transform.forward, transform.right);
        print("forward " + kart.transform.forward + Vector3.forward);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
        if (LastCarPos != kart.transform.position)
        {
            LastCarPos = CurrCarPos;
        }

        //每二個frame儲存一次賽車的角度
        if(time >= 0.02)
        {
            LastCarRot = CurrCarRot;
            time = 0;
        }
        
        CurrCarRot = kart.transform.rotation;
        CurrCarPos = kart.transform.localPosition;
        //transform.position = kart.transform.position + new Vector3(0, height, -distance);
        transform.position = LastCarPos + (transform.up * Box.size.y) - (transform.forward * Box.size.z);
        transform.rotation = LastCarRot;

    }

}
