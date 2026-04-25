using MafiaTopDown.Gameplay.Domain.Interactions;
using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Vehicles;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Interaction
{
    [DisallowMultipleComponent]
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private TopDownPlayerController playerController;
        [SerializeField] private float interactionRadius = 2.5f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private Rect promptRect = new Rect(16f, 72f, 420f, 36f);

        private VehicleSeatController _currentVehicleCandidate;
        private IInteractable _currentInteractableCandidate;
        private InteractionPromptData _currentPrompt;

        private void Reset()
        {
            playerController = GetComponent<TopDownPlayerController>();
        }

        private void Update()
        {
            RefreshCandidates();

            if (!Input.GetKeyDown(interactKey))
            {
                return;
            }

            var occupiedSeat = GetOccupiedSeatFromParents();
            if (occupiedSeat != null)
            {
                occupiedSeat.TryExit();
                return;
            }

            if (_currentVehicleCandidate != null)
            {
                _currentVehicleCandidate.TryEnter(playerController);
                return;
            }

            if (_currentInteractableCandidate != null && _currentInteractableCandidate.CanInteract)
            {
                _currentInteractableCandidate.Interact();
            }
        }

        private void OnGUI()
        {
            if (_currentPrompt == null || string.IsNullOrWhiteSpace(_currentPrompt.PromptText))
            {
                return;
            }

            var text = _currentPrompt.IsEnabled
                ? "[" + interactKey + "] " + _currentPrompt.PromptText
                : _currentPrompt.PromptText + " (" + _currentPrompt.DisabledReason + ")";

            GUI.Box(promptRect, text);
        }

        private void RefreshCandidates()
        {
            _currentVehicleCandidate = null;
            _currentInteractableCandidate = null;
            _currentPrompt = null;

            if (playerController == null || playerController.ControlsLocked)
            {
                return;
            }

            var colliders = Physics.OverlapSphere(transform.position, interactionRadius);
            var closestDistance = float.MaxValue;

            foreach (var hit in colliders)
            {
                if (hit == null)
                {
                    continue;
                }

                var seat = hit.GetComponentInParent<VehicleSeatController>();
                if (seat != null && !seat.IsOccupied)
                {
                    var seatDistance = Vector3.Distance(transform.position, seat.transform.position);
                    if (seatDistance < closestDistance)
                    {
                        closestDistance = seatDistance;
                        _currentVehicleCandidate = seat;
                        _currentInteractableCandidate = null;
                        _currentPrompt = InteractionPromptData.Enabled("Enter vehicle", "Interact");
                    }
                }

                var behaviours = hit.GetComponentsInParent<MonoBehaviour>();
                foreach (var behaviour in behaviours)
                {
                    var interactable = behaviour as IInteractable;
                    if (interactable == null)
                    {
                        continue;
                    }

                    var interactableDistance = Vector3.Distance(transform.position, behaviour.transform.position);
                    if (interactableDistance >= closestDistance)
                    {
                        continue;
                    }

                    closestDistance = interactableDistance;
                    _currentVehicleCandidate = null;
                    _currentInteractableCandidate = interactable;
                    _currentPrompt = interactable.GetPrompt();
                }
            }
        }

        private VehicleSeatController GetOccupiedSeatFromParents()
        {
            var current = transform.parent;
            while (current != null)
            {
                var seat = current.GetComponentInParent<VehicleSeatController>();
                if (seat != null && seat.IsOccupied)
                {
                    return seat;
                }

                current = current.parent;
            }

            return null;
        }
    }
}
