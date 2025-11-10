using System;
using UnityEngine;
using Sim.Utils;

namespace Sim.Actuators
{
    public enum MotorControlMode
    {
        Torque,
        Velocity,
        Position
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public abstract class MotorBase : MonoBehaviour
    {
        [SerializeField, Tooltip("The GameObject representing the motor joint")] protected GameObject motorJoint;
        [SerializeField] protected MotorControlMode controlMode = MotorControlMode.Torque;
        [SerializeField, Tooltip("Nm")] protected float maxTorque = 1.3f;
        [SerializeField, Tooltip("rad/s")] protected float angularVelocityLimit = 300.0f;
        [SerializeField, Tooltip("Torque Responsiveness"), Range(0f, 1f)] protected float torqueK = 1f;
        [SerializeField, Tooltip("Position Control Responsiveness"), Range(0f, 1f)] protected float positionK = 1f;
        [SerializeField] protected Axis rotationAxis = Axis.Y;
        [SerializeField] protected float minAngle = -Mathf.Infinity;
        [SerializeField] protected float maxAngle = Mathf.Infinity;
        protected Vector3 worldAxis;
        protected float maxAngularAcceleration;
        protected IPhysicsBody body;
        [SerializeField] protected float command; // Meaning depends on controlMode
        protected float motionVelocity; // Tracks velocity for trapezoidal profile

        protected virtual void OnValidate()
        {
            if (GetComponent<Rigidbody>() == null && GetComponent<ArticulationBody>() == null)
                Debug.LogWarning($"{name} should have either a Rigidbody or an ArticulationBody attached.");

            if (minAngle > maxAngle)
            {
                (maxAngle, minAngle) = (minAngle, maxAngle);
                Debug.LogWarning($"{name}: Swapped min/max angles because min > max");
            }
        }

        protected virtual void Awake()
        {
            var rb = GetComponent<Rigidbody>();
            var ab = GetComponent<ArticulationBody>();

            if (rb != null) body = new RigidbodyAdapter(rb);
            else if (ab != null) body = new ArticulationBodyAdapter(ab);
            else Debug.LogError("No Rigidbody or ArticulationBody found!");
        }

        protected virtual void Start()
        {
            Vector3 axisVector = rotationAxis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.up
            };
            float I = Vector3.Dot(axisVector, body.inertiaTensorRotation * Vector3.Scale(body.inertiaTensor, axisVector));
            maxAngularAcceleration = maxTorque / I;
            worldAxis = motorJoint.transform.TransformDirection(axisVector);
        }

        protected virtual void FixedUpdate()
        {
            switch (controlMode)
            {
                case MotorControlMode.Torque:
                    ApplyTorque(command);
                    break;

                case MotorControlMode.Velocity:
                    ApplyVelocityControl(command);
                    break;

                case MotorControlMode.Position:
                    ApplyPositionControl(command);
                    break;
            }
        }

        protected virtual void ApplyTorque(float torque)
        {
            // Smooth torque application using torqueK to simplify external factors slowing torque (near instant)
            float torqueOutput = Mathf.Lerp(GetTorque(), torque, torqueK);
            torqueOutput = Mathf.Clamp(torqueOutput, -maxTorque, maxTorque);

            if (controlMode != MotorControlMode.Position)
                IsAtLimit(ref torque);

            body.AddTorque(worldAxis * torqueOutput);
        }

        protected virtual void ApplyVelocityControl(float targetVelocity)
        {
            float currentVelocity = GetVelocity();
            motionVelocity = Mathf.MoveTowards(
                motionVelocity,
                Mathf.Clamp(targetVelocity, -angularVelocityLimit, angularVelocityLimit),
                maxAngularAcceleration * Time.fixedDeltaTime
            );

            float torque = (motionVelocity - currentVelocity) / Time.fixedDeltaTime;
            torque = Mathf.Clamp(torque, -maxTorque, maxTorque);

            if (controlMode != MotorControlMode.Position)
                IsAtLimit(ref targetVelocity);

            ApplyTorque(torque);
        }

        protected virtual void ApplyPositionControl(float targetAngle)
        {
            targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);
            float currentAngle = GetAngle();
            float angleError = Mathf.DeltaAngle(currentAngle, targetAngle);
            float currentVelocity = GetVelocity();
            float stoppingDistance = (currentVelocity * currentVelocity) / (2f * maxAngularAcceleration);
            float desiredVelocity;

            if (Mathf.Abs(angleError) <= stoppingDistance)
            {
                // Decelerate smoothly to stop at target
                desiredVelocity = Mathf.Sign(angleError) * Mathf.Sqrt(2f * maxAngularAcceleration * Mathf.Abs(angleError));
            }
            else
            {
                // Accelerate toward target at max velocity
                desiredVelocity = Mathf.Sign(angleError) * angularVelocityLimit;
            }

            // Apply position responsiveness gain
            desiredVelocity *= positionK;
            desiredVelocity = Mathf.Clamp(desiredVelocity, -angularVelocityLimit, angularVelocityLimit);

            ApplyVelocityControl(desiredVelocity);
        }

        protected virtual float GetAlongAxis(Vector3 v, Axis axis) =>
            axis switch
            {
                Axis.X => v.x,
                Axis.Y => v.y,
                Axis.Z => v.z,
                _ => v.y
            };

        protected virtual float GetVelocity() => GetAlongAxis(body.angularVelocity, rotationAxis);
        protected virtual float GetAngle() => GetAlongAxis(motorJoint.transform.localEulerAngles, rotationAxis);
        protected virtual float GetTorque() => GetAlongAxis(body.GetAccumulatedTorque(), rotationAxis);

        public void SetCommand(float value)
        {
            command = value;

            // Reset motion velocity to current velocity for trapezoidal profile
            motionVelocity = GetVelocity();
        }

        protected virtual bool IsAtLimit(ref float command)
        {
            float currentAngle = GetAngle();
            if ((currentAngle <= minAngle && command < 0f) ||
                (currentAngle >= maxAngle && command > 0f))
            {
                command = 0f;
                motionVelocity = 0f;
                return true;
            }
            return false;
        }
    }
}
