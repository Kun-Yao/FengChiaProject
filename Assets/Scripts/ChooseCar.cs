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
        ta = Resources.Load<TextAsset>("CarList/list");
        vs = ta.text.Split('\n');
        PosOfCar = transform.position + new Vector3(-5, 5, 0);

        //刪除字串後面的enter，MAC要把這個迴圈註解
        for (int i = 0; i < vs.Length - 1; i++)
        {
            vs[i] = vs[i].Substring(0, vs[i].Length - 1);
        }
        
        //顯示所有的車子
        for (int i = 0; i < vs.Length - 1; i += 4 )
        {
            //一列四台
            for(int j = 0; j < 4; j++)
            {
                Instantiate(Resources.Load("Prefabs/" + vs[i]), PosOfCar, Quaternion.Euler(0, 0, 0), transform);
                print(vs[i]);
                //車子的X軸(待設定)
                PosOfCar += new Vector3(2.5f, 0, 0);
            }
            //車子的Y軸(待設定)
            PosOfCar += new Vector3(0, 2.5f, 0);
        }
    }

    private void OnDestroy()
    {
        touchPos.onAxis -= Position;
        press.onStateUp -= PressRelease;
    }

    //按下觸控板移動list(未完成)
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
