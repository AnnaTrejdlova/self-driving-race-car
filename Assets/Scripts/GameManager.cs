using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class GameManager : MonoBehaviour
{
    VPVehicleController vehicle;

    // Start is called before the first frame update, gets also called after unpausing
    public void Start()
    {
        vehicle = FindObjectOfType<VPVehicleController>();

        if (ApplicationModel.transmission == Transmission.Manual)
        {
            //vehicle.gearbox.type = Gearbox.Type.Manual;
            vehicle.gearbox.autoShift = false;
        }
        else if (ApplicationModel.transmission == Transmission.Automatic)
        {
            vehicle.gearbox.autoShift = true;
        }

        vehicle.antiLock.enabled = ApplicationModel.assists[Assists.ABS];
        vehicle.tractionControl.enabled = ApplicationModel.assists[Assists.TSC];
        vehicle.stabilityControl.enabled = ApplicationModel.assists[Assists.ESC];
        vehicle.antiSpin.enabled = ApplicationModel.assists[Assists.ASR];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
