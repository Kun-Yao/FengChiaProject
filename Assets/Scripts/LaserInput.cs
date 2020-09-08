using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class LaserInput : MonoBehaviour
{
    public static GameObject currentObject;
    int currentID;
    Ray ray;
    RaycastHit hit;
    bool bHit;

    private void Start()
    {
        currentObject = null;
        currentID = 0;
    }

    private void Update()
    {
        ray = new Ray(transform.position, transform.forward);
        bHit = Physics.Raycast(ray, out hit);

        if (bHit)
        {
            print("AAA");
        }

        //RaycastHit[] hits;
        //hits = Physics.RaycastAll(transform.position, transform.forward, 100.0f);

        //for(int i = 0; i < hits.Length; i++)
        //{
        //    RaycastHit hit = hits[i];
        //    int id = hit.collider.gameObject.GetInstanceID();

        //    if(currentID != id)
        //    {
        //        currentID = id;
        //        currentObject = hit.collider.gameObject;
        //        print("AAAAAA");
        //    }
        //}
    }
}
