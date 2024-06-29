using KartGame.KartSystems;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Random = UnityEngine.Random;
using VehiclePhysics;
using System.Linq;
using System.Threading.Tasks;
using EdyCommonTools;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;
using UnityEditor.Rendering;

namespace KartGame.AI.Custom
{
    /// <summary>
    /// We only want certain behaviours when the agent runs.
    /// Training would allow certain functions such as OnAgentReset() be called and execute, while Inferencing will
    /// assume that the agent will continuously run and not reset.
    /// </summary>
    public enum AgentMode
    {
        Training,
        Inferencing
    }

    /// <summary>
    /// The CarAgent will drive the inputs for the VPVehicleController.
    /// </summary>
    public class CarAgent : Agent
    {
#region Training Modes
        [Tooltip("Are we training the agent or is the agent production ready?")]
        public AgentMode Mode = AgentMode.Training;
        [Tooltip("What is the initial checkpoint the agent will go to? This value is only for inferencing.")]
        public ushort InitCheckpointIndex;

#endregion

#region Senses
        [Header("Observation Params")]
        [Header("Checkpoints"), Tooltip("What are the series of checkpoints for the agent to seek and pass through?")]
        public Collider[] Colliders;
        [Tooltip("What layer are the checkpoints on? This should be an exclusive layer for the agent to use.")]
        public LayerMask CheckpointMask;
        public Spline raceLine;
        #endregion

        #region Rewards
        [Header("Rewards"), Tooltip("What penatly is given when the agent crashes?")]
        public float HitPenalty = -1f;
        [Tooltip("How much reward is given when the agent successfully passes the checkpoints?")]
        public float PassCheckpointReward;
        [Tooltip("Should typically be a small value, but we reward the agent for moving in the right direction.")]
        public float TowardsCheckpointReward;
        [Tooltip("Typically if the agent moves faster, we want to reward it for finishing the track quickly.")]
        public float SpeedReward;
        [Tooltip("Reward the agent when it keeps accelerating")]
        public float AccelerationReward;
        #endregion
        Dictionary<string, float> m_RewardsDebug;


        VPVehicleController m_Car;
        public Rigidbody m_rb;
        VPStandardInput m_Input;
        public Spline m_CenterlinePath;
        bool m_Acceleration;
        bool m_Brake;
        float m_Steering;
        int m_CheckpointIndex;

        bool m_EndEpisode;
        float m_LastAccumulatedReward;

        float episodeTime;

        string hit = null;
        string lastHit = null;
        float lastHitTime = 0;

        void Awake()
        {
            m_Car = GetComponent<VPVehicleController>();
            m_rb = GetComponent<Rigidbody>();
            Colliders = GameObject.FindGameObjectsWithTag("Checkpoint").OrderBy((gameObject)=> gameObject.name).Select((gameObject) => gameObject.GetComponent<Collider>()).ToArray();
            m_Input = GetComponent<VPStandardInput>();
        }

        void Start()
        {
            m_RewardsDebug = new()
            {
                { "hitPenalty", 0f },
                { "passCheckpoint", 0f },
                { "towardsCheckpoint", 0f },
                { "speed", 0f },
                { "acceleration", 0f },
            };
            // If the agent is training, then at the start of the simulation, pick a random checkpoint to train the agent.
            OnEpisodeBegin();

            if (Mode == AgentMode.Inferencing) m_CheckpointIndex = InitCheckpointIndex;

            TrackCheckpoints.OnPlayerCorrectCheckpoint += TrackCheckpoints_OnPlayerCorrectCheckpoint;
        }

        void Update()
        {
            if (m_EndEpisode && (Time.time - episodeTime) <= 0.4)
            {
                m_EndEpisode = false;
            }

            if (m_EndEpisode && (Time.time - episodeTime) > 0.4)
            {
                m_EndEpisode = false;
                Debug.Log("End episode!");
                AddReward(m_LastAccumulatedReward);
                lastHit = hit;
                lastHitTime = Time.time;

                Debug.Log("Episode time: " + (Time.time - episodeTime));
                m_rb.Sleep();
                m_Car.gameObject.SetActive(false);
                //EndEpisode();
                OnEpisodeBegin();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            var maskedValue = 1 << other.gameObject.layer;
            var triggered = maskedValue & CheckpointMask;

            FindCheckpointIndex(other, out var index);

            // Ensure that the agent touched the checkpoint and the new index is greater than the m_CheckpointIndex.
            if (triggered > 0 && index > m_CheckpointIndex || index == 0 && m_CheckpointIndex == Colliders.Length - 1)
            {
                AddReward(PassCheckpointReward);                                       
                m_CheckpointIndex = index;
            }
        }

        void FindCheckpointIndex(Collider checkPoint, out int index)
        {
            for (int i = 0; i < Colliders.Length; i++)
            {
                if (Colliders[i].GetInstanceID() == checkPoint.GetInstanceID())
                {
                    index = i;
                    return;
                }
            }
            index = -1;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(m_rb.position); // Car position

            int selectedGear = m_Car.data.Get(Channel.Vehicle, VehicleData.GearboxGear);
            sensor.AddObservation((selectedGear == -1) ?-1f:1f * m_rb.velocity.magnitude); // Car velocity

            bool isLimiterEngaged = false;
            if (m_Car.speedControl.speedLimiter)
            {
                if (m_rb.velocity.magnitude >= m_Car.speedControl.speedLimit* 0.95f)
                {
                    isLimiterEngaged = true;
                }
            }
            sensor.AddObservation(isLimiterEngaged); // Speed limiter engaged

            m_LastAccumulatedReward = 0.0f;
            m_EndEpisode = false;

            if (hit != null)
            {
                Debug.Log("HIT -> End episode");
                m_LastAccumulatedReward += HitPenalty;
                m_EndEpisode = true;
            }

            sensor.AddObservation(hit!=null);

            // Position from centerline
            float splinePos = m_CenterlinePath.Project(m_rb.position);
            Vector3 projectedPoint = m_CenterlinePath.GetPosition(splinePos);
            sensor.AddObservation((m_rb.position - projectedPoint).magnitude); // Position from centerline

            // Rotation from centerline
            Vector3 tangent = m_CenterlinePath.GetTangent(m_CenterlinePath.Project(m_rb.position));
            sensor.AddObservation(Vector3.Angle(m_rb.transform.forward, tangent)); // Rotation from centerline
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            //Debug.Log(actions.DiscreteActions[0]);
            //Debug.Log(actions.DiscreteActions[1]);
            //Debug.Log("");
            base.OnActionReceived(actions);
            InterpretDiscreteActions(actions);

            m_Input.externalThrottle = m_Acceleration ? 1f : 0f;
            m_Input.externalBrake = m_Brake ? 1f : 0f;
            m_Input.externalSteer = m_Steering;

            // Find the next checkpoint when registering the current checkpoint that the agent has passed.
            var next = (m_CheckpointIndex + 1) % Colliders.Length;
            var nextCollider = Colliders[next];
            var direction = (nextCollider.transform.position - m_Car.transform.position).normalized;
            var reward = Vector3.Dot(m_rb.velocity.normalized, direction);

            // Add rewards if the agent is heading in the right direction
            AddReward(reward * TowardsCheckpointReward);
            m_RewardsDebug["towardsCheckpoint"] += reward * TowardsCheckpointReward;
            AddReward((m_Acceleration && !m_Brake ? 1.0f : 0.0f) * AccelerationReward);
            m_RewardsDebug["acceleration"] += (m_Acceleration && !m_Brake ? 1.0f : 0.0f) * AccelerationReward;

            int selectedGear = m_Car.data.Get(Channel.Vehicle, VehicleData.GearboxGear);
            if (selectedGear > 0)
            {
                AddReward(m_rb.velocity.magnitude * SpeedReward);
                m_RewardsDebug["speed"] += m_rb.velocity.magnitude * SpeedReward;
            } else if (selectedGear == -1)
            {
                AddReward(-m_rb.velocity.magnitude * SpeedReward);
                m_RewardsDebug["speed"] -= m_rb.velocity.magnitude * SpeedReward;
            }
        }

        private void TrackCheckpoints_OnPlayerCorrectCheckpoint(object sender, System.EventArgs e)
        {
            AddReward(PassCheckpointReward);
            m_RewardsDebug["passCheckpoint"] += PassCheckpointReward;
        }

        public override void OnEpisodeBegin()
        {
            Debug.Log("OnEpisodeBegin");
            switch (Mode)
            {
                case AgentMode.Training:
                    episodeTime = Time.time;
                    m_CheckpointIndex = Random.Range(0, Colliders.Length - 1);
                    var collider = Colliders[m_CheckpointIndex];

                    collider.GetComponent<CheckpointSingle>().trackCheckpoints.nextCheckpointSingleIndexList[0] = m_CheckpointIndex; // Set the checkpoint beginning

                    //transform.localRotation = collider.transform.rotation;
                    //transform.position = collider.transform.position;

                    // Teleport
                    //m_rb.Sleep();
                    //m_Car.gameObject.SetActive(false);
                    m_rb.isKinematic = true;
                    m_Car.Reposition(collider.transform.position - new Vector3(0, 1, 0), collider.transform.rotation);
                    //m_Car.HardReposition(collider.transform.position - new Vector3(0,1,0), collider.transform.rotation, true);
                    //m_rb.transform.position = collider.transform.position - new Vector3(0, 1, 0);
                    //m_rb.transform.rotation = collider.transform.rotation;
                    m_rb.isKinematic = false;
                    m_Car.gameObject.SetActive(true);
                    m_rb.WakeUp();
                    //m_Car.Reposition(collider.transform.position - new Vector3(0, 1, 0), collider.transform.rotation);

                    //await Task.Delay(1000);
                    //m_Car.paused = false;
                    //m_rb.isKinematic = false;
                    //m_Car.SingleStep();

                    //m_rb.velocity = default;
                    m_Acceleration = false;
                    m_Brake = false;
                    hit = null;
                    m_Steering = 0f;
                    //await Task.Delay(1000);

                    Debug.Log("Position:");
                    Debug.Log(collider.transform.position);
                    Debug.Log(m_Car.cachedTransform.position);
                    Debug.Log(m_Car.transform.position);
                    Debug.Log(m_rb.transform.position);

                    // Reset rewards
                    m_RewardsDebug = new()
                    {
                        { "hitPenalty", 0f },
                        { "passCheckpoint", 0f },
                        { "towardsCheckpoint", 0f },
                        { "speed", 0f },
                        { "acceleration", 0f },
                    };

                    //RequestDecision();
                    break;
                default:
                    break;
            }
        }

        void InterpretDiscreteActions(ActionBuffers actions)
        {
            //Debug.Log(actions.DiscreteActions.Length);
            //Debug.Log(actions.DiscreteActions[0]);
            //Debug.Log(actions.DiscreteActions[1]);
            m_Steering = actions.DiscreteActions[0] - 1f;
            m_Acceleration = actions.DiscreteActions[1] > 1.0f;
            m_Brake = actions.DiscreteActions[1] < 1.0f;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = 1;
            discreteActionsOut[1] = 1;
            if (Input.GetKey(KeyCode.A))
            {
                discreteActionsOut[0] = 0;
            }
            if (Input.GetKey(KeyCode.D))
            {
                discreteActionsOut[0] = 2;
            }
            if (Input.GetKey(KeyCode.W))
            {
                discreteActionsOut[1] = 2;
            }
            if (Input.GetKey(KeyCode.S))
            {
                discreteActionsOut[1] = 0;
            }
            //Debug.Log(continuousActionsOut[0]);
            //Debug.Log(continuousActionsOut[1]);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Track") && collision.gameObject.name != lastHit && (Time.time - lastHitTime) > 0.4)
            {
                hit = collision.gameObject.name;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("frictionMultiplier: " + m_Car.tireFriction.frictionMultiplier.ToString());
            GUILayout.Label("Reward: " + GetCumulativeReward().ToString());
            GUILayout.Label("CompletedEpisodes: " + CompletedEpisodes.ToString());
            GUILayout.Label("Episode time: " + (Time.time - episodeTime));
            GUILayout.Label("Position: " + m_Car.transform.position.ToString()); 
            GUILayout.Label("fixedDeltaTime: " + Time.fixedDeltaTime.ToString());

            GameObject.Find("/GameHUD/HUD/Rewards").GetComponent<TextMeshProUGUI>().text = 
$@"<b>Rewards:</b>
passCheckpoint: {m_RewardsDebug["passCheckpoint"].ToString("F2")}
towardsCheckpoint: {m_RewardsDebug["towardsCheckpoint"].ToString("F2")}
acceleration: {m_RewardsDebug["acceleration"].ToString("F2")}
speed: {m_RewardsDebug["speed"].ToString("F2")}";
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float splinePos = m_CenterlinePath.Project(m_rb.position);
            Vector3 projectedPoint = m_CenterlinePath.GetPosition(splinePos);
            Vector3 tangent = m_CenterlinePath.GetTangent(m_CenterlinePath.Project(m_rb.position));

            Gizmos.DrawLine(projectedPoint, projectedPoint + tangent.normalized * 2);
            Gizmos.DrawSphere(projectedPoint, 0.2f);
            Gizmos.DrawLine(m_rb.position + new Vector3(0, 1.5f, 0), m_rb.position + m_rb.transform.forward*3 + new Vector3(0, 1.5f, 0));
        }
    }
}
