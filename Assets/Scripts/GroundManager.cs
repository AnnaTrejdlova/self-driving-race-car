using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class GroundManager : MonoBehaviour
{
    VPWheelCollider wheelCollider;
    VPVehicleController vehicleController;
    WheelHit hit;
    int wheelIndex;

    // Start is called before the first frame update
    void Start()
    {
        wheelCollider = GetComponent<VPWheelCollider>();
        vehicleController = gameObject.transform.parent.parent.GetComponent<VPVehicleController>();

        for (int i = 0; i < 4; i++)
        {
            if (wheelCollider == vehicleController.wheelState[i].wheelCol)
            {
                wheelIndex = i;
            }
        }
        Debug.Log("Wheel index: " + wheelIndex);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(vehicleController.GetWheelTireFriction(0).frictionMultiplier);
        wheelCollider.GetGroundHit(out hit);
        //Debug.Log(hit.collider.material.dynamicFriction);
        if (hit.collider)
        {
            vehicleController.GetWheelTireFriction(wheelIndex).frictionMultiplier = hit.collider.material.dynamicFriction;
            //Debug.Log(vehicleController.GetWheelTireFriction(wheelIndex).frictionMultiplier);
        }
    }
}
