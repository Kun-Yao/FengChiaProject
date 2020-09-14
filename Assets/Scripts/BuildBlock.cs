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
                if (currentObject.CompareTag("Block") || currentObject.CompareTag("Cylinder"))
                {
                    oldPos = right.GetComponent<Control>().hit.collider.gameObject.transform.position;
                    blockPos = right.GetComponent<Control>().hit.point + right.GetComponent<Control>().hit.normal / 2.0f;

                    blockPos.x = (float)Math.Round(blockPos.x, MidpointRounding.AwayFromZero);
                    blockPos.y = (float)Math.Round(blockPos.y, MidpointRounding.AwayFromZero);
                    blockPos.z = (float)Math.Round(blockPos.z, MidpointRounding.AwayFromZero);

                    if (newBlock == blocks[2])
                    {

                        if (blockPos.x != oldPos.x)
                        {
                            if (blockPos.x < 0)
                                blockPos.x += 0.3f;
                            else
                                blockPos.x -= 0.3f;
                            block = Instantiate(newBlock, blockPos, Quaternion.Euler(0, 0, 90));
                        }
                        else if (blockPos.z != oldPos.z)
                        {
                            if (blockPos.z < 0)
                                blockPos.z += 0.3f;
                            else
                                blockPos.z -= 0.3f;
                            block = Instantiate(newBlock, blockPos, Quaternion.Euler(90, 0, 0));
                        }
                        else
                        {
                            if (blockPos.y < 0)
                                blockPos.y += 0.3f;
                            else
                                blockPos.y -= 0.3f;
                            block = Instantiate(newBlock, blockPos, Quaternion.Euler(0, 0, 0));
                        }

                    }
                    else
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