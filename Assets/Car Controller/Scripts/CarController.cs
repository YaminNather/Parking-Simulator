using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CarController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Start Function Called");
    }

    void Update()
    {
        fCalcVel(mInput.y, mInput.x);

        fApplyVel();

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
        mCurAngularVel += mAngularAcc * Vector3.up * horizontalInput * Time.deltaTime;

        if (verticalInput == 0.0f) 
            mCurVel.z = fApplyDeccel(mCurVel.z, mDecc);

        if (horizontalInput == 0.0f)
            mCurAngularVel.y = fApplyDeccel(mCurAngularVel.y, mAngularDecc);

        mCurVel.z = fClampVel(mCurVel.z, mMaxVel);
        mCurAngularVel.y = fClampVel(mCurAngularVel.y, mMaxAngularVel);
    }

    float fClampVel(float curVel, float maxMagnitude)
    {
        curVel = Mathf.Clamp(curVel, -maxMagnitude, maxMagnitude);
        return(curVel);
    }

    float fApplyDeccel(float curVel, float decc)
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
    
    void fApplyVel()
    {
        transform.position += transform.rotation * (mCurVel * Time.deltaTime);

        transform.Rotate(mCurAngularVel.x, mCurAngularVel.y, mCurAngularVel.z);
    }

    #region Variables
    [Header("Movement")]
    [SerializeField] private float mAcc;
    [SerializeField] private float mDecc;
    [SerializeField] private Vector3 mCurVel;
    [SerializeField] private float mMaxVel;

    [Header("Rotation")]
    [SerializeField] private float mAngularAcc;

    [SerializeField] private float mAngularDecc;
    [SerializeField] private Vector3 mCurAngularVel;
    [SerializeField] private float mMaxAngularVel;

    private Vector2 mInput;

    #endregion
}
