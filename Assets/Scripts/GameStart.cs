using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private GameManager gameManager;
    // Start is called before the first frame update
    void Awake()
    {
        
        for(int i=0; i<6; i++)
        {
            gameManager.setEmpty(i, true);
        }

        for(int i=1; i<6; i++)
        {
            if (gameManager.getEmpty(i) == true)
            {
                GameObject player = (GameObject)Instantiate(Resources.Load(gameManager.getName()), gameManager.getLocation(i), Quaternion.Euler(0, 0, 0));
                print(player.name);
                //player.transform.GetChild(0).gameObject.SetActive(true);
                gameManager.setEmpty(i, false);
                break;
            }
        }

        //transform.GetChild(0).gameObject.SetActive(false);
    }

    
}
