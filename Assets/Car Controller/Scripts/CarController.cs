using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
            //if (!fRotationalSweepTest(mCurAngularVel, out RaycastHit hitInfo))
            //{
            //    Debug.Log("No Rotational Collision");
            //    transform.Rotate(new Vector3(mCurAngularVel.x, mCurAngularVel.y, mCurAngularVel.z) * Time.deltaTime);
            //}
            //else
            //    Debug.Log($"Rotational Collision with {hitInfo.transform.name}", hitInfo.transform.gameObject);

            transform.Rotate(new Vector3(mCurAngularVel.x, mCurAngularVel.y, mCurAngularVel.z) * Time.deltaTime);
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
        Collider collider = GetComponentInChildren<Collider>();
        Bounds bounds = collider.bounds;
        Vector3[] points = new Vector3[]
        {
            bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z),
            bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z),
            bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z),
            bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z),

            bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z),
            bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z),
            bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z),
            bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z)
        };

        RaycastHit? leastDistanceHitInfo = null;
        foreach (Vector3 point in points)
        {
            Ray ray = new Ray(point, Quaternion.Euler(angle) * (point - bounds.center) - point);
            if (Physics.Raycast(ray, out RaycastHit curPointHitInfo))
            {
                if (leastDistanceHitInfo == null)
                {
                    leastDistanceHitInfo = curPointHitInfo;
                }
                else
                {
                    if (curPointHitInfo.distance < leastDistanceHitInfo?.distance)
                        leastDistanceHitInfo = curPointHitInfo;
                }
            }
        }

        if (leastDistanceHitInfo != null)
        {
            hitInfo = (RaycastHit)leastDistanceHitInfo;
            return true;
        }

        else
        {
            hitInfo = new RaycastHit();
            return false;
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
