namespace MafiaTopDown.Gameplay.Domain.Missions
{
    public sealed record ObjectiveDefinition(
        string Id,
        string Title,
        string SuccessCondition,
        string NextObjectiveId);
}
