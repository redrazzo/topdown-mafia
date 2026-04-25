using MafiaTopDown.Gameplay.Domain.Campaign;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Domain.Tests;

public sealed class MissionProgressTrackerTests
{
    [Test]
    public void Starts_at_the_definition_starting_stage()
    {
        var mission = BuildMission();
        var tracker = new MissionProgressTracker(mission);

        Assert.That(tracker.CurrentStageId, Is.EqualTo("pickup"));
        Assert.That(tracker.IsComplete, Is.False);
    }

    [Test]
    public void Completing_a_stage_advances_to_the_next_stage()
    {
        var mission = BuildMission();
        var tracker = new MissionProgressTracker(mission);

        tracker.CompleteCurrentStage();

        Assert.That(tracker.CurrentStageId, Is.EqualTo("drive"));
        Assert.That(tracker.CompletedStageIds, Is.EquivalentTo(new[] { "pickup" }));
        Assert.That(tracker.IsComplete, Is.False);
    }

    [Test]
    public void Completing_the_final_stage_returns_the_completion_reward()
    {
        var mission = BuildMission();
        var tracker = new MissionProgressTracker(mission);

        tracker.CompleteCurrentStage();
        tracker.CompleteCurrentStage();
        var result = tracker.CompleteCurrentStage();

        Assert.That(result.MissionCompleted, Is.True);
        Assert.That(result.CompletionReward.CashReward, Is.EqualTo(125));
        Assert.That(result.CompletionReward.UnlockDistrictIds, Is.EquivalentTo(new[] { "business-core" }));
        Assert.That(tracker.IsComplete, Is.True);
        Assert.That(tracker.CurrentStageId, Is.Null);
    }

    private static MissionDefinition BuildMission()
    {
        return new MissionDefinition(
            id: "mission-vincent-ledger",
            title: "A Quiet Favor",
            summary: "Deliver the family ledger to Vincent.",
            startingStageId: "pickup",
            completionReward: new RewardPayload(
                cashReward: 125,
                reputationDelta: 1,
                unlockDistrictIds: new[] { "business-core" },
                unlockActivityIds: new[] { "dock-courier-job" }),
            stages: new[]
            {
                new MissionStage("pickup", "Take the car", "Get in the assigned sedan.", "drive", true),
                new MissionStage("drive", "Reach the office", "Drive to the back office.", "handoff", true),
                new MissionStage("handoff", "Hand over the ledger", "Deliver the ledger to Vincent.", null, false)
            },
            checkpoints: new[]
            {
                new MissionCheckpoint("pickup-checkpoint", "District_01", "PickupSpawn", "pickup"),
                new MissionCheckpoint("drive-checkpoint", "District_01", "DriveSpawn", "drive")
            },
            failureConditions: new[]
            {
                new FailureCondition("car-destroyed", FailureConditionType.VehicleDestroyed, "Lose the assigned sedan.")
            });
    }
}
