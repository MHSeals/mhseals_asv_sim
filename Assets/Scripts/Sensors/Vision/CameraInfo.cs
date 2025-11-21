using RosMessageTypes.Sensor;
using UnityEngine;
using Sim.Utils.ROS;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace Sim.Sensors.Vision {
    [RequireComponent(typeof(Camera))]
    public class CameraInfo : MonoBehaviour, IROSSensor<CameraInfoMsg> {
        [field: SerializeField] public string topicName { get; set; } = "camera/camera_info";
        [field: SerializeField] public string frameId { get; set; } = "camera_link_optical_frame";
        [field: SerializeField] public float Hz { get; set; } = 5.0f;
        public ROSPublisher<CameraInfoMsg> publisher { get; set; }

        private Camera sensorCamera;

        private void Start() {
            sensorCamera = GetComponent<Camera>();
            if (publisher == null)
                publisher = gameObject.AddComponent<ROSPublisher<CameraInfoMsg>>();

            publisher.Initialize(topicName, frameId, CreateMessage, Hz);
        }

        public CameraInfoMsg CreateMessage() {
            return CameraInfoGenerator.ConstructCameraInfoMessage(
                sensorCamera, 
                ROSPublisher<CameraInfoMsg>.CreateHeader(frameId), 
                0f, 
                1.0f
            );
        }
    }
}