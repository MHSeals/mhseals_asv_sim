using UnityEngine;
using Sim.Actuators.Motors;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

namespace Sim.Controllers {
    public class OmniXController : MonoBehaviour, IControllerBase {
        [SerializeField] private InputActionReference linearAction, angularAction;
        [SerializeField, Tooltip("Length (front to rear) and width (left to right) between thrusters")] private float length, width;
        [SerializeField] private Thruster frontLeft, frontRight, rearLeft, rearRight;
        [SerializeField] private ThrusterConfig config; // Assumes all thrusters have this configuration
        private Vector2 linearInput;
        private float angularInput;

        private void OnEnable() {
            linearAction.action.performed += OnLinearPerformed;
            linearAction.action.canceled += OnLinearCanceled;

            angularAction.action.performed += OnAngularPerformed;
            angularAction.action.canceled += OnAngularCanceled;

            linearAction.action.Enable();
            angularAction.action.Enable();
        }

        private void OnDisable() {
            linearAction.action.performed -= OnLinearPerformed;
            linearAction.action.canceled -= OnLinearCanceled;

            angularAction.action.performed -= OnAngularPerformed;
            angularAction.action.canceled -= OnAngularCanceled;

            linearAction.action.Disable();
            angularAction.action.Disable();
        }

        private void OnLinearPerformed(InputAction.CallbackContext ctx) {
            linearInput = ctx.ReadValue<Vector2>();
            Move();
        }

        private void OnLinearCanceled(InputAction.CallbackContext ctx) {
            linearInput = Vector2.zero;
            Move();
        }

        private void OnAngularPerformed(InputAction.CallbackContext ctx) {
            angularInput = ctx.ReadValue<float>();
            Move();
        }

        private void OnAngularCanceled(InputAction.CallbackContext ctx) {
            angularInput = 0;
            Move();
        }
        
        private void Move() {
            SetMotion(new Vector3(linearInput.x, 0, linearInput.y), new Vector3(0, angularInput, 0));
        }

        // TODO: More accurately model desired linear and angular velocity (not just full forward throttle/backward/angular)
        public void SetMotion(Vector3 linear, Vector3 angular) {
            if (linear.y == 1) {
                frontLeft.SetCommand(config.GetMinCommand());
                frontRight.SetCommand(config.GetMinCommand());
                rearLeft.SetCommand(config.GetMaxCommand());
                rearRight.SetCommand(config.GetMaxCommand());
            }
        }
    }
}