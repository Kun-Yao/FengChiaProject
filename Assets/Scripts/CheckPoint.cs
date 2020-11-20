using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    public int checkPoint;
    GameObject GM;
    GameManager gameManager;
    float ruler;

    void OnTriggerEnter(Collider other)
    {
        GM = GameObject.Find("Game");
        gameManager = FindObjectOfType<GameManager>();

        if (other.gameObject.tag == "Player")
        {
            gameManager.relifePoint = checkPoint;
            if (GM.GetComponent<Lap>().points.Peek() == checkPoint)
            {

                GM.GetComponent<Lap>().points.Pop();
                if (checkPoint == 0)
                    GM.GetComponent<Lap>().lap++;
            }

            if(GM.GetComponent<Lap>().points.Count == 0)
            {
                print("Finish");
            }


            if (checkPoint == 23)
            {
                GM.GetComponent<Lap>().point = 0;
                ruler = Vector3.Distance(GM.GetComponent<Lap>().array[0].transform.position, GM.GetComponent<Lap>().array[checkPoint].transform.position);
            }
            else
            {
                GM.GetComponent<Lap>().point = checkPoint + 1;
                ruler = Vector3.Distance(GM.GetComponent<Lap>().array[checkPoint].transform.position, GM.GetComponent<Lap>().array[checkPoint + 1].transform.position);
            }
            GM.GetComponent<Lap>().lastDis = (float)(Mathf.Round(ruler * 100000)) / 100000;
        }

        // if(checkPoint == 1) {
        //     //主要是看是不是到了最後一個判斷點 是增加圈數
        //     if(other.gameObject.GetComponent<Lap>().point == 2)
        //         other.gameObject.GetComponent<Lap>().lap += 1;
        //     //這邊主要是怕人會去逆向跑也會算進圈數中
        //     other.gameObject.GetComponent<Lap>().point = 1;
        //     other.gameObject.GetComponent<Lap>().lastPoint = 1;

        // }
        // else {
        //     //必須上一個判斷也要是和point一樣才可以把主要判斷點point變成目前的checkPoint
        //     if(other.gameObject.GetComponent<Lap>().point == checkPoint-1 && other.gameObject.GetComponent<Lap>().point == other.gameObject.GetComponent<Lap>().lastPoint)
        //         other.gameObject.GetComponent<Lap>().point = checkPoint;
        //     other.gameObject.GetComponent<Lap>().lastPoint = checkPoint;

        // }
    }
}
