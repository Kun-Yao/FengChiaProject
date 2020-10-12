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
    public Text time;
    int sec = 0;
    int min = 0;
    bool startTime = true;

    void Awake()
    {

        for (int i = 0; i < 3; i++)
        {
            points.Push(4);
            points.Push(3);
            points.Push(2);
            points.Push(1);
        }
    }

    IEnumerator timer3()
    {
        yield return new WaitForSeconds(1);
        sec++;
        startTime = true;
    }

    void Update()
    {
        if (startTime)
        {
            StartCoroutine("timer3");
            startTime = false;
            min += sec / 60;
            sec = sec % 60;
            time.text = "Time: " + min + " : " + sec;
        }
    }
}
