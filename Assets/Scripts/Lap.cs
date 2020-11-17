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
        for (i = 0; i < 3; i++)
        {
            points.Push(23);
            points.Push(20);
            points.Push(17);
            points.Push(15);
            points.Push(6);
            points.Push(3);
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

        lastDis = Vector3.Distance(array[0].transform.position, array[1].transform.position);
        lastDis = (float)(Mathf.Round(lastDis * 100000)) / 100000;

        InvokeRepeating("time", 1, 1);
    }

    private void Update()
    {
        if (wrongWay)
            print("wrong");
    }

    private void time()
    {
        
        print(point);
        if (car == null)
            car = GameObject.FindWithTag("Player");
        newDis = Vector3.Distance(car.transform.position, array[point].transform.position);
        newDis = (float)(Mathf.Round(newDis * 100000)) / 100000;


        //夾角需要待測
        if (newDis > lastDis && Vector3.Angle(car.transform.forward, array[point].transform.position) > 20)
            wrongWay = true;
        else
            wrongWay = false;
        lastDis = newDis;
    }

    //IEnumerator timer3()
    //{
    //    yield return new WaitForSeconds(1);
    //    sec++;
    //    startTime = true;
    //}

    //void Update()
    //{
    //    if (startTime)
    //    {
    //        StartCoroutine("timer3");
    //        startTime = false;
    //        min += sec / 60;
    //        sec = sec % 60;
    //        time.text = "Time: " + min + " : " + sec;
    //    }
    //}
}
