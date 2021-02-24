using UnityEngine;

public class CarController : MonoBehaviour
{
    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        Debug.Log("Start Function Called");
    }

    private void LateUpdate()
    {
        fCalcVel(mInput.y, mInput.x);

        fApplyVel();

        fRotateSteeringWheel();

        mInput = Vector3.zero;
    }

    public void fAddToInput(Vector2 input)
    {
        mInput += input;

        mInput.x = Mathf.Clamp(mInput.x, -1.0f, 1.0f);
        mInput.y = Mathf.Clamp(mInput.y, -1.0f, 1.0f);
    }

    private void fCalcVel(float verticalInput, float horizontalInput)
    {
        mCurVel += Vector3.forward * mAcc * verticalInput * Time.deltaTime;

        if (mCurVel.z != 0.0f)
        {
            float deltaAngularVelAmount = Mathf.Abs(mCurVel.z) / mMaxVel;
            mCurAngularVel += (mMaxAngularVel * deltaAngularVelAmount) * Vector3.up * horizontalInput * Time.deltaTime;
        }

        if (verticalInput == 0.0f)
        {
            mCurVel.z = fApplyDeccel(mCurVel.z, mDecc);
        }
        if (mCurVel.z == 0.0f || horizontalInput == 0.0f)
            mCurAngularVel.y = fApplyDeccel(mCurAngularVel.y, mAngularDecc);
        
        mCurVel.z = Mathf.Clamp(mCurVel.z, mMaxBackwardVel, mMaxVel);
        mCurAngularVel.y = Mathf.Clamp(mCurAngularVel.y, -mMaxAngularVel, mMaxAngularVel);
    }

    private static float fApplyDeccel(float curVel, float decc)
    {
        if (curVel != 0.0f)
        {
            float sign = Mathf.Sign(curVel);
            curVel += (sign * -1.0f * decc) * Time.deltaTime;

            if (Mathf.Sign(curVel) != sign)
                curVel = 0.0f;
        }

        return(curVel);
    }
    
    private void fApplyVel()
    {
        void lfapplyMovementVel()
        {
            Vector3 direction = transform.rotation * mCurVel.normalized;
            float magnitude = mCurVel.magnitude * Time.deltaTime;

            Vector3 deltaMovement = Vector3.zero;
            if (!mRigidbody.SweepTest(direction, out RaycastHit hitInfo, magnitude))
            {
                //Debug.Log("Not Colliding");

                deltaMovement = direction * magnitude;
            }

            else
            {
                //Debug.Log("Colliding");
                deltaMovement = direction * (hitInfo.distance - 0.5f);
                mCurVel = Vector3.zero;
            }

            transform.position += deltaMovement;
        }

        void lfApplyAngularVel()
        {
            //if (!fRotationalSweepTest(mCurAngularVel * Time.deltaTime, out RaycastHit hitInfo))
            //{
            //    Debug.Log("No Rotational Collision");
            //    transform.Rotate(mCurAngularVel * Time.deltaTime);
            //}
            //else
            //    Debug.Log($"Rotational Collision with {hitInfo.transform.name}", hitInfo.transform.gameObject);

            transform.Rotate(mCurAngularVel * Time.deltaTime);
        }

        lfapplyMovementVel();
        lfApplyAngularVel();
    }

    private void fRotateSteeringWheel()
    {
        mSteeringWheel.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -(mCurAngularVel.y / mMaxAngularVel * 45.0f));
    }

    private void fDrawSweepTestRay(Vector3 direction)
    {
        MeshCollider meshCollider = GetComponentInChildren<MeshCollider>();

        foreach (Vector3 vertex in meshCollider.sharedMesh.vertices)
        {
            Vector3 origin = meshCollider.transform.position + meshCollider.transform.rotation * vertex;
            Debug.DrawRay(origin, direction, Color.cyan);
        }
    }

    private bool fRotationalSweepTest(Vector3 angle, out RaycastHit hitInfo)
    {
        hitInfo = new RaycastHit();

        BoxCollider collider = GetComponentInChildren<BoxCollider>();
        Bounds bounds = collider.bounds;
        Vector3[] points = new Vector3[]
        {
            collider.center + new Vector3(collider.size.x, collider.size.y, collider.size.z) * 0.5f,
            collider.center + new Vector3(-collider.size.x, collider.size.y, collider.size.z) * 0.5f,
            collider.center + new Vector3(collider.size.x, collider.size.y, -collider.size.z) * 0.5f,
            collider.center + new Vector3(-collider.size.x, collider.size.y, -collider.size.z) * 0.5f,

            collider.center + new Vector3(collider.size.x, -collider.size.y, collider.size.z) * 0.5f,
            collider.center + new Vector3(-collider.size.x, -collider.size.y, collider.size.z) * 0.5f,
            collider.center + new Vector3(collider.size.x, -collider.size.y, -collider.size.z) * 0.5f,
            collider.center + new Vector3(-collider.size.x, -collider.size.y, -collider.size.z) * 0.5f
        };

        for (int i = 0; i < points.Length; i++)
            points[i] = collider.transform.TransformPoint(points[i]);

        RaycastHit? leastDistanceHitInfo = null;
        foreach (Vector3 point in points)
        {
            Ray ray = new Ray(point, Quaternion.Euler(collider.transform.eulerAngles + angle) * (point - collider.transform.TransformPoint(collider.center)) - point);
            Debug.DrawRay(ray.origin, ray.direction, Color.cyan);

            if (lfcustomRaycast(ray, out RaycastHit curHitInfo, gameObject))
            {
                if (leastDistanceHitInfo == null)
                {
                    leastDistanceHitInfo = curHitInfo;
                    continue;
                }

                if (curHitInfo.distance < leastDistanceHitInfo?.distance)
                    leastDistanceHitInfo = curHitInfo;
            }
        }

        if (leastDistanceHitInfo == null)
        {
            return false;
        }
        
        hitInfo = (RaycastHit)leastDistanceHitInfo;
        return true;

        bool lfcustomRaycast(Ray ray, out RaycastHit raycastHit, GameObject toIgnore)
        {
            raycastHit = new RaycastHit();

            RaycastHit[] hitInfos = Physics.RaycastAll(ray.origin, ray.direction);

            if (hitInfos.Length == 0)
                return false;

            RaycastHit? smallestHitInfoForPoint = lfgetSmallestRaycastHit(hitInfos, toIgnore.gameObject);

            if (smallestHitInfoForPoint == null)
                return false;

            raycastHit = (RaycastHit)smallestHitInfoForPoint;
            return true;

        }

        RaycastHit? lfgetSmallestRaycastHit(RaycastHit[] hitInfos, GameObject toIgnore)
        {
            if (hitInfos.Length == 1 && hitInfos[0].transform.gameObject == toIgnore)
                return null;

            RaycastHit r = (hitInfos[0].transform.gameObject != toIgnore) ? hitInfos[0] : hitInfos[1];
            foreach (RaycastHit hitInfo in hitInfos)
            {
                if (hitInfo.transform.gameObject == toIgnore)
                    continue;

                if (hitInfo.distance < r.distance)
                    r = hitInfo;
            }

            return r;
        }
    }

    #region Variables
    private Rigidbody mRigidbody;

    [Header("Movement")]
    [SerializeField] private float mAcc;
    [SerializeField] private float mBackwardAcc;
    [SerializeField] private float mMaxVel;
    [SerializeField] private float mMaxBackwardVel;
    [SerializeField] private float mDecc;
    [SerializeField] private Vector3 mCurVel;

    [Header("Rotation")]
    [SerializeField] private float mMaxAngularVel;
    [SerializeField] private float mAngularDecc;
    [SerializeField] private Vector3 mCurAngularVel;

    [Header("Steering Wheel Stuff")] 
    [SerializeField] private GameObject mSteeringWheel;

    private Vector2 mInput;
    #endregion
}
