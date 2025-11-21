using System;
using Sim.Utils.ROS;

namespace Sim.Sensors {
    public interface IROSSensor<T> where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message {
        string topicName { get; set; }
        string frameId { get; set; }
        ROSPublisher<T> publisher { get; set; }
        T CreateMessage();
    }
}
