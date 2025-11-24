
using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Sim.Utils;
using Sim.Utils.ROS;

namespace Sim.Sensors.Nav {
    public class GPS : MonoBehaviour, IROSSensor<NavSatFixMsg> {
        [field: SerializeField] public string topicName { get; set; } = "gps/raw";
        [field: SerializeField] public string frameId { get; set; } = "gps_link";
        [field: SerializeField] public float Hz { get; set; } = 20.0f;
        public ROSPublisher<NavSatFixMsg> publisher { get; set; }

        public NavSatFixMsg CreateMessage() {
            return new NavSatFixMsg {
                header = ROSPublisher<ImuMsg>.CreateHeader(frameId)
            };
        }

        void Start() {
            if (publisher == null) publisher = gameObject.AddComponent<ROSPublisher<NavSatFixMsg>>();
            publisher.Initialize(topicName, frameId, CreateMessage, Hz);
        }
    }
}
