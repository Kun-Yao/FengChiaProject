using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lap : MonoBehaviour
{
    // public int point = 0;
    // public int lastPoint = 0;
    public int lap = 0;
    public Stack<int> points = new Stack<int>();
    int sec = 0;
    int min = 0;
    bool startTime = true;

    GameObject cp;
    GameObject car;
    public GameObject[] array = new GameObject[24];

    public float lastDis = 1000f;
    private float newDis = 0f;
    private bool wrongWay;
    public int point = 0;


    void Awake()
    {
        int i = 0;
        for (i = 0; i < 2; i++)
        {
            points.Push(4);
            points.Push(3);
            points.Push(2);
            points.Push(1);
            points.Push(0);
        }

        car = GameObject.FindWithTag("Player");

        //初始化逆向
        cp = GameObject.Find("CheckPoints");

        i = 0;
        foreach (Transform child in cp.transform)
        {
            array[i++] = child.gameObject;
        }
    }

   

  
}
