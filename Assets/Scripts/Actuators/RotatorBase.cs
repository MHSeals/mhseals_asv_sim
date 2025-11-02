// using System;
// using UnityEngine;
// using Unity.Robotics.ROSTCPConnector;
// using RosMessageTypes.Std;
// using System.Numerics;
//
// namespace Sim.Actuators
// {
//     public abstract class RotatorBase : MonoBehaviour
//     {
//         [SerializeField, Tooltip("The object to rotate")] private GameObject rotatorJoint;
//         [SerializeField] private string topicName;
//
//         [SerializeField, Range(0.001f, 1.0f)]
//         [Tooltip("Controls the responsiveness of rotation")]
//         private float rotationK = 1.0f;
//
//         [SerializeField, Tooltip("rad/s")] private float maxAngularVelocity = 5.0f;
//         [SerializeField] private float minRotation = Mathf.NegativeInfinity;
//         [SerializeField] private float maxRotation = Mathf.PositiveInfinity;
//         [SerializeField, Tooltip("The rigid body to apply torque to")] private Rigidbody rigidBody;
//         [SerializeField] private bool useAngleControl = true;
//         [SerializeField] private bool debug = false;
//         private float rotationCommand = 0.0f;
//         private float rotationCurr;
//
//         void OnValidate()
//         {
//             if (minRotation > maxRotation)
//             {
//                 float temp = minRotation;
//                 minRotation = maxRotation;
//                 maxRotation = temp;
//                 Debug.LogWarning($"{name}: Swapped min/max rotation because min > max");
//             }
//         }
//
//         private void Start()
//         {
//             rotationCurr = rotatorJoint.transform.localEulerAngles.y;
//             ROSConnection.GetOrCreateInstance().Subscribe<Float32Msg>(topicName + "/rotation", ThrustCallback);
//         }
//
//         private void FixedUpdate()
//         {
//             float alpha = 1f - Mathf.Exp(-torqueK * Time.fixedDeltaTime);
//             currentTorque = Mathf.Lerp(currentTorque, desiredTorque, alpha);
//             GetComponent<Rigidbody>().AddTorque(currentTorque, ForceMode.Force);
//
//             float rotationSetpoint = rotationCommand * maxThrust;
//             float thrustError = thrustSetpoint - thrustCurr;
//             thrustCurr += Mathf.Clamp(thrustError * thrustK, -maxThrust, maxThrust);
//             Vector3 thrustDirLocal = new(Mathf.Sin(Mathf.Deg2Rad * thrustAngle), 0.0f, Mathf.Cos(Mathf.Deg2Rad * thrustAngle));
//             Vector3 thrustDir = transform.TransformDirection(thrustDirLocal);
//             Vector3 rotationForce = thrustCurr * thrustDir;
//             rigidBody.AddForceAtPosition(thrustForce, thrusterJoint.transform.position);
//
//             if (debug) Debug.DrawRay(rotatorJoint.transform.position + rotatorJoint.transform.rotation,
//                                      rotationForce);
//         }
//
//         private void ThrustCallback(Float32Msg msg)
//         {
//             rotationCommand = Math.Clamp(msg.data, minRotation, maxRotation);
//         }
//     }
// }
//
