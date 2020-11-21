using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{

    public int checkPoint;
    GameObject GM;
    GameManager gameManager;
    float ruler;
    public GameObject finished;

    private void Start()
    {
        GM = GameObject.Find("Game");
        gameManager = FindObjectOfType<GameManager>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.relifePoint = checkPoint;
            print(checkPoint);

            if (GM.GetComponent<Lap>().points.Count <= 0)
            {
                print("Finish");
                other.GetComponent<CarController>().enabled = false;
                finished.SetActive(true);
                Invoke("finish", 5);
            }
            else if (GM.GetComponent<Lap>().points.Peek() == checkPoint)
            {

                GM.GetComponent<Lap>().points.Pop();
                if (checkPoint == 0)
                    GM.GetComponent<Lap>().lap++;
            }
            
           
        }
    }

    private void finish()
    {
        SceneManager.LoadScene(0);
    }
}
