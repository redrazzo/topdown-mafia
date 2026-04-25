namespace MafiaTopDown.Gameplay.Domain.Law
{
    public sealed class PoliceResponsePlan
    {
        public PoliceResponsePlan(PoliceResponseTier tier, int responderCount, string statusText)
        {
            Tier = tier;
            ResponderCount = responderCount;
            StatusText = statusText;
        }

        public PoliceResponseTier Tier { get; }

        public int ResponderCount { get; }

        public string StatusText { get; }
    }
}
