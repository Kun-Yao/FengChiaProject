using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    GameManager gameManager;
    
    void Awake()
    {
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
                GameObject player = (GameObject)Instantiate(Resources.Load("Prefabs/"+gameManager.CarName), gameManager.getLocation(i), Quaternion.Euler(0, 0, 0));
                print(player.name);
                //player.transform.GetChild(0).gameObject.SetActive(true);
                gameManager.setEmpty(i, false);
                break;
            }
        }

        //transform.GetChild(0).gameObject.SetActive(false);
    }

    
}
