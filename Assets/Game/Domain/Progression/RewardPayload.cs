using System;
using System.Collections.Generic;

namespace MafiaTopDown.Gameplay.Domain.Progression
{
    public sealed class RewardPayload
    {
        public static readonly RewardPayload Empty = new RewardPayload(0, 0, Array.Empty<string>(), Array.Empty<string>());

        public RewardPayload(
            int cashReward,
            int reputationDelta,
            IEnumerable<string> unlockDistrictIds,
            IEnumerable<string> unlockActivityIds)
        {
            CashReward = cashReward;
            ReputationDelta = reputationDelta;
            UnlockDistrictIds = new List<string>(unlockDistrictIds);
            UnlockActivityIds = new List<string>(unlockActivityIds);
        }

        public int CashReward { get; }

        public int ReputationDelta { get; }

        public IReadOnlyList<string> UnlockDistrictIds { get; }

        public IReadOnlyList<string> UnlockActivityIds { get; }
    }
}
