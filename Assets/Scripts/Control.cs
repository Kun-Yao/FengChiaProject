using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Control : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean RTrig;
    [SerializeField] SteamVR_Action_Boolean LTrig;
    [SerializeField] SteamVR_Action_Boolean side;
    [SerializeField] SteamVR_Action_Boolean menu;
    [SerializeField] SteamVR_Action_Pose pose;
    [SerializeField] SteamVR_Action_Vector2 TouchAxis;
    [SerializeField] SteamVR_Action_Single RGas;
    [SerializeField] SteamVR_Action_Single LGas;
    [SerializeField] SteamVR_Action_Boolean Reset;
    [SerializeField] SteamVR_Action_Boolean drift;

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

    public bool setReset()
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

    public bool Jump()
    {
        return drift.stateUp;
    }
    public bool Drift()
    {
        return drift.state;
    }

    public bool unDrift()
    {
        return drift.stateUp;
    }
}
