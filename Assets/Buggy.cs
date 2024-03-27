// Source: https://github.com/bitgalaxis/WheelColliders

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 

public class Buggy : MonoBehaviour
{
    public Transform gravityTarget;

    public float power = 15000f;
    public float torque = 500f;
    public float gravity = 9.81f;

    public bool autoOrient = false;
    public float autoOrientSpeed = 1f;

    private float horInput;
    private float verInput;
    private float steerAngle;

    public Wheel[] wheels;

    private Collider[] Colliders;
    int m_CheckpointIndex;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Colliders = GameObject.FindGameObjectsWithTag("Checkpoint").OrderBy((gameObject) => gameObject.name).Select((gameObject) => gameObject.GetComponent<Collider>()).ToArray();
    }

    void Update() 
    {
        ProcessInput();
        Vector3 diff = transform.position - gravityTarget.position;
        if(autoOrient) { AutoOrient(-diff); }
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_CheckpointIndex = Random.Range(0, Colliders.Length - 1);
            var collider = Colliders[m_CheckpointIndex];
            Debug.Log(collider.transform.position.ToString());
            //transform.localRotation = collider.transform.rotation;
            //transform.position = collider.transform.position;


            rb.Sleep();
            rb.gameObject.SetActive(false);
            rb.isKinematic = true;
            rb.transform.SetPositionAndRotation(collider.transform.position - new Vector3(0, 1, 0), collider.transform.rotation);
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.gameObject.SetActive(true);
            rb.WakeUp();
        }
    }

    void FixedUpdate()
    {
        ProcessForces();
        //ProcessGravity();
    }

    void ProcessInput()
    {
        verInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");
    }

    void ProcessForces()
    {
        //Vector3 force = new Vector3(0f, 0f, verInput * power);
        //rb.AddRelativeForce(force);

        //Vector3 rforce = new Vector3(0f, horInput* torque, 0f);
        //rb.AddRelativeTorque(rforce);

        foreach(Wheel w in wheels)
        {
            w.Steer(horInput);
            w.Accelerate(verInput * power);
            w.UpdatePosition();
        }
    }

    void ProcessGravity()
    {
        Vector3 diff = transform.position - gravityTarget.position;
        rb.AddForce(-diff.normalized * gravity * (rb.mass));
    }

    void AutoOrient(Vector3 down)
    {
        Quaternion orientationDirection = Quaternion.FromToRotation(-transform.up, down) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, orientationDirection, autoOrientSpeed * Time.deltaTime);
    }

    private void OnGUI()
    {
        GUILayout.Label("velocity: " + rb.velocity.ToString());
        GUILayout.Label("angularVelocity: " + rb.angularVelocity.ToString());
    }
}