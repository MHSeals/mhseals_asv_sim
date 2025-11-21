using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System.Collections.Generic;
using Sim.Actuators.Motors;
using Sim.Utils.ROS;

namespace Sim.Controllers
{
    public abstract class ControllerBase : MonoBehaviour
    {
        [SerializeField] protected List<Thruster> thrusters;
    }
}

