using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

namespace Sim.Utils.ROS {
    [Serializable]
    public class ROSSubscriber<T> : MonoBehaviour where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message {
        public string topicName;

        public void Initialize(string topicName, Action<T> callback) {
            this.topicName = topicName;
            ROSConnection.GetOrCreateInstance().Subscribe(topicName, callback);
        }

        public void Initialize(Action<T> callback) {
            if (topicName == null) { Debug.LogError("No topic name set"); return; }
            ROSConnection.GetOrCreateInstance().Subscribe(topicName, callback);
        }
    }
}
