using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTest : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody = null;

    //input
    float inputPush;
    float inputTorque;

    [Header("Suspension")]
    //懸吊點位置
    [SerializeField] Vector3 suspensionPoint = new Vector3(1.8f, 0, 1f);
    //懸吊長度
    [SerializeField] float suspensionStrech = 1;
    //地面的layer
    [SerializeField] LayerMask groundLayer = 0;
    //儲存懸吊資料
    Suspensions[] suspensions = new Suspensions[4];
    struct Suspensions
    {
        public Transform suspensions;
        public float lastLenght;

        public Suspensions(Transform sus, float lenght = 0)
        {
            this.suspensions = sus;
            this.lastLenght = lenght;
        }

        public void SetLastSuspensionLength(float length)
        {
            this.lastLenght = length;
        }
    }

    [Header("SuspensionCalculate")]
    // 彈力係數
    [SerializeField] float k = 5f;
    // 阻力
    [SerializeField] float damping = 0.5f;

    [Header("CarPush")]
    [SerializeField] float speed = 80f;
    [SerializeField] float maxumnSpeed = 10f;
    [SerializeField] float dragScale = 5f;

    [Header("CarRotate")]
    [SerializeField] float anglearSpeed = 2f;
    [SerializeField] float maxumnAnglearSpeed = 3f;
    // 角度修正率
    [SerializeField] float angularFixedRate = 0.5f;

    [Header("physics Effect")]
    [SerializeField] float inertiaDumpScale = 100f;
    float lastInputPush;

    private void OnDrawGizmos()
    {
        // 在編輯畫面顯示懸吊系統
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    var point = new Vector3(i * suspensionPoint.x / 2, suspensionPoint.y, j * suspensionPoint.z / 2);
                    point = transform.rotation * point;
                    point = transform.position + point;

                    Gizmos.DrawSphere(point, 0.05f);
                    Gizmos.DrawLine(point, (-transform.up * suspensionStrech) + point);
                }
            }
        }
    }

    private void Start()
    {
        GenerateSuspension();

        // 設定最大角向量
        rigidbody.maxAngularVelocity = maxumnAnglearSpeed;
    }

    private void Update()
    {
        inputPush = Input.GetAxis("Vertical");
        inputTorque = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        RaycastHit[] compressionInfo = SuspenionBounce();

        bool wheelContact = PushForce(compressionInfo);

        RotateTorque(wheelContact);

        PhysicalsFixed(wheelContact);
        lastInputPush = inputPush;
    }


    private void GenerateSuspension()
    {
        int suspensionIndex = 0;

        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                var point = new Vector3(i * suspensionPoint.x / 2, suspensionPoint.y, j * suspensionPoint.z / 2);
                point = transform.rotation * point;
                point = transform.position + point;

                // 生成懸吊點
                GameObject suspension = new GameObject();
                suspension.name = "suspension: " + point;
                suspension.transform.parent = transform;
                suspension.transform.position = point;
                suspension.transform.rotation = transform.rotation;
                suspension.transform.localScale = Vector3.one;

                // 懸吊點存入陣列
                suspensions[suspensionIndex] = new Suspensions(suspension.transform);

                suspensionIndex++;
            }
        }
    }

    private RaycastHit[] SuspenionBounce()
    {
        RaycastHit[] SuspensionInfo = new RaycastHit[4];

        for (int i = 0; i < suspensions.Length; i++)
        {
            Transform suspensionPoint = suspensions[i].suspensions;
            float lastSuspensionLenght = suspensions[i].lastLenght;

            // 偵測懸吊
            Ray suspensionRay = new Ray(suspensionPoint.position, -suspensionPoint.up);
            RaycastHit hit;
            bool compression = Physics.Raycast(suspensionRay, out hit, suspensionStrech, groundLayer);

            // 懸吊距離
            float suspensionLength = (hit.distance != 0) ? hit.distance : suspensionStrech;

            if (compression)
            {
                float m = rigidbody.mass;
                float a = 9.18f;

                // 每個點的施平均施力中間值
                float pointForce = (m * a) / suspensions.Length;

                // 懸吊壓縮比率
                float compressionRatio = (suspensionStrech - suspensionLength) / suspensionStrech;

                // 彈簧施力比
                float suspensionForceRatio = (1f / 2f) * k * compressionRatio * compressionRatio;

                // 懸吊施力
                float suspensionForce = pointForce * 2 * suspensionForceRatio;

                // 彈簧阻力
                float verticalVelocity = suspensionLength - lastSuspensionLenght;
                float verticalAddForce = suspensionForce;

                bool addDamping = (verticalVelocity > 0 && verticalAddForce > 0) || (verticalVelocity < 0 && verticalAddForce < 0);
                float suspensionDamping = addDamping ? suspensionForce * damping : 0;

                float addForce = suspensionForce - suspensionDamping;

                // 施力
                Vector3 suspensionAddForce = suspensionPoint.up * addForce;
                rigidbody.AddForceAtPosition(suspensionAddForce, suspensionPoint.position);

                suspensions[i].SetLastSuspensionLength(suspensionLength);
            }

            SuspensionInfo[i] = hit;
        }

        return SuspensionInfo;
    }

    // 回傳有沒有輪子碰地
    private bool PushForce(RaycastHit[] compressionInfo)
    {
        bool wheelContact = false;

        for (int i = 0; i < compressionInfo.Length; i++)
        {
            RaycastHit hit = compressionInfo[i];
            Vector3 normal = hit.normal;

            wheelContact = wheelContact ? true : (hit.collider != null);

            // 確認地板
            if (hit.collider == null) { continue; }

            Vector3 pushForceDirction = Vector3.ProjectOnPlane(transform.forward, hit.normal);

            // Push
            Vector3 pushForce = pushForceDirction * inputPush * speed;
            rigidbody.AddForce(pushForce);

            // 速度限制
            if (rigidbody.velocity.magnitude > maxumnSpeed)
                rigidbody.velocity = rigidbody.velocity.normalized * maxumnSpeed;
        }

        return wheelContact;
    }

    private void RotateTorque(bool whellContact)
    {
        // 輪子有無碰地面
        if (!whellContact) { return; }

        float speedScale2AngleVelocity = rigidbody.velocity.magnitude / maxumnSpeed;
        float rotateSpeed = anglearSpeed * inputTorque * inputPush * speedScale2AngleVelocity;
        Vector3 angleSpeed = transform.up * rotateSpeed;
        rigidbody.AddRelativeTorque(angleSpeed, ForceMode.Impulse);

        Vector3 angularVelocity2Fixed = transform.InverseTransformDirection(rigidbody.angularVelocity);
        float angluarDir2Fixed = (angularVelocity2Fixed.y > 0) ? 1 : -1;
        float fixedScale = Mathf.Abs(angularVelocity2Fixed.y / maxumnAnglearSpeed);
        Vector3 angleFixed = -(transform.up * angluarDir2Fixed * angularFixedRate * fixedScale);
        rigidbody.AddRelativeTorque(angleFixed, ForceMode.Impulse);
    }

    private void PhysicalsFixed(bool wheelContact)
    {
        if (!wheelContact) { return; }

        Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        localVelocity.y = 0;
        Vector3 velocityDrag = transform.TransformVector(localVelocity);
        rigidbody.AddForce(-velocityDrag * dragScale);

        float inputGap = lastInputPush - inputPush;
        float speedScale2PhysicsEffect = rigidbody.velocity.magnitude / maxumnSpeed;
        Vector3 effectTorque = transform.right * inputGap * speedScale2PhysicsEffect * inertiaDumpScale;
        rigidbody.AddTorque(effectTorque);
    }
}
