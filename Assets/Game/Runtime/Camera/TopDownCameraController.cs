using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Camera
{
    public sealed class TopDownCameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Vector3 offset = new Vector3(0f, 10.8f, -6.6f);
        [SerializeField] private float followSmoothness = 7f;
        [SerializeField] private float zoomSpeed = 8f;
        [SerializeField] private float minimumHeight = 8.6f;
        [SerializeField] private float maximumHeight = 14.2f;
        [SerializeField] private float pitch = 52f;
        [SerializeField] private float yaw = -2f;

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            var scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollDelta) > 0.001f)
            {
                offset.y = Mathf.Clamp(offset.y - scrollDelta * zoomSpeed, minimumHeight, maximumHeight);
                offset.z = Mathf.Lerp(-5.8f, -7.8f, Mathf.InverseLerp(minimumHeight, maximumHeight, offset.y));
            }

            var targetPosition = followTarget.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSmoothness);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
