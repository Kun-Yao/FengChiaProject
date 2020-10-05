using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class carevent
{
    public static void ResetCar(string CarName, Vector3 Position)
    {
        string name = CarName;
        GameObject.Destroy(GameObject.Find(CarName));
        //GameObject clone = (GameObject)Resources.Load(CarName);
        GameObject newG = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/" + name), Position, Quaternion.Euler(0, 0, 0));
        newG.name = name;
        newG.GetComponent<CarController>().enabled = true;
        newG.transform.GetChild(0).gameObject.SetActive(true);
        newG.GetComponent<Rigidbody>().useGravity = true;
    }
}
