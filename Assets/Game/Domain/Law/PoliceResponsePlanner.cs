namespace MafiaTopDown.Gameplay.Domain.Law
{
    public static class PoliceResponsePlanner
    {
        public static PoliceResponsePlan Plan(HeatState heatState)
        {
            return heatState.ResponseTier switch
            {
                PoliceResponseTier.Watch => new PoliceResponsePlan(PoliceResponseTier.Watch, 1, "Keep your head down"),
                PoliceResponseTier.Patrol => new PoliceResponsePlan(PoliceResponseTier.Patrol, 2, "Patrols are looking"),
                PoliceResponseTier.Hunt => new PoliceResponsePlan(PoliceResponseTier.Hunt, 3, "Citywide manhunt"),
                _ => new PoliceResponsePlan(PoliceResponseTier.None, 0, "Clear")
            };
        }
    }
}
