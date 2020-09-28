using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Control : MonoBehaviour
{
    public SteamVR_Action_Boolean RTrig;
    public SteamVR_Action_Boolean LTrig;
    public SteamVR_Action_Boolean side;
    public SteamVR_Action_Boolean menu;
    public SteamVR_Action_Pose pose;
    public SteamVR_Action_Vector2 TouchAxis;
    public SteamVR_Action_Single RGas;
    public SteamVR_Action_Single LGas;
    public SteamVR_Action_Boolean Reset;

    public Ray ray;
    public RaycastHit hit;
    public bool bHit;

    public bool GetRGrab()
    {
        ray = new Ray(transform.position, transform.forward);
        bHit = Physics.Raycast(ray, out hit);
        return RTrig.stateUp;
    }
    public bool GetLGrab()
    {
        ray = new Ray(transform.position, transform.forward);
        bHit = Physics.Raycast(ray, out hit);
        return LTrig.stateUp;
    }

    public bool getSide()
    {
        return Reset.stateUp;
    }

    public bool getmenu()
    {
        return menu.stateUp;
    }

    public float accelator()
    {
        return RGas.axis;
    }

    public float goback()
    {
        return LGas.axis;
    }
}
