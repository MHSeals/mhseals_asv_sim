// using System;
// using UnityEngine;
// using Unity.Robotics.ROSTCPConnector;
// using RosMessageTypes.Std;
//
// namespace Sim.Actuators
// {
//     public class Thruster : MonoBehaviour
//     {
//         [SerializeField, Tooltip("The location of the thrust applied")] private GameObject thrusterJoint;
//         [SerializeField] private string topicName;
//
//         [SerializeField] private float maxThrust = 250.0f;
//         [SerializeField, Range(0.0f, 3000.0f)] private float maxRpmVisual = 500.0f;
//         [SerializeField, Tooltip("The rigidbody to apply the forces to")] private Rigidbody rigidbody;
//         [SerializeField] private bool animateRotation = true;
//         [SerializeField] private bool debug = false;
//         private float velocityCommand = 0.0f;
//         private float velocityCurr = 0.0f;
//         private float thrustAngle;
//
//         private void Start()
//         {
//             thrustAngle = thrusterJoint.transform.localEulerAngles.y;
//             ROSConnection.GetOrCreateInstance().Subscribe<Float32Msg>(topicName + "/thrust", ThrustCallback);
//         }
//
//         private void FixedUpdate()
//         {
//             Vector3 thrustDirLocal = new Vector3(Mathf.Sin(Mathf.Deg2Rad * thrustAngle), 0f, Mathf.Cos(Mathf.Deg2Rad * thrustAngle));
//             Vector3 thrustDir = transform.TransformDirection(thrustDirLocal);
//
//             // measure velocity along the thrust axis
//
//             if (debug) Debug.DrawRay(thrusterJoint.transform.position, thrustForce / maxThrust);
//
//             // thruster visual control
//             if (animateRotation)
//             {
//                 float thrusterAngleTurnRate = (thrustCurr / maxThrust) * (maxRpmVisual / 60);
//                 Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -thrusterAngleTurnRate * 360.0f * Time.deltaTime);
//                 thrusterJoint.transform.localRotation *= rotation;
//             }
//         }
//
//         private void ThrustCallback(Float32Msg msg)
//         {
//             thrustCommand = Math.Clamp(msg.data, -1.0f, 1.0f);
//         }
//     }
// }
//
