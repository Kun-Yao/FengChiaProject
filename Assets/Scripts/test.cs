using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject control;
    public GameObject demo;

    public void cancel()
    {
        control.SetActive(true);
        demo.SetActive(false);
    }
}
