using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookBlocks : MonoBehaviour
{
    public GameObject one;
    public GameObject two;
    public GameObject three;
    public GameObject four;

    private void Start()
    {
        One();
    }

    public void One()
    {
        two.SetActive(false);
        three.SetActive(false);
        four.SetActive(false);
        one.SetActive(true);
    }

    public void Two()
    {
        one.SetActive(false);
        three.SetActive(false);
        four.SetActive(false);
        two.SetActive(true);
    }

    public void Three()
    {
        one.SetActive(false);
        two.SetActive(false);
        four.SetActive(false);
        three.SetActive(true);
    }

    public void Four()
    {
        one.SetActive(false);
        two.SetActive(false);
        three.SetActive(false);
        four.SetActive(true);
    }
}
