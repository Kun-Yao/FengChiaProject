using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    private string CarName;
    private Vector3[] coordinate = { new Vector3(-15, 0, 0), new Vector3(-10, 0, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(10, 0, 0) };
    private bool[] isEmpty = new bool[6];
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void setName(string Name)
    {
        CarName = Name;
    }

    public string getName()
    {
        return CarName;
    }

    public void setEmpty(int index, bool empty)
    {
        isEmpty[index] = empty;
    }

    public bool getEmpty(int index)
    {
        return isEmpty[index];
    }

    public Vector3 getLocation(int index)
    {
        return coordinate[index];
    }
}
