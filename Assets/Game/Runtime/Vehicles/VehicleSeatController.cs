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

            var occupant = _occupant;
            _occupant = null;

            occupant.transform.SetParent(null, true);

            if (exitAnchor != null)
            {
                occupant.transform.SetPositionAndRotation(exitAnchor.position, exitAnchor.rotation);
            }

            occupant.SetControlsLocked(false);
            SetOccupantRenderersVisible(true);
            _occupantRenderers = Array.Empty<Renderer>();
            _stateMachine.ConfirmExited();
            return true;
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
