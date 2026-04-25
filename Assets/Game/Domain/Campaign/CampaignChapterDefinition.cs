using System;
using System.Collections.Generic;
using System.Linq;

namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public sealed class CampaignChapterDefinition
    {
        public CampaignChapterDefinition(
            string id,
            string title,
            IEnumerable<string> missionIds,
            string nextChapterId = "")
        {
            Id = id;
            Title = title;
            MissionIds = missionIds.ToArray();
            NextChapterId = nextChapterId;

            if (MissionIds.Count == 0)
            {
                throw new ArgumentException("A chapter must contain at least one mission.", nameof(missionIds));
            }
        }

        public string Id { get; }

        public string Title { get; }

        public IReadOnlyList<string> MissionIds { get; }

        public string NextChapterId { get; }
    }
}
