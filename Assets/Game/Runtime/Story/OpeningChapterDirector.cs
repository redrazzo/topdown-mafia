using MafiaTopDown.Gameplay.Runtime.Missions;
using MafiaTopDown.Gameplay.Runtime.Scenes;
using MafiaTopDown.Gameplay.Runtime.Vehicles;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Story
{
    public sealed class OpeningChapterDirector : MonoBehaviour
    {
        [SerializeField] private SimpleObjectiveSystem objectiveSystem;
        [SerializeField] private VehicleSeatController assignedVehicle;
        [SerializeField] private SceneTransitionController sceneTransitionController;
        [SerializeField] private string exteriorSceneName = "District_01";
        [SerializeField] private string interiorSceneName = "Interior_BackOffice_01";
        [SerializeField] private string interiorSpawnPointId = "BackOfficeDoor";

        public void OnVehicleEntered()
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.CompleteCurrentObjective();
            }
        }

        public void OnArrivalAtBuilding()
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.CompleteCurrentObjective();
            }
        }

        public void OnEnterBackOffice()
        {
            if (sceneTransitionController == null)
            {
                return;
            }

            sceneTransitionController.TransitionToExteriorInteriorPair(
                exteriorSceneName,
                interiorSceneName,
                interiorSpawnPointId);
        }

        public void OnLedgerHandoffCompleted()
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.CompleteCurrentObjective();
            }
        }

        public bool HasAssignedVehicle()
        {
            return assignedVehicle != null;
        }
    }
}
