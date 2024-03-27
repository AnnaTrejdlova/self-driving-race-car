// Source: https://github.com/bitgalaxis/WheelColliders

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool powered = false;
    public float maxAngle = 90f;
    public float offset = 0f;

    private float turnAngle;
    private WheelCollider wcol;
    public Transform wheelMesh;

    private void Start()
    {
        wcol = GetComponent<WheelCollider>();
    }

    public void Steer(float steerInput)
    {
        turnAngle = steerInput * maxAngle + offset;
        wcol.steerAngle = turnAngle;
    }

    public void Accelerate(float powerInput)
    {
        if(powered) wcol.motorTorque = powerInput;
        else wcol.brakeTorque = 0;
    }

    public void UpdatePosition()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        wcol.GetWorldPose(out pos, out rot);
        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
    }

    private void OnGUI()
    {
        switch (gameObject.name)
        {
            case "FL":
                GUILayout.BeginArea(new Rect(0, 100, 200, 200));
                break;
            case "FR":
                GUILayout.BeginArea(new Rect(1000, 100, 200, 200));
                break;
            case "RL":
                GUILayout.BeginArea(new Rect(0, 400, 200, 200));
                break;
            case "RR":
                GUILayout.BeginArea(new Rect(1000, 400, 200, 200));
                break;
        }
        GUILayout.Label("brakeTorque: " + wcol.brakeTorque);
        GUILayout.Label("isGrounded: " + wcol.isGrounded);
        GUILayout.Label("motorTorque: " + wcol.motorTorque);
        GUILayout.Label("rotationSpeed: " + wcol.rotationSpeed);
        GUILayout.Label("rpm: " + wcol.rpm);
        GUILayout.Label("sprungMass: " + wcol.sprungMass);
        GUILayout.Label("steerAngle: " + wcol.steerAngle);
        GUILayout.EndArea();
    }
}
