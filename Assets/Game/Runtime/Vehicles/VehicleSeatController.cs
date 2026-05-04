using MafiaTopDown.Gameplay.Domain.Vehicles;
using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.Data;
using System;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Vehicles
{
    [DisallowMultipleComponent]
    public sealed class VehicleSeatController : MonoBehaviour
    {
        [SerializeField] private Transform? driverAnchor;
        [SerializeField] private Transform? exitAnchor;
        [SerializeField] private CampaignProgressionController? campaignProgressionController;
        [SerializeField] private MissionDefinitionAsset? missionAsset;
        [SerializeField] private string missionStageIdOnEnter = string.Empty;
        [SerializeField] private float exitProbeStep = 0.85f;

        private readonly VehicleSeatStateMachine _stateMachine = new();
        private TopDownPlayerController? _occupant;
        private Renderer[] _occupantRenderers = Array.Empty<Renderer>();

        public VehicleSeatState State => _stateMachine.State;

        public bool IsOccupied => _occupant != null;

        public bool TryEnter(TopDownPlayerController player)
        {
            if (IsOccupied || _stateMachine.State != VehicleSeatState.OnFoot)
            {
                return false;
            }

            _stateMachine.BeginEnter();
            _occupant = player;
            _occupant.SetControlsLocked(true);
            _occupantRenderers = _occupant.GetComponentsInChildren<Renderer>(true);
            SetOccupantRenderersVisible(false);

            if (driverAnchor != null)
            {
                _occupant.transform.SetPositionAndRotation(driverAnchor.position, driverAnchor.rotation);
                _occupant.transform.SetParent(driverAnchor, true);
            }

            _stateMachine.ConfirmEntered();
            if (campaignProgressionController != null && missionAsset != null)
            {
                campaignProgressionController.CompleteMissionStage(missionAsset, missionStageIdOnEnter);
            }

            return true;
        }

        public bool TryExit()
        {
            if (_occupant == null || _stateMachine.State != VehicleSeatState.Driving)
            {
                return false;
            }

            _stateMachine.BeginExit();

            var occupant = _occupant!;
            _occupant = null;
            var characterController = occupant.GetComponent<CharacterController>();
            var wasEnabled = characterController != null && characterController.enabled;
            if (wasEnabled && characterController != null)
            {
                characterController.enabled = false;
            }

            occupant.transform.SetParent(null, true);

            if (exitAnchor != null)
            {
                var exitPosition = ResolveClearExitPosition(exitAnchor.position, characterController);
                occupant.transform.SetPositionAndRotation(exitPosition, exitAnchor.rotation);
            }

            if (wasEnabled && characterController != null)
            {
                characterController.enabled = true;
            }

            occupant.SetControlsLocked(false);
            SetOccupantRenderersVisible(true);
            _occupantRenderers = Array.Empty<Renderer>();
            _stateMachine.ConfirmExited();
            return true;
        }

        private Vector3 ResolveClearExitPosition(Vector3 requestedPosition, CharacterController? characterController)
        {
            if (IsExitClear(requestedPosition, characterController))
            {
                return requestedPosition;
            }

            var directions = new[]
            {
                transform.right,
                -transform.right,
                transform.forward,
                -transform.forward,
                (transform.right + transform.forward).normalized,
                (-transform.right + transform.forward).normalized,
                (transform.right - transform.forward).normalized,
                (-transform.right - transform.forward).normalized
            };

            for (var ring = 1; ring <= 3; ring += 1)
            {
                var distance = exitProbeStep * ring;
                foreach (var direction in directions)
                {
                    var candidate = requestedPosition + (direction * distance);
                    if (IsExitClear(candidate, characterController))
                    {
                        return candidate;
                    }
                }
            }

            return requestedPosition;
        }

        private static bool IsExitClear(Vector3 position, CharacterController? characterController)
        {
            var radius = characterController != null ? Mathf.Max(0.2f, characterController.radius) : 0.35f;
            var height = characterController != null ? Mathf.Max(1f, characterController.height) : 1.8f;
            var bottom = position + Vector3.up * (radius + 0.08f);
            var top = position + Vector3.up * Mathf.Max(radius + 0.12f, height - radius);
            return !Physics.CheckCapsule(bottom, top, radius, ~0, QueryTriggerInteraction.Ignore);
        }

        private void SetOccupantRenderersVisible(bool isVisible)
        {
            foreach (var renderer in _occupantRenderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = isVisible;
                }
            }
        }
    }
}
