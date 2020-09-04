using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class carevent
{
    public static void ResetCar(GameObject gameObject, Vector3 Position)
    {
        GameObject.Destroy(gameObject);
        GameObject newG = GameObject.Instantiate(gameObject, Position, Quaternion.Euler(0, 0, 0));
        newG.GetComponent<CarController>().enabled = true;
        newG.transform.GetChild(1).GetComponentInChildren<Camera>().enabled = true;
    }
}
