﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CarController : MonoBehaviour
{
    private SteamVR_Action_Single RGas = SteamVR_Input.GetSingleAction("RPull");
    private SteamVR_Action_Single LGas = SteamVR_Input.GetSingleAction("LPull");
    private SteamVR_Action_Boolean Reset = SteamVR_Input.GetBooleanAction("Reset");
    private SteamVR_Action_Boolean Drift = SteamVR_Input.GetBooleanAction("Drift");

    GameObject left;
    GameObject right;
    GameObject V, C, G, U;
    GameManager GM;

    int direction = 0;
    int driftLevel = 0;
    float maxspeed = 150;
    float maxForce = 3000;
    float turn = 0;
    float lastTurn = 0;
    float driftPower = 0;
    float currentForce;
    float boostForce;
    float RightRotationY;
    float LeftRotationY;
    float radius;
    float drag;
    bool isGround = false;
    bool isGroundLastFrame;
    bool isDrifting = false;

    Vector3 H_Direction;
    Vector3 checkPoint;
    Vector3 forceDir;
    Vector3 tempForce;
    Vector3 localVelocity;
    Rigidbody rb;

    Quaternion m_DriftOffset;
    Quaternion rotationStream;

    private void Awake()
    {
        GM = FindObjectOfType<GameManager>();
        Reset.onStateUp += Relife;

    }

    private void OnDestroy()
    {
        Reset.onStateUp -= Relife;
    }

// Start is called before the first frame update
    void Start()
    {
        if (GM.canMove == false)
        {
            StartCoroutine(wait());
        }
        currentForce = maxForce;
        checkPoint = transform.position;
        rb = transform.GetComponent<Rigidbody>();

        V = GameObject.Find("V");
        C = GameObject.Find("C");
        U = GameObject.Find("U");
        V.GetComponent<MeshRenderer>().enabled = true;
        C.GetComponent<MeshRenderer>().enabled = true;
        U.GetComponent<MeshRenderer>().enabled = true;
        G = GameObject.Find("Game");
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(5);
        startRace();
    }

    void startRace()
    {
        GM.canMove = true;
        
    }

    // Update is called once per frame
    void Update()
    {

        //按下空格起跳
        if (left.GetComponent<Control>().Jump())
        {
            if (isGround)   //如果在地上
            {
                Jump();
            }
        }

        //按下飄移鍵，並且有轉向：開始漂移
        //if (left.GetComponent<Control>().Drift() && turn != 0)
        //{
        //    //落地瞬間、不在飄移並且速度大於一定值時開始飄移
        //    if (isGround && !isGroundLastFrame && !isDrifting && rb.velocity.sqrMagnitude > 10)
        //    {
        //        StartDrift();   //開始飄移
        //    }
        //}

        ////放開飄移鍵：飄移結束
        //if (left.GetComponent<Control>().unDrift())
        //{
        //    if (isDrifting)
        //    {
        //       //Boost(boostForce);//加速
        //        StopDrift();//停止飄移
        //    }
        //}
    }

    private void FixedUpdate()
    {
        if (left == null)
        {
            print("left is null");
            left = GameObject.Find("Controller (left)");
        }

        if (right == null)
        {
            print("right is null");
            right = GameObject.Find("Controller (right)");
        }

        if (GM == null)
        {
            GM = FindObjectOfType<GameManager>();
        }

        if (GM.canMove == false) return;

        CheckGroundNormal();        //檢測是否在地面上，並且使車與地面保持水平

        if (isGround == false)
        {
            return;
        }

        TurnAround();                     //控制左右轉向

        //如果在漂移
        if (isDrifting)
        {
            CalculateDriftingLevel();   //计算漂移等级
        }

        //根据上述情况，进行最终的旋转和加力
        //rb.MoveRotation(rotationStream.normalized);
        //计算力的方向
        CalculateForceDir();
        //移动
        GiveForce();
        Show();
    }

    private void TurnAround()
    {
        
        //只能在移動時轉彎
        if (right.GetComponent<Control>().accelator() < 0.1 && left.GetComponent<Control>().goback() < 0.1)
        {
            return;
        }

        //轉彎: 根據雙手控制器y軸旋轉量的平均值計算轉彎角度
        RightRotationY = checkAngle(right.transform.localEulerAngles.y);
        LeftRotationY = checkAngle(left.transform.localEulerAngles.y);
        turn = (RightRotationY + LeftRotationY) / 2 / 100;

        if (turn > 0.05)
        {
            H_Direction = transform.right;
        }
        else if(turn < -0.05)
        {
            H_Direction = -transform.right;
        }
        else
        {
            H_Direction = transform.forward;
        }
        
        transform.Rotate(0, turn * direction, 0);

        if (right.GetComponent<Control>().accelator() > 0 || left.GetComponent<Control>().goback() > 0)
        {
            
            localVelocity = transform.InverseTransformDirection(rb.velocity);
            if(Mathf.Abs(localVelocity.x) > 15f && !isDrifting)
            {
                radius = Mathf.Sin((180 - 2 * Mathf.Abs(turn)) / 2) * localVelocity.z * 0.5f / Mathf.Sin(2 * Mathf.Abs(turn));
                drag = rb.mass * localVelocity.z * localVelocity.z / radius;
            }
            else
            {
                drag = 0;
            }
            rb.AddForce(drag * H_Direction);
        }

    }

    //計算施力方向
    public void CalculateForceDir()
    {
        //前進或倒退
        if (right.GetComponent<Control>().accelator() > 0.1)
        {
            direction = 1;
        }
        else if (left.GetComponent<Control>().goback() > 0.1)
        {
            direction = -1;
        }
        else
        {
            direction = 0;
            rb.velocity *= 0.95f;
        }

        forceDir = transform.forward * direction * Mathf.Cos(turn);
    }

    private void GiveForce()
    {
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        if(localVelocity.z > maxspeed)
        {
            currentForce = 0;
        }
        else
        {
            currentForce = maxForce;
        }
        
        //計算合力: 實際施力 = 最大力道 * 水平方向施力 * 雙手控制器按壓的幅度差
        tempForce = currentForce * forceDir * Mathf.Abs(right.GetComponent<Control>().accelator() - left.GetComponent<Control>().goback());
        SoundManager.playSound(Mathf.Abs(right.GetComponent<Control>().accelator() - left.GetComponent<Control>().goback()));

        if (!isGround)  //如不在地上，加重力
        {
            tempForce = rb.mass * 9.8f * Vector3.down;
        }

        rb.AddForce(tempForce, ForceMode.Force);
        
    }
    
    //加速
    public void Boost(float boostForce)
    {
        //按照漂移等级加速：1 / 1.1 / 1.2
        currentForce = (1 + (int)driftLevel / 10) * boostForce;
        rb.AddForce(currentForce * transform.forward, ForceMode.VelocityChange);
    }

   

    void Jump()
    {
        rb.AddForce(100 * Vector3.up);
    }

    private void StartDrift()
    {
        isDrifting = true;

        //根據水平輸入決定漂移時車的朝向，因為合速度方向與車身方向不一致，所以為加力方向添加偏移
        if (turn < -20)
        {
            H_Direction = transform.right;
        }
        else if (turn > 20)
        {
            H_Direction = -transform.right;
        }
    }

    void StopDrift()
    {
        isDrifting = false;
        H_Direction = new Vector3(0,0,0);
        driftPower = 0;
    }

    public void CalculateDriftingLevel()
    {
        driftPower += Time.fixedDeltaTime;
        //0.7秒提升一个漂移等级
        if (driftPower < 0.7)
        {
            driftLevel = 1;
        }
        else if (driftPower < 1.4)
        {
            driftLevel = 2;
        }
        else
        {
            driftLevel = 3;
        }
    }

    //偵測是否在地上
    public void CheckGroundNormal()
    {
        Quaternion VStream = transform.GetComponent<Rigidbody>().rotation;
        Quaternion HStream = transform.GetComponent<Rigidbody>().rotation;

        //在車底的四個方向設置laser
        RaycastHit frontHit;
        RaycastHit rearHit;
        RaycastHit rightHit;
        RaycastHit leftHit;

        //laser是否接觸地面
        Physics.Raycast(transform.position + transform.forward, Vector3.down, out frontHit);
        Physics.Raycast(transform.position + -transform.forward, Vector3.down, out rearHit);
        Physics.Raycast(transform.position + transform.right, Vector3.down, out rightHit);
        Physics.Raycast(transform.position + -transform.right, Vector3.down, out leftHit);


        if (frontHit.distance < 1.1f || rearHit.distance < .1f || rightHit.distance < 1.1f || leftHit.distance < 1.1f)
        {
            isGround = true;
        }

        else
        {
            isGround = false;
            direction = 0;
        }

        float deltav = frontHit.distance - rearHit.distance;
        float deltah = leftHit.distance - rightHit.distance;
        if (Mathf.Abs(deltav) > 0.1f)
        {
            transform.Rotate(Mathf.Asin(deltav / 2), 0, 0);
        }
        if (Mathf.Abs(deltah) > 0.1f)
        {
            transform.Rotate(0, 0, Mathf.Asin(deltah / 2));
        }


        //Vector3 tempNormal = (frontHit.normal + rearHit.normal).normalized;
        //Quaternion q = Quaternion.FromToRotation(transform.up, tempNormal);
        //rb.MoveRotation(q);

    }

    private void Relife(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!GM.canMove) return;
        Debug.Log("break");
        Debug.Log(gameObject.name);
        string[] N = gameObject.name.Split('(');
        //ResetCar(車名, 生成位置, 車頭方向);
        GM.ResetCar(N[0]);
    }

    private float checkAngle(float angle)
    {
        float finalAngle = angle - 180;
        if (finalAngle > 0)
        {
            return finalAngle - 180;
        }
        else
        {
            return finalAngle + 180;
        }
    }

    private void Show()
    {
        V.GetComponent<TextMesh>().text = (Mathf.Abs((int)localVelocity.z)).ToString();
        C.GetComponent<TextMesh>().text = G.GetComponent<Lap>().lap.ToString() + " / 2 Lap";
    }
}
