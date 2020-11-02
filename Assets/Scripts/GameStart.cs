using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    GameManager gameManager;
    GameObject StartLine;
    GameObject number;
    BoxCollider box;

    void Awake()
    {
        StartLine = GameObject.Find("StartLine");
        gameManager = FindObjectOfType<GameManager>();
        print(gameManager.CarName);
        for (int i = 0; i < 6; i++)
        {
            gameManager.setEmpty(i, true);
        }

        for (int i = 1; i < 6; i++)
        {
            if (gameManager.getEmpty(i) == true)
            {
                //生成賽車(player)
                GameObject player = (GameObject)Instantiate(Resources.Load("Prefabs/" + gameManager.CarName), StartLine.transform.position, Quaternion.Euler(0, 0, 0));
                player.transform.rotation = Quaternion.FromToRotation(player.transform.forward, StartLine.transform.forward);
                string[] tmp = player.name.Split('(');
                player.name = tmp[0];
                box = player.transform.GetComponent<BoxCollider>();

                //起始位置
                player.transform.position -= new Vector3(0, 0, player.GetComponent<BoxCollider>().center.z);
                player.transform.position -= new Vector3(0, 0, player.GetComponent<BoxCollider>().size.z / 2 + 1);

                //關閉場地攝影機
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);

                //開啟賽車攝影機                
                player.transform.GetChild(0).gameObject.SetActive(true);
                player.transform.GetChild(0).gameObject.AddComponent<CameraFollow>();
                player.transform.GetChild(0).SetParent(null);
                player.GetComponent<Rigidbody>().useGravity = true;
                player.GetComponent<CarController>().enabled = true;
                gameManager.setEmpty(i, false);
                break;
            }
        }
        //transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(count());
    }

    IEnumerator count()
    {
        number = GameObject.Find("Count");
        print("a");
        yield return new WaitForSeconds(2);
        number.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        number.transform.GetChild(0).gameObject.SetActive(false);
        number.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        number.transform.GetChild(1).gameObject.SetActive(false);
        number.transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        number.transform.GetChild(2).gameObject.SetActive(false);
        number.transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        number.transform.GetChild(3).gameObject.SetActive(false);
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
