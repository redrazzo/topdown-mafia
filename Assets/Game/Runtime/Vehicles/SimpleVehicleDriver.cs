using MafiaTopDown.Gameplay.Domain.Vehicles;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Vehicles
{
    [DisallowMultipleComponent]
    public sealed class SimpleVehicleDriver : MonoBehaviour
    {
        [SerializeField] private VehicleSeatController seatController;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float reverseSpeed = 4f;
        [SerializeField] private float steeringSpeed = 90f;
        [SerializeField] private float drag = 4f;

        private float _currentSpeed;

        private void Reset()
        {
            seatController = GetComponent<VehicleSeatController>();
        }

        private void Update()
        {
            if (seatController == null || seatController.State != VehicleSeatState.Driving)
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, drag * Time.deltaTime);
                transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
                return;
            }

            var throttle = Input.GetAxisRaw("Vertical");
            var steer = Input.GetAxisRaw("Horizontal");

            var targetSpeed = throttle >= 0f ? throttle * acceleration : throttle * reverseSpeed;
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, acceleration * Time.deltaTime);

            transform.Rotate(Vector3.up, steer * steeringSpeed * Time.deltaTime);
            transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
        }
    }
}
