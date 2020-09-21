using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3[] coordinate = { new Vector3(-15, 0, 0), new Vector3(-10, 0, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(10, 0, 0)};
        bool[] isEmpty = new bool[6];
        for(int i=0; i<6; i++)
        {
            isEmpty[i] = true;
        }

        for(int i=1; i<6; i++)
        {
            if (isEmpty[i])
            {
                Instantiate(Resources.Load());
                isEmpty[i] = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
