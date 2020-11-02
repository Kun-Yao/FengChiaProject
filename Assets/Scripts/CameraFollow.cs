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
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        kart = GameObject.Find(gameManager.CarName);
        Box = kart.GetComponent<BoxCollider>();
        print("kart " + kart.name);
        transform.position = kart.transform.position + new Vector3(0, Box.size.y, -Box.size.z);
        CurrCarPos = kart.transform.position;
        LastCarPos = CurrCarPos - new Vector3(0, 0, 3);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PosOfCam = LastCarPos;
        transform.position = PosOfCam;
        if (Mathf.Abs(CurrCarPos.z - LastCarPos.z) > 0.1f)
        {
            LastCarPos = CurrCarPos;
            CurrCarPos = kart.transform.position;
        }
        
        print("curr " + CurrCarPos);
        print("last " + LastCarPos);

    }

}
