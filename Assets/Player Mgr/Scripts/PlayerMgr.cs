using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMgr : MonoBehaviour
{
    private void Start()
    {
        mCarController = GetComponent<CarController>();
    }

    private void Update()
    {
        mCarController.fAddToInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    }

    private CarController mCarController;
}
