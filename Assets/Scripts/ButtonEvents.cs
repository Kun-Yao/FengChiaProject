using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvents : MonoBehaviour
{
    
    public GameObject right;
    GameObject engine;
    GameObject target;

    public static GameObject currentObject;

    private void Start()
    {
        engine = GameObject.Find("Engine");
    }

    private void Update()
    {
        if (right.GetComponent<Control>().GetRGrab())
        {
            if (right.GetComponent<Control>().bHit)
            {
                currentObject = right.GetComponent<Control>().hit.collider.gameObject;
                if (currentObject.CompareTag("scene"))
                {
                    if (currentObject.name.CompareTo("Home") == 0)
                    {
                        SceneManager.LoadScene(0);
                    }
                    else if (currentObject.name.CompareTo("Garage") == 0)
                    {
                        SceneManager.LoadScene(1);
                    }
                    else if (currentObject.name.CompareTo("Game") == 0)
                    {
                        SceneManager.LoadScene(2);
                    }
                    else if (currentObject.name.CompareTo("Go") == 0)
                    {
                        DontDestroyOnLoad(target);
                        SceneManager.LoadScene(3);
                    }
                }
                else if (currentObject.CompareTag("save"))
                {
                    if(engine != null)
                        engine.GetComponent<Combine>().combine();
                }
            }
        }
    }
        
}
