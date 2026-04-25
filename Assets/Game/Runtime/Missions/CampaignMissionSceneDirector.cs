using System.Linq;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Missions
{
    public sealed class CampaignMissionSceneDirector : MonoBehaviour
    {
        [SerializeField] private SaveGameFileService saveGameFileService;
        [SerializeField] private SimpleObjectiveSystem objectiveSystem;
        [SerializeField] private MissionSceneBinding[] missionBindings = new MissionSceneBinding[0];

        private void Start()
        {
            if (saveGameFileService == null || objectiveSystem == null)
            {
                return;
            }

            foreach (var binding in missionBindings)
            {
                if (binding.Root != null)
                {
                    binding.Root.SetActive(false);
                }
            }

            var save = saveGameFileService.LoadOrCreateSave();
            var activeBinding = missionBindings.FirstOrDefault(binding =>
                binding.MissionAsset != null &&
                binding.MissionAsset.MissionId == save.CurrentMissionId);

            if (activeBinding == null)
            {
                objectiveSystem.ClearObjectives();
                return;
            }

            if (activeBinding.Root != null)
            {
                activeBinding.Root.SetActive(true);
            }

            objectiveSystem.Configure(
                activeBinding.ToObjectiveSteps(),
                activeBinding.ResolveStartingObjectiveId(save.CurrentMissionStageId));
        }

        [System.Serializable]
        public sealed class MissionSceneBinding
        {
            public MissionDefinitionAsset MissionAsset;
            public GameObject Root;
            public MissionObjectiveStep[] ObjectiveSteps = new MissionObjectiveStep[0];

            public SimpleObjectiveSystem.ObjectiveStep[] ToObjectiveSteps()
            {
                var output = new SimpleObjectiveSystem.ObjectiveStep[ObjectiveSteps.Length];
                for (var index = 0; index < ObjectiveSteps.Length; index += 1)
                {
                    output[index] = ObjectiveSteps[index].ToObjectiveStep();
                }

                return output;
            }

            public string ResolveStartingObjectiveId(string savedObjectiveId)
            {
                if (!string.IsNullOrWhiteSpace(savedObjectiveId) &&
                    ObjectiveSteps.Any(step => step.Id == savedObjectiveId))
                {
                    return savedObjectiveId;
                }

                return ObjectiveSteps.Length == 0 ? string.Empty : ObjectiveSteps[0].Id;
            }
        }

        [System.Serializable]
        public sealed class MissionObjectiveStep
        {
            public string Id = string.Empty;
            public string Title = string.Empty;
            public string SuccessCondition = string.Empty;
            public string NextObjectiveId = string.Empty;

            public SimpleObjectiveSystem.ObjectiveStep ToObjectiveStep()
            {
                return new SimpleObjectiveSystem.ObjectiveStep
                {
                    Id = Id,
                    Title = Title,
                    SuccessCondition = SuccessCondition,
                    NextObjectiveId = NextObjectiveId
                };
            }
        }
    }
}
