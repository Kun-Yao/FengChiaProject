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
    int AxisX;
    int now;
    Ray ray;
    RaycastHit hit;
    GameObject currentObject;
    GameManager gameManager;
    float top;
    float buttom;
    GameObject lastObject;
    GameObject rArrow, lArrow;

    private void Awake()
    {
        touchPos.onAxis += Position;
        press.onStateUp += PressRelease;
        gameManager = FindObjectOfType<GameManager>();
        transform.position = cameraRig.transform.position + new Vector3(0, 0, 7.63f);
    }

    private void OnDestroy()
    {
        touchPos.onAxis -= Position;
        //press.onState -= PressRelease;
    }

    void Start()
    {
        ta = Resources.Load<TextAsset>("CarList/list");
        vs = ta.text.Split('\n');
        PosOfCar = transform.position;
        lArrow = GameObject.Find("LArrow");
        rArrow = GameObject.Find("RArrow");

        //刪除字串後面的enter，MAC要把這個迴圈註解
        for (int i = 0; i < vs.Length - 1; i++)
        {
            vs[i] = vs[i].Substring(0, vs[i].Length - 1);
        }

        //顯示所有的車子
        for (int i = 0; i < vs.Length - 1; i++ )
        {
            GameObject model = (GameObject)Instantiate(Resources.Load("Prefabs/" + vs[i]), PosOfCar, Quaternion.Euler(0, 0, 0), transform);
            BoxCollider box = model.GetComponent<BoxCollider>();
            box.isTrigger = true;
            //車子的X軸
            print(PosOfCar);
            PosOfCar += new Vector3(20, 0, 0);
        }
        now = 0;
        print("now" + now);
        currentObject = transform.GetChild(now).gameObject;
        if (lastObject != null)
        {
            lastObject.GetComponent<Outline>().enabled = false;
            lastObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        currentObject.GetComponent<Outline>().enabled = true;
        string[] split = currentObject.name.Split('(');
        gameManager.CarName = split[0];
        print(gameManager.getName());
        lastObject = currentObject;
        lArrow.SetActive(false);
    }
    

    private void Update()
    {
        //SelectCar();
    }

    //按下觸控板左右移動list
    private void Position(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        if (axis.x > 0.5f)
        {
            AxisX = -1;
        }
        else if (axis.x < -0.5f)
        {
            AxisX = 1;
        }
    }

    private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        //可往右?
        if (AxisX > 0 && now > 0)
        {
            transform.position += new Vector3(20, 0, 0);
            now--;
        }

        //可往左?
        else if (AxisX < 0 && now < vs.Length-2)
        {
            lArrow.SetActive(true);
            rArrow.SetActive(true);
            transform.position -= new Vector3(20, 0, 0);
            now++;
        }
        
        if(now == 0)
        {
            lArrow.SetActive(false);
            rArrow.SetActive(true);
        }
        else if(now == vs.Length - 2)
        {
            lArrow.SetActive(true);
            rArrow.SetActive(false);
        }
        else
        {
            lArrow.SetActive(true);
            rArrow.SetActive(true);
        }

        print("now" + now);
        currentObject = transform.GetChild(now).gameObject;
        if (lastObject != null)
        {
            lastObject.GetComponent<Outline>().enabled = false;
            lastObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        currentObject.GetComponent<Outline>().enabled = true;
        string[] split = currentObject.name.Split('(');
        gameManager.CarName = split[0];
        print(gameManager.getName());
        lastObject = currentObject;
    }

    //private void SelectCar()
    //{
    //    if (right.GetComponent<Control>().GetRGrab())
    //    {
    //        currentObject = right.GetComponent<Control>().hit.collider.gameObject;
    //        if (right.GetComponent<Control>().bHit && !currentObject.CompareTag("scene"))
    //        {
    //            if(lastObject != null)
    //            {
    //                lastObject.GetComponent<Outline>().enabled = false;
    //                lastObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    //            }
    //            currentObject.GetComponent<Outline>().enabled = true;
    //            string[] split = currentObject.name.Split('(');
    //            gameManager.CarName = split[0];
    //            print(gameManager.getName());
    //            lastObject = currentObject;

    //        }
    //    }
    //}


}
