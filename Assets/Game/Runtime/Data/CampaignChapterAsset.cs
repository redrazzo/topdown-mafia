using System.Linq;
using MafiaTopDown.Gameplay.Domain.Campaign;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Campaign Chapter", fileName = "CampaignChapter")]
    public sealed class CampaignChapterAsset : ScriptableObject
    {
        [SerializeField] private string chapterId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private MissionDefinitionAsset[] missions = new MissionDefinitionAsset[0];
        [SerializeField] private CampaignChapterAsset nextChapter;

        public string ChapterId => chapterId;

        public MissionDefinitionAsset[] Missions => missions;

        public CampaignChapterAsset NextChapterAsset => nextChapter;

        public CampaignChapterDefinition ToDefinition()
        {
            return new CampaignChapterDefinition(
                chapterId,
                displayName,
                missions.Select(mission => mission.MissionId),
                nextChapter == null ? string.Empty : nextChapter.ChapterId);
        }
    }
}
