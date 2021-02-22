using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyChecker : MonoBehaviour
{
    private void Update()
    {
        Color color = Color.green;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward.normalized;

        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, mDistance))
            color = Color.red;

        Debug.DrawRay(origin, direction * mDistance, color);
    }

    public void fSetDistance(float value) => mDistance = value;

    #region Variables
    private float mDistance = 5.0f;
    #endregion
}
