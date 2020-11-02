using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    GameManager gameManager;
    GameObject StartLine;
    
    void Awake()
    {
        StartLine = GameObject.Find("StartLine");
        gameManager = FindObjectOfType<GameManager>();
        print(gameManager.CarName);
        for(int i=0; i<6; i++)
        {
            gameManager.setEmpty(i, true);
        }

        for(int i=1; i<6; i++)
        {
            if (gameManager.getEmpty(i) == true)
            {
                //生成賽車(player)
                GameObject player = (GameObject)Instantiate(Resources.Load("Prefabs/" + gameManager.CarName), StartLine.transform.position, Quaternion.Euler(0,0,0));
                player.transform.rotation = Quaternion.FromToRotation(player.transform.forward, StartLine.transform.forward);
                string[] tmp = player.name.Split('(');
                player.name = tmp[0];

                //起始位置
                player.transform.position -= new Vector3(0, 0, player.GetComponent<BoxCollider>().center.z);
                player.transform.position -= new Vector3(0, 0, player.GetComponent<BoxCollider>().size.z/2 + 1);

                //關閉場地攝影機
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                //開啟賽車攝影機
                player.transform.GetChild(0).gameObject.SetActive(true);
                player.GetComponent<Rigidbody>().useGravity = true;
                player.GetComponent<CarController>().enabled = true;
                gameManager.setEmpty(i, false);
                break;
            }
        }
        print("canMove? " + carevent.canMove);
        wait();
        //transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(3000);
        print("end");
        startRace();
    }

    void startRace()
    {
        carevent.canMove = true;
        print("canMove? " + carevent.canMove);
    }
}
