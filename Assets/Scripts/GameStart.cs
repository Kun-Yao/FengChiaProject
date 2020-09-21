using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
        for(int i=0; i<6; i++)
        {
            gameManager.setEmpty(i, true);
        }

        for(int i=1; i<6; i++)
        {
            if (gameManager.getEmpty(i) == true)
            {
                Instantiate(Resources.Load(gameManager.getName()), gameManager.getLocation(i), Quaternion.Euler(0, 0, 0));
                gameManager.setEmpty(i, false);
            }
        }
    }

    
}
