using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Start Function Called");
    }

    private void Update()
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
        if (horizontalInput == 0.0f)
            mCurAngularVel.y = fApplyDeccel(mCurAngularVel.y, mAngularDecc);
        
        mCurVel.z = Mathf.Clamp(mCurVel.z, mMaxBackwardVel, mMaxVel);
        mCurAngularVel.y = Mathf.Clamp(mCurAngularVel.y, -mMaxAngularVel, mMaxAngularVel);
    }

    private float fApplyDeccel(float curVel, float decc)
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
        transform.position += transform.rotation * (mCurVel * Time.deltaTime);

        transform.Rotate(mCurAngularVel.x, mCurAngularVel.y, mCurAngularVel.z);
    }

    private void fRotateSteeringWheel()
    {
        mSteeringWheel.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -(mCurAngularVel.y / mMaxAngularVel * 45.0f));
    }

    #region Variables
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
