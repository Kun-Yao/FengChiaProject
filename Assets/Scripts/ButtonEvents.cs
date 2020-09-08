using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvents : MonoBehaviour
{
    public int SceneIndex;
    public GameObject right;

    public static GameObject currentObject;
    

    private void Update()
    {
        if (right.GetComponent<Control>().GetRGrab())
        {
            if (right.GetComponent<Control>().bHit)
            {
                currentObject = right.GetComponent<Control>().hit.collider.gameObject;
                string tag = currentObject.tag;
                if (tag == "scene")
                {
                    currentObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    print("Scene " + SceneIndex);
                    SceneManager.LoadScene(SceneIndex);
                }
            }
        }
    }
        
}
