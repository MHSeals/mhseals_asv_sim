using RosMessageTypes.Sensor;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using Sim.Utils.ROS;

namespace Sim.Sensors.Vision {
    [RequireComponent(typeof(Camera))]
    public class ROSCameraAsync : MonoBehaviour, IROSSensor<ImageMsg> {
        [SerializeField] private RenderTexture rgbRenderTexture;

        [field: SerializeField] public string topicName { get; set; } = "camera/image_raw";
        [field: SerializeField] public string frameId { get; set; } = "camera_link_optical_frame";
        [field: SerializeField] public float Hz { get; set; } = 15.0f;
        public ROSPublisher<ImageMsg> publisher { get; set; }

        private Camera sensorCamera;
        private Texture2D rgbTexture2D;
        private float timeSincePublish = 0.0f;

        private void Start() {
            sensorCamera = GetComponent<Camera>();
            sensorCamera.targetTexture = rgbRenderTexture;

            if (publisher == null)
                publisher = gameObject.AddComponent<ROSPublisher<ImageMsg>>();

            publisher.Initialize(topicName, frameId, CreateMessage, Hz);
        }

        public ImageMsg CreateMessage() {
            return rgbTexture2D.ToImageMsg(ROSPublisher<ImageMsg>.CreateHeader(frameId));
        }

        private void FixedUpdate() {
            timeSincePublish += Time.fixedDeltaTime;
            if (timeSincePublish > 1.0f / Hz) {
                RequestReadback(rgbRenderTexture);
                timeSincePublish = 0.0f;
            }
        }

        private void RequestReadback(RenderTexture targetTexture) {
            AsyncGPUReadback.Request(targetTexture, 0, TextureFormat.RGB24, OnReadbackComplete);
        }

        private void OnReadbackComplete(AsyncGPUReadbackRequest request) {
            if (request.hasError) {
                Debug.LogWarning("Failed to read back texture once");
                return;
            }

            if (rgbTexture2D == null || rgbTexture2D.width != sensorCamera.targetTexture.width || rgbTexture2D.height != sensorCamera.targetTexture.height) {
                rgbTexture2D = new Texture2D(sensorCamera.targetTexture.width, sensorCamera.targetTexture.height, TextureFormat.RGB24, false);
            }

            rgbTexture2D.LoadRawTextureData(request.GetData<byte>());
            rgbTexture2D.Apply();

            if (publisher != null) publisher.Publish();
        }
    }
}