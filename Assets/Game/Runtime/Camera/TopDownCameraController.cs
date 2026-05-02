using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Camera
{
    public sealed class TopDownCameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Vector3 offset = new Vector3(0f, 18f, -10f);
        [SerializeField] private float followSmoothness = 7f;
        [SerializeField] private float zoomSpeed = 8f;
        [SerializeField] private float minimumHeight = 12f;
        [SerializeField] private float maximumHeight = 24f;
        [SerializeField] private float pitch = 55f;
        [SerializeField] private float yaw = -3f;

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
            }

            var targetPosition = followTarget.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSmoothness);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
