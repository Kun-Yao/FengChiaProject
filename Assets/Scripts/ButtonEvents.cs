using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvents : MonoBehaviour
{
    public int SceneIndex;
    public GameObject right;
    public void TransScene(int SceneIndex)
    {
        if (right.GetComponent<Control>().GetGrab())
        {
            print("Click");
            Debug.Log("Scene " + SceneIndex);
            if (right.GetComponent<Control>().bHit && CompareTag("scene"))
            {
                SceneManager.LoadScene(SceneIndex);
            }
        }
    }
    
}
