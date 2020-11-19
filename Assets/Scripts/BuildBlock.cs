using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BuildBlock : MonoBehaviour
{

    public GameObject newBlock;
    public int slotIndex = 1;
    public GameObject[] blocks;

    public GameObject left;
    public GameObject right;

    public static GameObject currentObject;

    void Awake()
    {
        newBlock = blocks[1];
    }

    public void SetBlock(int num)
    {
        newBlock = blocks[num];
    }

    void Update()
    {

        UpdateCar();
    }


    void UpdateCar()
    {

        //RaycastHit hit;
       //Ray ray;
        Vector3 blockPos;
        GameObject block;
        Vector3 oldPos;

        if (Input.GetMouseButtonDown(0) || right.GetComponent<Control>().GetRGrab())
        {
            if (right.GetComponent<Control>().bHit)
            {
                currentObject = right.GetComponent<Control>().hit.collider.gameObject;
                if (currentObject.CompareTag("Block"))
                {
                    oldPos = right.GetComponent<Control>().hit.collider.gameObject.transform.position;
                    blockPos = right.GetComponent<Control>().hit.point + right.GetComponent<Control>().hit.normal / 2.0f;

                    blockPos.x = (float)Math.Round(blockPos.x, MidpointRounding.AwayFromZero);
                    blockPos.y = (float)Math.Round(blockPos.y, MidpointRounding.AwayFromZero);
                    blockPos.z = (float)Math.Round(blockPos.z, MidpointRounding.AwayFromZero);

                    block = Instantiate(newBlock, blockPos, Quaternion.identity);
                    block.transform.parent = this.transform;
                }
            }
        }
        if (Input.GetMouseButtonDown(1) || left.GetComponent<Control>().GetLGrab())
        {
            if (left.GetComponent<Control>().bHit)
            {
                currentObject = left.GetComponent<Control>().hit.collider.gameObject;
                if (currentObject.CompareTag("Block") || currentObject.CompareTag("Cylinder"))
                {
                    block = left.GetComponent<Control>().hit.collider.gameObject;
                    if (block != blocks[0])
                        Destroy(block);
                }
            }
        }
    }
}