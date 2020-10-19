//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CarTest : MonoBehaviour
//{
//    bool isGround;
//    bool isGroundLastFrame;
//    bool isDrifting;
//    float boostForce;

//    Rigidbody kartRigidbody;
//    // Start is called before the first frame update
//    void Start()
//    {
//        kartRigidbody = transform.GetComponent<Rigidbody>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //按下空格起跳
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            if (isGround)   //如果在地上
//            {
//                Jump();
//            }
//        }

//        //按住空格，并且有水平输入：开始漂移
//        if (Input.GetKey(KeyCode.Space) && h_Input != 0)
//        {
//            //落地瞬间、不在漂移并且速度大于一定值时开始漂移
//            if (isGround && !isGroundLastFrame && !isDrifting && kartRigidbody.velocity.sqrMagnitude > 10)
//            {
//                StartDrift();   //开始漂移
//            }
//        }

//        //放开空格：漂移结束
//        if (Input.GetKeyUp(KeyCode.Space))
//        {
//            if (isDrifting)
//            {
//                Boost(boostForce);//加速
//                StopDrift();//停止漂移
//            }
//        }
//    }

//    private void FixedUpdate()
//    {
//        //车转向
//        CheckGroundNormal();        //检测是否在地面上，并且使车与地面保持水平
//        Turn();                     //输入控制左右转向

//        //起步时 力递增
//        IncreaseForce();
//        //漂移加速后/松开加油键 力递减
//        ReduceForce();


//        //如果在漂移
//        if (isDrifting)
//        {
//            CalculateDriftingLevel();   //计算漂移等级
//            ChangeDriftColor();         //根据漂移等级改变颜色
//        }

//        //根据上述情况，进行最终的旋转和加力
//        kartRigidbody.MoveRotation(rotationStream);
//        //计算力的方向
//        CalculateForceDir();
//        //移动
//        AddForceToMove();
//    }

//    //偵測是否在地上(完成)
//    public void CheckGroundNormal()
//    {
//        Quaternion VStream = transform.GetComponent<Rigidbody>().rotation;
//        Quaternion HStream = transform.GetComponent<Rigidbody>().rotation;

//        RaycastHit frontHit;
//        RaycastHit rearHit;
//        RaycastHit rightHit;
//        RaycastHit leftHit;

//        bool hasfront = Physics.Raycast(transform.position + new Vector3(0, 0, 2), -transform.up, out frontHit, 1.1f);
//        bool hasrear = Physics.Raycast(transform.position + new Vector3(0, 0, -2), -transform.up, out rearHit, 1.1f);
//        bool hasright = Physics.Raycast(transform.position + new Vector3(1, 0, 0), -transform.up, out rightHit, 1.1f);
//        bool hasleft = Physics.Raycast(transform.position + new Vector3(-1, 0, 0), -transform.up, out leftHit, 1.1f);

//        //print(frontHit.transform.gameObject.layer);
//        if (hasfront || hasrear || hasright || hasleft)
//            isGround = true;
//        //Debug.DrawLine(transform.position + new Vector3(0, 0, 2), frontHit.point);
//        else
//            isGround = false;
//            //Debug.Log("no no no");

//        //垂直方向與地面水平
//        Vector3 VNormal = (frontHit.normal + rearHit.normal).normalized;
//        Quaternion VQuaternion = Quaternion.FromToRotation(transform.up, VNormal);
//        Vector3 HNormal = (frontHit.normal + rearHit.normal).normalized;
//        Quaternion HQuaternion = Quaternion.FromToRotation(transform.up, HNormal);

//        //水平方向與地面水平
//        VStream = VQuaternion * VStream;
//        transform.GetComponent<Rigidbody>().MoveRotation(VStream);
//        HStream = HQuaternion * HStream;
//        transform.GetComponent<Rigidbody>().MoveRotation(HStream);
//    }

//    //轉彎
//    public void Turn()
//    {
//        //只能在移动时转弯
//        if (kartRigidbody.velocity.sqrMagnitude <= 0.1)
//        {
//            return;
//        }

//        //漂移时自带转向
//        if (driftDirection == DriftDirection.Left)
//        {
//            rotationStream = rotationStream * Quaternion.Euler(0, -40 * Time.fixedDeltaTime, 0);
//        }
//        else if (driftDirection == DriftDirection.Right)
//        {
//            rotationStream = rotationStream * Quaternion.Euler(0, 40 * Time.fixedDeltaTime, 0);
//        }

//        //后退时左右颠倒
//        float modifiedSteering = Vector3.Dot(kartRigidbody.velocity, transform.forward) >= 0 ? h_Input : -h_Input;

//        //输入可控转向：如果在漂移，可控角速度为30，否则平常状态为60.
//        turnSpeed = driftDirection != DriftDirection.None ? 30 : 60;
//        float turnAngle = modifiedSteering * turnSpeed * Time.fixedDeltaTime;
//        Quaternion deltaRotation = Quaternion.Euler(0, turnAngle, 0);

//        rotationStream = rotationStream * deltaRotation;//局部坐标下旋转,这里有空换一个简单的写法
//    }

//    //计算加力方向
//    public void CalculateForceDir()
//    {
//        //往前加力
//        if (v_Input > 0)
//        {
//            verticalModified = 1;
//        }
//        else if (v_Input < 0)//往后加力
//        {
//            verticalModified = -1;
//        }

//        forceDir_Horizontal = m_DriftOffset * transform.forward;
//    }

//    //加力移动
//    public void AddForceToMove()
//    {
//        //计算合力
//        Vector3 tempForce = verticalModified * currentForce * forceDir_Horizontal;

//        if (!isGround)  //如不在地上，则加重力
//        {
//            tempForce = tempForce + gravity * Vector3.down;
//        }

//        kartRigidbody.AddForce(tempForce, ForceMode.Force);
//    }

//    void Jump()
//    {

//    }

//    void StartDrift()
//    {
//        isDrifting = true;

//        //根据水平输入决定漂移时车的朝向，因为合速度方向与车身方向不一致，所以为加力方向添加偏移
//        if (h_Input < 0)
//        {
//            driftDirection = DriftDirection.Left;
//            //左漂移时，合速度方向为车头朝向的右前方，偏移具体数值需结合实际自己调试
//            m_DriftOffset = Quaternion.Euler(0f, 30, 0f);
//        }
//        else if (h_Input > 0)
//        {
//            driftDirection = DriftDirection.Right;
//            m_DriftOffset = Quaternion.Euler(0f, -30, 0f);
//        }

//        //播放漂移粒子特效
//        PlayDriftParticle();
//    }

//    void StopDrift()
//    {
//        isDrifting = false;
//        driftDirection = DriftDirection.None;
//        driftPower = 0;
//        m_DriftOffset = Quaternion.identity;
//        StopDriftParticle();
//    }

//    //加速
//    public void Boost(float boostForce)
//    {
//        //按照漂移等级加速：1 / 1.1 / 1.2
//        currentForce = (1 + (int)driftLevel / 10) * boostForce;
//        EnableTrail();
//    }

//    //力递减
//    public void ReduceForce()
//    {
//        float targetForce = currentForce;
//        if (isGround && v_Input == 0)
//        {
//            targetForce = 0;
//        }
//        else if (currentForce > normalForce)    //用于加速后回到普通状态
//        {
//            targetForce = normalForce;
//        }

//        if (currentForce <= normalForce)
//        {
//            DisableTrail();
//        }

//        //每秒60递减，可调
//        currentForce = Mathf.MoveTowards(currentForce, targetForce, 60 * Time.fixedDeltaTime);
//    }

//    public void CalculateDriftingLevel()
//    {
//        driftPower += Time.fixedDeltaTime;
//        //0.7秒提升一个漂移等级
//        if (driftPower < 0.7)
//        {
//            driftLevel = DriftLevel.One;
//        }
//        else if (driftPower < 1.4)
//        {
//            driftLevel = DriftLevel.Two;
//        }
//        else
//        {
//            driftLevel = DriftLevel.Three;
//        }
//    }

//    public void ChangeDriftColor()
//    {
//        foreach (var tempParticle in wheelsParticeles)
//        {
//            var t = tempParticle.main;
//            t.startColor = driftColors[(int)driftLevel];
//        }
//    }
//}
