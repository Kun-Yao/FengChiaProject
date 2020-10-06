using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ChooseCar : MonoBehaviour
{
    public SteamVR_Action_Vector2 touchPos = null;
    public SteamVR_Action_Boolean press = null;
    public GameObject right = null;
    public GameObject cameraRig;

    TextAsset ta;
    string[] vs;
    Vector3 PosOfCar;
    int AxisY;
    Ray ray;
    RaycastHit hit;
    GameObject currentObject;
    GameManager gameManager;
    float top;
    float buttom;
    GameObject lastObject;

    private void Awake()
    {
        touchPos.onAxis += Position;
        press.onState += PressRelease;
        gameManager = FindObjectOfType<GameManager>();
        transform.position = cameraRig.transform.position + new Vector3(0, 0, 7.63f);
    }

    private void OnDestroy()
    {
        touchPos.onAxis -= Position;
        press.onState -= PressRelease;
    }

    void Start()
    {
        ta = Resources.Load<TextAsset>("CarList/list");
        vs = ta.text.Split('\n');
        PosOfCar = transform.position + new Vector3(-5, 0, 0);

        //刪除字串後面的enter，MAC要把這個迴圈註解
        for (int i = 0; i < vs.Length - 1; i++)
        {
            vs[i] = vs[i].Substring(0, vs[i].Length - 1);
        }

        //顯示所有的車子
        for (int i = 0; i < vs.Length - 1; i += 4 )
        {
            //一列四台
            for(int j = 0; j < 4 && i+j < vs.Length - 1; j++)
            {
                print(j);
                GameObject model = (GameObject)Instantiate(Resources.Load("Prefabs/" + vs[i+j]), PosOfCar, Quaternion.Euler(0, 0, 0), transform);
                model.GetComponent<BoxCollider>().isTrigger = true;
                //車子的X軸(待設定)
                PosOfCar += new Vector3(5f, 0, 0);
            }
            //車子的Y軸(待設定)
            PosOfCar += new Vector3(-20, -2.5f, 0);
        }
    }
    

    private void Update()
    {
        SelectCar();
    }

    //按下觸控板移動list
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
        if (AxisY > 0 && transform.GetChild(vs.Length-1).transform.position.y <= cameraRig.transform.position.y)
        {
            transform.position += new Vector3(0, 0.5f, 0);
        }
        else if (AxisY < 0 && transform.GetChild(0).transform.position.y >= cameraRig.transform.position.y)
        {
            transform.position -= new Vector3(0, 0.5f, 0);
        }
    }

    private void SelectCar()
    {
        if (right.GetComponent<Control>().GetRGrab())
        {
            currentObject = right.GetComponent<Control>().hit.collider.gameObject;
            if (right.GetComponent<Control>().bHit && !currentObject.CompareTag("scene"))
            {
                if(lastObject != null)
                    lastObject.GetComponent<Outline>().enabled = false;
                currentObject.GetComponent<Outline>().enabled = true;
                string[] split = currentObject.name.Split('(');
                gameManager.CarName = split[0];
                print(gameManager.getName());
                lastObject = currentObject;

            }
        }
    }


}
