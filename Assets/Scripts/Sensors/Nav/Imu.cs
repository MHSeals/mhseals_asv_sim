using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Sim.Utils;
using Sim.Utils.ROS;

namespace Sim.Sensors.Nav {
    public class Imu : MonoBehaviour, IROSSensor<ImuMsg> {
        [field: SerializeField] public string topicName { get; set; } = "imu/raw";
        [field: SerializeField] public string frameId { get; set; } = "imu_link";
        [field: SerializeField] public float Hz { get; set; } = 50.0f;
        public ROSPublisher<ImuMsg> publisher { get; set; }

        private IPhysicsBody body;

        void OnValidate() {
            if (GetComponent<Rigidbody>() == null && GetComponent<ArticulationBody>() == null)
                Debug.LogWarning($"{name} should have either a Rigidbody or an ArticulationBody attached.");
        }

        void Awake() {
            var rb = GetComponent<Rigidbody>();
            var ab = GetComponent<ArticulationBody>();

            if (rb != null) body = new RigidbodyAdapter(rb);
            else if (ab != null) body = new ArticulationBodyAdapter(ab);
            else throw new MissingComponentException($"{name} requires a Rigidbody or ArticulationBody!");
        }

        public ImuMsg CreateMessage() {
            return new ImuMsg {
                orientation = body.transform.rotation.To<FLU>(),
                angular_velocity = body.angularVelocity.To<FLU>(),
                header = ROSPublisher<ImuMsg>.CreateHeader(frameId)
            };
        }

        void Start() {
            if (publisher == null) publisher = gameObject.AddComponent<ROSPublisher<ImuMsg>>();
            publisher.Initialize(topicName, frameId, CreateMessage, Hz);
        }
    }
}
