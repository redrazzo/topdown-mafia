using MafiaTopDown.Gameplay.Domain.Campaign;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Domain.Tests;

public sealed class CampaignMissionServiceTests
{
    [Test]
    public void Starting_a_mission_sets_the_active_stage_and_checkpoint_location()
    {
        var mission = BuildMission();
        var saveData = SaveGameData.CreateFreshGame("District_01", "DefaultSpawn");

        var started = CampaignMissionService.StartMission(saveData, mission);

        Assert.That(started.CurrentMissionId, Is.EqualTo("mission-a-quiet-favor"));
        Assert.That(started.CurrentMissionStageId, Is.EqualTo("pickup"));
        Assert.That(started.LastSceneName, Is.EqualTo("District_01"));
        Assert.That(started.LastSpawnPointId, Is.EqualTo("PickupSpawn"));
    }

    [Test]
    public void Completing_a_stage_advances_the_mission_and_updates_the_checkpoint_location()
    {
        var mission = BuildMission();
        var started = CampaignMissionService.StartMission(
            SaveGameData.CreateFreshGame("District_01", "DefaultSpawn"),
            mission);

        var advanced = CampaignMissionService.CompleteCurrentStage(started, mission);

        Assert.That(advanced.CurrentMissionId, Is.EqualTo("mission-a-quiet-favor"));
        Assert.That(advanced.CurrentMissionStageId, Is.EqualTo("drive"));
        Assert.That(advanced.LastSceneName, Is.EqualTo("District_01"));
        Assert.That(advanced.LastSpawnPointId, Is.EqualTo("DriveSpawn"));
        Assert.That(advanced.CompletedMissionIds, Is.Empty);
    }

    [Test]
    public void Completing_the_final_stage_clears_active_mission_and_applies_the_completion_reward()
    {
        var mission = BuildMission();
        var saveData = CampaignMissionService.StartMission(
            SaveGameData.CreateFreshGame("District_01", "DefaultSpawn"),
            mission);

        saveData = CampaignMissionService.CompleteCurrentStage(saveData, mission);
        saveData = CampaignMissionService.CompleteCurrentStage(saveData, mission);
        saveData = CampaignMissionService.CompleteCurrentStage(saveData, mission);
        saveData = CampaignMissionService.CompleteCurrentStage(saveData, mission);

        Assert.That(saveData.CurrentMissionId, Is.Null);
        Assert.That(saveData.CurrentMissionStageId, Is.Null);
        Assert.That(saveData.Cash, Is.EqualTo(200));
        Assert.That(saveData.Reputation, Is.EqualTo(2));
        Assert.That(saveData.CompletedMissionIds, Is.EquivalentTo(new[] { "mission-a-quiet-favor" }));
        Assert.That(saveData.UnlockedDistrictIds, Does.Contain("business-core"));
        Assert.That(saveData.UnlockedActivityIds, Does.Contain("activity-ledger-run"));
    }

    private static MissionDefinition BuildMission()
    {
        return new MissionDefinition(
            id: "mission-a-quiet-favor",
            title: "A Quiet Favor",
            summary: "Take the sedan, drive to the office, hand over the ledger, and leave clean.",
            startingStageId: "pickup",
            completionReward: new RewardPayload(
                cashReward: 200,
                reputationDelta: 2,
                unlockDistrictIds: new[] { "business-core" },
                unlockActivityIds: new[] { "activity-ledger-run" }),
            stages: new[]
            {
                new MissionStage("pickup", "Take the sedan", "Enter the assigned sedan.", "drive", true),
                new MissionStage("drive", "Reach the office", "Drive to the family office.", "handoff", true),
                new MissionStage("handoff", "Hand over the ledger", "Deliver the ledger inside the back office.", "leave", true),
                new MissionStage("leave", "Leave the office", "Walk back outside without drawing heat.", null, false)
            },
            checkpoints: new[]
            {
                new MissionCheckpoint("pickup-checkpoint", "District_01", "PickupSpawn", "pickup"),
                new MissionCheckpoint("drive-checkpoint", "District_01", "DriveSpawn", "drive"),
                new MissionCheckpoint("handoff-checkpoint", "Interior_BackOffice_01", "InteriorSpawn", "handoff"),
                new MissionCheckpoint("leave-checkpoint", "Interior_BackOffice_01", "LedgerDeskSpawn", "leave")
            },
            failureConditions: new[]
            {
                new FailureCondition("assigned-car-lost", FailureConditionType.VehicleDestroyed, "Do not lose the sedan.")
            });
    }
}
