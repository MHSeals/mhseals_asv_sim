using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

namespace Sim.Utils.ROS {
    [Serializable]
    public class ROSPublisher<T> : MonoBehaviour where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message {
        public string topicName;
        public string frameId;
        public float Hz, timeSincePublish;
        public ROSConnection ros;
        private Func<T> CreateMessage;

        public void Initialize(string topicName, string frameId, Func<T> CreateMessage, float Hz=10.0f) {
            this.topicName = topicName;
            this.frameId = frameId;
            this.Hz = Hz;
            this.CreateMessage = CreateMessage;
        }

        public void Initialize(Func<T> CreateMessage) {
            if (topicName == null || frameId == null) { Debug.LogError("Topic name or frame ID not set");  return; }
            if (Hz == 0.0f) Hz = 10.0f;
            this.CreateMessage = CreateMessage;
        }
        
        private void Start() {
            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<T>(topicName);
            timeSincePublish = 0.0f;
        }

        private void FixedUpdate() {
            timeSincePublish += Time.fixedDeltaTime;
            if (timeSincePublish < 1.0f / Hz) return;
            ros.Publish(topicName, CreateMessage());
            timeSincePublish = 0.0f;
        }

        public void Publish() { ros.Publish(topicName, CreateMessage()); }

        public static HeaderMsg CreateHeader(string frameId) {
            var header = new HeaderMsg { frame_id = frameId };

            var publishTime = Clock.Now;
            var sec = publishTime;
            var nanosec = (publishTime - Math.Floor(publishTime)) * Clock.k_NanoSecondsInSeconds;
            header.stamp.sec = (int)sec;
            header.stamp.nanosec = (uint)nanosec;

            return header;
        }
    }
}
