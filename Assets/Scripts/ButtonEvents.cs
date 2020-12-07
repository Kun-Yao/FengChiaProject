using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour
{
    
    public GameObject right;
    GameObject engine;
    GameObject target;
    public GameObject demo = null;
    public GameObject control = null;
    private string txtName;

    public static GameObject currentObject;
    GameManager GM;
    Text Warning;

    private void Start()
    {
        engine = GameObject.Find("Engine (1)");
    }

    private void Update()
    {
        Invoke("button", 1);
    }

    private void button()
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
                        SceneManager.LoadScene(3);
                    }
                }
                else if (currentObject.CompareTag("save"))
                {
                    if (engine != null)
                    {
                        engine.GetComponent<MeshRenderer>().enabled = false;
                        foreach(Transform child in engine.transform)
                            child.GetComponent<MeshRenderer>().enabled = false;
                        demo.SetActive(true);
                        control.SetActive(false);
                    }
                }
                else if (currentObject.CompareTag("end"))
                {
                    Application.Quit();
                }
            }
        }
    }

}
