using Cinemachine.Utility;
using EdyCommonTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VehiclePhysics;

public class TargetFollower : MonoBehaviour
{
    VPVehicleController m_Car;
    Rigidbody m_rb;
    VPStandardInput m_Input;
    SplineFollower target;
    SplineFollowerChaser followerChaser;

    // Start is called before the first frame update
    void Start()
    {
        m_Car = GetComponent<VPVehicleController>();
        m_rb = GetComponent<Rigidbody>();
        m_Input = GetComponent<VPStandardInput>();
        followerChaser = GetComponent<SplineFollowerChaser>();
        target = GetComponent<SplineFollowerChaser>().target;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromTarget = Vector3.Distance(Vector3.ProjectOnPlane(target.transform.position, Vector3.up), Vector3.ProjectOnPlane(m_rb.position, Vector3.up));
        m_Input.externalBrake = Mathf.InverseLerp(followerChaser.minDistance, 0, distanceFromTarget);

        m_Input.externalThrottle = Mathf.InverseLerp(followerChaser.minDistance, followerChaser.maxDistance, distanceFromTarget);

        float angle = Vector3.Angle(Vector3.ProjectOnPlane(m_rb.velocity, Vector3.up), Vector3.ProjectOnPlane(target.transform.position - m_rb.position, Vector3.up));
        //if (angle > 90)
        //{
        //    m_Input.externalBrake = 1f;
        //    m_Input.externalThrottle = 0f;
        //}

        m_Input.externalSteer = Mathf.InverseLerp(0, followerChaser.outerAngle, angle) * 
            Mathf.Sign(Vector3.Cross(Vector3.ProjectOnPlane(m_rb.velocity, Vector3.up), Vector3.ProjectOnPlane(target.transform.position - m_rb.position, Vector3.up)).y);

    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 300, 300, 200));
        GUILayout.Label("externalThrottle: " + m_Input.externalThrottle.ToString("0.00"));
        GUILayout.Label("externalSteer: " + m_Input.externalSteer.ToString("0.00"));
        GUILayout.Label("externalBrake: " + m_Input.externalBrake.ToString("0.00"));
        GUILayout.Label("Angle: " + Vector3.Angle(Vector3.ProjectOnPlane(m_rb.velocity, Vector3.up), Vector3.ProjectOnPlane(target.transform.position - m_rb.position, Vector3.up)).ToString("0.0"));

        float distanceFromTarget = Vector3.Distance(Vector3.ProjectOnPlane(target.transform.position, Vector3.up), Vector3.ProjectOnPlane(m_rb.position, Vector3.up));
        GUILayout.Label("distanceFromTarget: " + distanceFromTarget);
        GUILayout.Label("position: " + m_rb.position);
        GUILayout.Label("position: " + m_rb.gameObject.transform.position);
        GUILayout.EndArea();
    }
}
