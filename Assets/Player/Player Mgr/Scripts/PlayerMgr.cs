using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerMgr : MonoBehaviour
{
    private void Start()
    {
        mCam = GetComponentInChildren<Camera>();
        mCarController = GetComponent<CarController>();

        fSetSafetyCheckersDistance(mDangerCheckDistance, mWarningCheckDistance);
    }

    private void Update()
    {
        mCarController.fAddToInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        mCam.transform.Rotate(new Vector3(0.0f, Input.GetAxis("Mouse X") * mCamSens, 0.0f) * Time.deltaTime);
    }

    private void fSetSafetyCheckersDistance(float dangerDistance, float warningDistance)
    {
        foreach (SafetyChecker.SafetyChecker sc in GetComponentsInChildren<SafetyChecker.SafetyChecker>())
        {
            sc.fWarningDistance(warningDistance);
            sc.fDangerDistance(dangerDistance);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        fSetSafetyCheckersDistance(mDangerCheckDistance, mWarningCheckDistance);
    }
#endif

    #region Variables
    private CarController mCarController;

    [Header("Camera Stuff")]
    private Camera mCam = null;
    [SerializeField] private float mCamSens = 45.0f;

    [Header("Safety Checker Stuff")] 
    [SerializeField] private float mWarningCheckDistance = 5.0f;
    [SerializeField] private float mDangerCheckDistance = 2.0f;

    #endregion
}
