using MafiaTopDown.Gameplay.Domain.Vehicles;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Vehicles
{
    [DisallowMultipleComponent]
    public sealed class SimpleVehicleDriver : MonoBehaviour
    {
        [SerializeField] private VehicleSeatController seatController = null!;
        [SerializeField] private float maxForwardSpeed = 8.5f;
        [SerializeField] private float reverseSpeed = 3.75f;
        [SerializeField] private float acceleration = 16f;
        [SerializeField] private float braking = 22f;
        [SerializeField] private float steeringSpeed = 112f;
        [SerializeField] private float coastDrag = 5.5f;
        [SerializeField] private Rigidbody vehicleBody = null!;

        private float _currentSpeed;

        private void Reset()
        {
            seatController = GetComponent<VehicleSeatController>();
            vehicleBody = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            seatController ??= GetComponent<VehicleSeatController>();
            vehicleBody ??= GetComponent<Rigidbody>();
            if (vehicleBody == null)
            {
                vehicleBody = gameObject.AddComponent<Rigidbody>();
            }

            ConfigureVehicleBody(vehicleBody);
        }

        private void FixedUpdate()
        {
            if (seatController == null || seatController.State != VehicleSeatState.Driving)
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, coastDrag * Time.fixedDeltaTime);
                MoveVehicle(0f, Time.fixedDeltaTime);
                return;
            }

            var throttle = Input.GetAxisRaw("Vertical");
            var steer = Input.GetAxisRaw("Horizontal");
            var deltaTime = Time.fixedDeltaTime;
            var targetSpeed = throttle >= 0f
                ? throttle * maxForwardSpeed
                : throttle * reverseSpeed;
            var speedChange = Mathf.Abs(targetSpeed) < Mathf.Abs(_currentSpeed) ? braking : acceleration;

            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, speedChange * deltaTime);
            MoveVehicle(steer, deltaTime);
        }

        private void ConfigureVehicleBody(Rigidbody body)
        {
            body.useGravity = false;
            body.mass = 1200f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
        }

        private void MoveVehicle(float steer, float deltaTime)
        {
            if (vehicleBody == null)
            {
                transform.position += transform.forward * (_currentSpeed * deltaTime);
                return;
            }

            var speedRatio = Mathf.InverseLerp(0.15f, maxForwardSpeed, Mathf.Abs(_currentSpeed));
            var steeringDirection = Mathf.Approximately(_currentSpeed, 0f) ? 1f : Mathf.Sign(_currentSpeed);
            var turnDegrees = steer * steeringSpeed * speedRatio * steeringDirection * deltaTime;
            var nextRotation = vehicleBody.rotation * Quaternion.Euler(0f, turnDegrees, 0f);
            var nextPosition = vehicleBody.position + (nextRotation * Vector3.forward * (_currentSpeed * deltaTime));

            vehicleBody.MoveRotation(nextRotation);
            vehicleBody.MovePosition(nextPosition);
        }
    }
}
