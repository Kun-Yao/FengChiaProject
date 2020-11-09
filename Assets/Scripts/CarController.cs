using System.Collections;
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
    GameManager GM;

    int direction = 0;
    int driftDirection = 0;
    int driftLevel = 0;
    float maxspeed = 60;
    float maxForce = 3000;
    float turn = 0;
    float driftPower = 0;
    float currentForce;
    float boostForce;
    bool isGround = false;
    bool isGroundLastFrame;
    bool isDrifting = false;

    Vector3 checkPoint;
    Vector3 forceDir_Horizontal;
    Rigidbody rb;

    Quaternion m_DriftOffset;
    Quaternion rotationStream;

    private void Awake()
    {
        GM = FindObjectOfType<GameManager>();
        Reset.onStateUp += Relife;
        if (GM.canMove == false)
        {
            StartCoroutine(wait());
        }
    }

    private void OnDestroy()
    {
        Reset.onStateUp -= Relife;
    }

// Start is called before the first frame update
    void Start()
    {
        currentForce = maxForce;
        checkPoint = transform.position;
        rb = transform.GetComponent<Rigidbody>();
        rb.mass = 100;
    }

    IEnumerator wait()
    {
        print("start");
        yield return new WaitForSeconds(3);
        print("end");
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
        if (left.GetComponent<Control>().Drift() && turn != 0)
        {
            //落地瞬間、不在飄移並且速度大於一定值時開始飄移
            if (isGround && !isGroundLastFrame && !isDrifting && rb.velocity.sqrMagnitude > 10)
            {
                StartDrift();   //開始飄移
            }
        }

        //放開飄移鍵：飄移結束
        if (left.GetComponent<Control>().unDrift())
        {
            if (isDrifting)
            {
                Boost(boostForce);//加速
                StopDrift();//停止飄移
            }
        }
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

        //車子轉向
        CheckGroundNormal();        //檢測是否在地面上，並且使車與地面保持水平
        if (isGround == false)
        {
            return;
        }
        TurnAround();                     //控制左右轉向

        //漂移加速后/松开加油键 力递减
        //ReduceForce();

        //如果在漂移
        if (isDrifting)
        {
            CalculateDriftingLevel();   //计算漂移等级
        }

        //根据上述情况，进行最终的旋转和加力
        rb.MoveRotation(rotationStream.normalized);
        //计算力的方向
        CalculateForceDir();
        //移动
        GiveForce();
    }

    private void TurnAround()
    {
        //只能在移動時轉彎
        if (rb.velocity.sqrMagnitude <= 0.1)
        {
            return;
        }

        //轉彎: 根據雙手控制器y軸旋轉量的平均值計算轉彎角度
        turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        //print("turn = " + right.transform.localRotation.eulerAngles);
        if(Mathf.Abs(turn) > 45)
        {
            turn = 45 * turn / Mathf.Abs(turn);
        }
        transform.Rotate(0, turn * direction, 0);

        //飄移角度
        if (driftDirection == -1)
        {
            rotationStream = rotationStream * Quaternion.Euler(0, -40 * Time.fixedDeltaTime, 0);
        }
        else if (driftDirection == 1)
        {
            rotationStream = rotationStream * Quaternion.Euler(0, 40 * Time.fixedDeltaTime, 0);
        }

        Quaternion deltaRotation = Quaternion.Euler(0, turn * direction, 0);
        rotationStream = rotationStream * deltaRotation;
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
        
        //水平施力方向
        forceDir_Horizontal = m_DriftOffset * transform.forward;
    }

    private void GiveForce()
    {
        //計算合力: 實際施力 = 最大力道 * 水平方向施力 * 雙手控制器按壓的幅度差
        Vector3 tempForce = maxForce * forceDir_Horizontal * (right.GetComponent<Control>().accelator() - left.GetComponent<Control>().goback());

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

    //力递减
    //public void ReduceForce()
    //{

    //    float targetForce = currentForce;
    //    if (isGround && (right.GetComponent<Control>().accelator() <= 0.1 || left.GetComponent<Control>().goback() <= 0.1))
    //    {
    //        direction = 0;
    //        rb.velocity *= 0.98f;
    //    }
    //    else if (currentForce > maxForce)    //用于加速后回到普通状态
    //    {
    //        targetForce = maxForce;
    //    }

    //    //每秒60递减，可调
    //    currentForce = Mathf.MoveTowards(currentForce, targetForce, 60 * Time.fixedDeltaTime);
    //}

    void Jump()
    {
        rb.AddForce(100 * Vector3.up);
    }

    private void StartDrift()
    {
        isDrifting = true;

        //根據水平輸入決定漂移時車的朝向，因為合速度方向與車身方向不一致，所以為加力方向添加偏移
        if (turn < 0)
        {
            driftDirection = -1;
            //左漂移時，合速度方向為車頭朝向的右前方，偏移具體數值需結合實際調試
            m_DriftOffset = Quaternion.Euler(0f, 30, 0f);
        }
        else if (turn > 0)
        {
            driftDirection = 1;
            m_DriftOffset = Quaternion.Euler(0f, -30, 0f);
        }
    }

    void StopDrift()
    {
        isDrifting = false;
        driftDirection = 0;
        driftPower = 0;
        m_DriftOffset = Quaternion.identity;
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
        bool hasfront = Physics.Raycast(transform.position + new Vector3(0, 0, 2), -transform.up, out frontHit);
        bool hasrear = Physics.Raycast(transform.position + new Vector3(0, 0, -2), -transform.up, out rearHit);
        bool hasright = Physics.Raycast(transform.position + new Vector3(1, 0, 0), -transform.up, out rightHit);
        bool hasleft = Physics.Raycast(transform.position + new Vector3(-1, 0, 0), -transform.up, out leftHit);

        if (hasfront || hasrear || hasright || hasleft)
        {
            isGround = true;
            Debug.DrawLine(transform.position + new Vector3(0, 0, 2), frontHit.point);
        }

        else
        {
            isGround = false;
            direction = 0;
        }
            
        //垂直方向與地面水平
        Vector3 VNormal = (frontHit.normal + rearHit.normal).normalized;
        Quaternion VQuaternion = Quaternion.FromToRotation(transform.up, VNormal);
        VStream = VQuaternion * VStream;
        transform.GetComponent<Rigidbody>().MoveRotation(VStream);

        //水平方向與地面水平
        Vector3 HNormal = (frontHit.normal + rearHit.normal).normalized;
        Quaternion HQuaternion = Quaternion.FromToRotation(transform.up, HNormal);
        HStream = HQuaternion * HStream;
        transform.GetComponent<Rigidbody>().MoveRotation(HStream);

    }

    private void Relife(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("break");
        Debug.Log(gameObject.name);
        string[] N = gameObject.name.Split('(');
        //ResetCar(車名, 生成位置, 車頭方向);
        GM.ResetCar(N[0]);
    }
}
