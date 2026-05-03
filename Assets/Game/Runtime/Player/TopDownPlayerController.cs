using UnityEngine;
using MafiaTopDown.Gameplay.Runtime.UI;

namespace MafiaTopDown.Gameplay.Runtime.Player
{
    [DisallowMultipleComponent]
    public sealed class TopDownPlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float sprintMultiplier = 1.65f;
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private CharacterController? characterController;

        private bool _controlsLocked;
        private GameSettingsData? _settings;

        public bool ControlsLocked => _controlsLocked;

        public void SetControlsLocked(bool isLocked)
        {
            _controlsLocked = isLocked;
        }

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _settings = GameSettingsFileService.LoadOrCreate();
        }

        private void Update()
        {
            if (_controlsLocked)
            {
                return;
            }

            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            var isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var sensitivity = _settings == null ? 1f : Mathf.Clamp(_settings.InputSensitivity, 0.75f, 1.35f);
            var speed = walkSpeed * sensitivity * (isSprinting ? sprintMultiplier : 1f);
            var desiredVelocity = input.normalized * speed;

            if (characterController != null)
            {
                characterController.SimpleMove(desiredVelocity);
            }
            else
            {
                transform.position += desiredVelocity * Time.deltaTime;
            }

            if (input.sqrMagnitude > 0.001f)
            {
                var targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
    }
}
