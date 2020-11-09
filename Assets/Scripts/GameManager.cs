using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public string CarName;
    public int relifePoint;
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
        else if(this != instance)
        {
            Destroy(gameObject);
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

    public bool canMove = false;

    public void ResetCar(string CarName)
    {
        string name = CarName;
        Debug.Log(name);
        GameObject.Destroy(GameObject.Find(CarName));
        //GameObject clone = (GameObject)Resources.Load(CarName);
        Transform relife = GameObject.Find("CheckPoints").transform.GetChild(relifePoint);
        GameObject newG = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/" + name), relife.position, Quaternion.Euler(0, 0, 0));
        newG.transform.rotation = Quaternion.FromToRotation(newG.transform.forward, relife.right);
        newG.name = name;
        newG.GetComponent<CarController>().enabled = true;
        //newG.transform.GetChild(0).gameObject.SetActive(true);
        newG.GetComponent<Rigidbody>().useGravity = true;
        canMove = true;
    }
}
