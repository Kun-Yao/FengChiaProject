using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseCar : MonoBehaviour
{
    public GameObject right;

    TextAsset ta;
    string[] vs;
    Vector3 PosOfCar;

    // Start is called before the first frame update
    void Start()
    {
        ta = Resources.Load<TextAsset>("carList/list");
        vs = ta.text.Split('\n');

        //MAC要把這個迴圈註解
        for (int i = 0; i < vs.Length - 1; i++)
        {
            vs[i] = vs[i].Substring(0, vs[i].Length - 1);
        }

        for(int i=0; i<vs.Length-1; i+=4)
        {
            for(int j=0; j<4; j++)
            {
                //車子的位置(待設定)
                PosOfCar = new Vector3(0, 0, 10);
                Instantiate(Resources.Load(vs[i]));
            }
            
        }
    }

    //利用觸控板移動list
    void RolltheRing()
    {

    }


}
