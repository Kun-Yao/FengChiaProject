﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;
    public float height = 2.5f;
    public float heightDamping = 2.0f;
    public float lookAtHeight = 0.0f;
    public Rigidbody parentRigidbody;
    public float rotationSnapTime = 0.3F;
    public float distanceSnapTime;
    public float distanceMultiplier;
    private Vector3 lookAtVector;
    private float usedDistance;
    float wantedRotationAngle;
    float wantedHeight;
    float currentRotationAngle;
    float currentHeight;
    Quaternion currentRotation;
    Vector3 wantedPosition;
    private float yVelocity = 0.0F;
    private float zVelocity = 0.0F;
    GameManager GM;

    void Start()
    {
        lookAtVector = new Vector3(0, lookAtHeight, 0);
        GM = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (target == null)
            target = GameObject.Find(GM.CarName).transform;

        if(parentRigidbody == null)
        {
            parentRigidbody = GameObject.Find(GM.CarName).GetComponent<Rigidbody>();
        }
    }
    void LateUpdate()
    {
        wantedHeight = target.position.y + height;
        currentHeight = transform.position.y;
        wantedRotationAngle = target.eulerAngles.y;
        currentRotationAngle = transform.eulerAngles.y;
        currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle, ref yVelocity, rotationSnapTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        wantedPosition = target.position;
        wantedPosition.y = currentHeight;
        usedDistance = Mathf.SmoothDampAngle(usedDistance, distance + (parentRigidbody.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);
        wantedPosition += Quaternion.Euler(0, currentRotationAngle, 0) * new Vector3(0, 0, -usedDistance);
        transform.position = wantedPosition;
        transform.LookAt(target.position + lookAtVector);
    }

}
