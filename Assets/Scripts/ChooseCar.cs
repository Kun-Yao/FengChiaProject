using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ChooseCar : MonoBehaviour
{
    public SteamVR_Action_Vector2 touchPos = null;
    public SteamVR_Action_Boolean press = null;

    TextAsset ta;
    string[] vs;
    Vector3 PosOfCar;
    int AxisY;

    private void Awake()
    {
        touchPos.onAxis += Position;
        press.onStateUp += PressRelease;
    }

    void Start()
    {
        ta = Resources.Load<TextAsset>("carList/list");
        vs = ta.text.Split('\n');

        //MAC要把這個迴圈註解
        for (int i = 0; i < vs.Length - 1; i++)
        {
            vs[i] = vs[i].Substring(0, vs[i].Length - 1);
        }

        for (int i = 0; i < vs.Length - 1; i += 4)
        {
            for (int j = 0; j < 4; j++)
            {
                //車子的位置(待設定)
                PosOfCar = new Vector3(0, 0, 10);
                Instantiate(Resources.Load(vs[i]));
            }

        }
    }

    private void OnDestroy()
    {
        touchPos.onAxis -= Position;
        press.onStateUp -= PressRelease;
    }

    //利用觸控板移動list
    private void Position(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        if(axis.y > 0)
        {
            AxisY = 1;
        }
        else
        {
            AxisY = -1;
        }
    }

    private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if(AxisY > 0)
        {
            transform.position += new Vector3(0, 0.5f, 0);
        }
        else
        {
            transform.position -= new Vector3(0, 0.5f, 0);
        }
    }

}
