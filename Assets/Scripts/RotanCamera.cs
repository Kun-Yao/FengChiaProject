using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RotanCamera : MonoBehaviour
{
    public SteamVR_Action_Vector2 touchPos = null;
    public SteamVR_Action_Boolean press = null;
    GameObject engine;
    GameObject canvas;

    int AxisX;

    private void Awake()
    {
        touchPos.onAxis += Position;
        engine = GameObject.Find("Engine");
        canvas = GameObject.Find("Canvas");
        //press.onStateUp += PressRelease;
    }

    private void OnDestroy()
    {
        touchPos.onAxis -= Position;
        //press.onStateUp -= PressRelease;
    }

    private void Position(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        if (axis.x > 0)
        {
            transform.RotateAround(engine.transform.position, Vector3.up, -100 * Time.deltaTime);
            canvas.transform.RotateAround(engine.transform.position, Vector3.up, -100 * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(engine.transform.position, Vector3.up, 100 * Time.deltaTime);
            canvas.transform.RotateAround(engine.transform.position, Vector3.up, 100 * Time.deltaTime);
        }
    }

    //private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    //{
    //    if (AxisX > 0)
    //    {
    //        transform.RotateAround(Vector3.zero, Vector3.up, -200 * Time.deltaTime);
    //    }
    //    else
    //    {
    //        transform.RotateAround(Vector3.zero, Vector3.up, 200 * Time.deltaTime);
    //    }
    //}
}
