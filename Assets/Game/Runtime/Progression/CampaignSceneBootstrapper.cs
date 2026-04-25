using MafiaTopDown.Gameplay.Runtime.Data;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Progression
{
    public sealed class CampaignSceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private CampaignProgressionController campaignProgressionController;
        [SerializeField] private CampaignChapterAsset bootstrapChapter;
        [SerializeField] private MissionDefinitionAsset bootstrapMission;

        private void Awake()
        {
            if (campaignProgressionController == null)
            {
                return;
            }

            if (bootstrapChapter != null)
            {
                campaignProgressionController.EnsureChapterStarted(bootstrapChapter);
                return;
            }

            if (bootstrapMission != null)
            {
                campaignProgressionController.EnsureMissionStarted(bootstrapMission);
            }
        }
    }
}
