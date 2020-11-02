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
    GameObject checkPoints;

    int direction = 0;
    int driftDirection = 0;
    int driftLevel = 0;
    float maxspeed = 60;
    float maxForce = 15000;
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
        Reset.onStateUp += Relife;
    }

    private void OnDestroy()
    {
        Reset.onStateUp -= Relife;
    }

// Start is called before the first frame update
    void Start()
    {
        currentForce = maxForce;
        StartCoroutine(wait());
        checkPoint = transform.position;
        rb = transform.GetComponent<Rigidbody>();
        rb.mass = 100;
        left = transform.GetChild(0).transform.GetChild(0).gameObject;
        right = transform.GetChild(0).transform.GetChild(1).gameObject;
        checkPoints = GameObject.Find("CheckPoints");

    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(3);
        print("end");
        startRace();
    }

    void startRace()
    {
        carevent.canMove = true;
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

        //按住空格，并且有水平输入：开始漂移
        if (left.GetComponent<Control>().Drift() && turn != 0)
        {
            //落地瞬间、不在漂移并且速度大于一定值时开始漂移
            if (isGround && !isGroundLastFrame && !isDrifting && rb.velocity.sqrMagnitude > 10)
            {
                StartDrift();   //开始漂移
            }
        }

        //放开空格：漂移结束
        if (left.GetComponent<Control>().unDrift())
        {
            if (isDrifting)
            {
                Boost(boostForce);//加速
                StopDrift();//停止漂移
            }
        }
    }

    private void FixedUpdate()
    {
        //车转向
        CheckGroundNormal();        //检测是否在地面上，并且使车与地面保持水平
        if (isGround == false)
        {
            return;
        }
        TurnAround();                     //输入控制左右转向

        //漂移加速后/松开加油键 力递减
        ReduceForce();

        //如果在漂移
        if (isDrifting)
        {
            CalculateDriftingLevel();   //计算漂移等级
        }

        //根据上述情况，进行最终的旋转和加力
        rb.MoveRotation(rotationStream);
        //计算力的方向
        CalculateForceDir();
        //移动
        GiveForce();
    }

    private void TurnAround()
    {
        //只能在移动时转弯
        if (rb.velocity.sqrMagnitude <= 0.1)
        {
            return;
        }

        //轉彎
        turn = (right.transform.localRotation.y + left.transform.localRotation.y) / 2;
        print("turn = " + right.transform.localRotation.eulerAngles);
        if(Mathf.Abs(turn) > 45)
        {
            turn = 45 * turn / Mathf.Abs(turn);
        }
        transform.Rotate(0, turn * direction, 0);

        //漂移时自带转向
        if (driftDirection == -1)
        {
            rotationStream = rotationStream * Quaternion.Euler(0, -40 * Time.fixedDeltaTime, 0);
        }
        else if (driftDirection == 1)
        {
            rotationStream = rotationStream * Quaternion.Euler(0, 40 * Time.fixedDeltaTime, 0);
        }

        Quaternion deltaRotation = Quaternion.Euler(0, turn * direction, 0);
        rotationStream = rotationStream * deltaRotation;//局部坐标下旋转
    }

    //计算加力方向
    public void CalculateForceDir()
    {
        //往前加力
        if (right.GetComponent<Control>().accelator() > 0.1)
        {
            direction = 1;
        }
        else if (left.GetComponent<Control>().goback() > 0.1)//往后加力
        {
            direction = -1;
        }

        forceDir_Horizontal = m_DriftOffset * transform.forward;
    }

    private void GiveForce()
    {
        //计算合力
        Vector3 tempForce = maxForce * forceDir_Horizontal * (right.GetComponent<Control>().accelator() - left.GetComponent<Control>().goback());

        if (!isGround)  //如不在地上，则加重力
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
    public void ReduceForce()
    {

        float targetForce = currentForce;
        if (isGround && (right.GetComponent<Control>().accelator() <= 0.1 || left.GetComponent<Control>().goback() <= 0.1))
        {
            direction = 0;
            rb.velocity *= 0.98f;
        }
        else if (currentForce > maxForce)    //用于加速后回到普通状态
        {
            targetForce = maxForce;
        }

        //每秒60递减，可调
        currentForce = Mathf.MoveTowards(currentForce, targetForce, 60 * Time.fixedDeltaTime);
    }

    void Jump()
    {
        rb.AddForce(100 * Vector3.up);
    }

    private void StartDrift()
    {
        isDrifting = true;

        //根据水平输入决定漂移时车的朝向，因为合速度方向与车身方向不一致，所以为加力方向添加偏移
        if (turn < 0)
        {
            driftDirection = -1;
            //左漂移时，合速度方向为车头朝向的右前方，偏移具体数值需结合实际自己调试
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

        RaycastHit frontHit;
        RaycastHit rearHit;
        RaycastHit rightHit;
        RaycastHit leftHit;

        bool hasfront = Physics.Raycast(transform.position + new Vector3(0, 0, 2), -transform.up, out frontHit, 1.1f);
        bool hasrear = Physics.Raycast(transform.position + new Vector3(0, 0, -2), -transform.up, out rearHit, 1.1f);
        bool hasright = Physics.Raycast(transform.position + new Vector3(1, 0, 0), -transform.up, out rightHit, 1.1f);
        bool hasleft = Physics.Raycast(transform.position + new Vector3(-1, 0, 0), -transform.up, out leftHit, 1.1f);

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
        Vector3 HNormal = (frontHit.normal + rearHit.normal).normalized;
        Quaternion HQuaternion = Quaternion.FromToRotation(transform.up, HNormal);

        //水平方向與地面水平
        VStream = VQuaternion * VStream;
        transform.GetComponent<Rigidbody>().MoveRotation(VStream);
        HStream = HQuaternion * HStream;
        transform.GetComponent<Rigidbody>().MoveRotation(HStream);
    }

    private void Relife(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("break");
        Debug.Log(gameObject.name);
        string[] N = gameObject.name.Split('(');
        //ResetCar(車名, 生成位置, 車頭方向);
        //carevent.ResetCar(N[0], checkPoint, );
    }
}
