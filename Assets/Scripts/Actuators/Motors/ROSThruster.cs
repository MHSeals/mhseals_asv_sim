using UnityEngine;
using Sim.Utils.ROS;
using RosMessageTypes.Std;

namespace Sim.Actuators.Motors {
    public class ROSThruster : Thruster {
        [SerializeField] private string topicName;
        
        private ROSSubscriber<Float32Msg> ros;

        protected override void Awake() {
            base.Awake();
            ros.Initialize(topicName, CommandCallback);
        }
        
        private void CommandCallback(Float32Msg msg) {
            base.SetCommand(msg.data);
        } 
    }
}