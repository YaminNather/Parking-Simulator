using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SafetyChecker
{
    public class SafetyChecker : MonoBehaviour
    {
        private void Update()
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward.normalized;

            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, mWarningDistance))
            {
                SafetyCheckListener safetyCheckListener = hitInfo.transform.GetComponentInChildren<SafetyCheckListener>();
                if (safetyCheckListener == null)
                    safetyCheckListener = hitInfo.transform.GetComponentInParent<SafetyCheckListener>();


                if (safetyCheckListener != null)
                {
                    Color debugRayColor = Color.yellow;
                    
                    if(hitInfo.distance <= mDangerDistance)
                    {
                        debugRayColor = Color.red;
                        safetyCheckListener.fRecieveRayFromSafetyChecker(Ranges.Danger);
                    }
                    else
                        safetyCheckListener.fRecieveRayFromSafetyChecker(Ranges.Warning);

                    Debug.DrawRay(origin, direction * mWarningDistance, debugRayColor);
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * mDangerDistance, Color.red);
                Debug.DrawRay(origin + (direction * mDangerDistance), direction * (mWarningDistance - mDangerDistance), Color.yellow);
            }
        }

        public void fDangerDistance(float value) => mDangerDistance = value;
        public void fWarningDistance(float value) => mWarningDistance = value;

        #region Variables
        private float mWarningDistance = 2.0f;
        private float mDangerDistance = 5.0f;
        #endregion
    }
}
