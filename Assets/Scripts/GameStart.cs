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
                GameObject player = (GameObject)Instantiate(Resources.Load("Prefabs/car14"), StartLine.transform.position, Quaternion.Euler(0, 0, 0));
                string[] tmp = player.name.Split('(');
                player.name = tmp[0];
                print(i);
                player.transform.position -= new Vector3(0, 0, player.GetComponent<BoxCollider>().center.z);
                player.transform.position -= new Vector3(0, 0, player.GetComponent<BoxCollider>().size.z/2 + 1);
                print(player.name);
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                player.transform.GetChild(0).gameObject.SetActive(true);
                player.GetComponent<Rigidbody>().useGravity = true;
                gameManager.setEmpty(i, false);
                break;
            }
        }

        //transform.GetChild(0).gameObject.SetActive(false);
    }

    
}
