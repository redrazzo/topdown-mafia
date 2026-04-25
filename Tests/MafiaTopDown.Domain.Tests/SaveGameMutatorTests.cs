using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Domain.Tests;

public sealed class SaveGameMutatorTests
{
    [Test]
    public void Applying_a_reward_updates_cash_reputation_and_unlocks()
    {
        var saveData = new SaveGameData(
            currentMissionId: "mission-vincent-ledger",
            currentMissionStageId: "handoff",
            lastSceneName: "Interior_BackOffice_01",
            lastSpawnPointId: "InteriorSpawn",
            cash: 40,
            reputation: 0,
            heatLevel: 0,
            completedMissionIds: Array.Empty<string>(),
            unlockedDistrictIds: new[] { "docks" },
            unlockedActivityIds: Array.Empty<string>());

        var reward = new RewardPayload(
            cashReward: 125,
            reputationDelta: 2,
            unlockDistrictIds: new[] { "business-core" },
            unlockActivityIds: new[] { "dock-courier-job" });

        var updated = SaveGameMutator.ApplyReward(saveData, reward);

        Assert.That(updated.Cash, Is.EqualTo(165));
        Assert.That(updated.Reputation, Is.EqualTo(2));
        Assert.That(updated.UnlockedDistrictIds, Is.EquivalentTo(new[] { "docks", "business-core" }));
        Assert.That(updated.UnlockedActivityIds, Is.EquivalentTo(new[] { "dock-courier-job" }));
    }

    [Test]
    public void Completing_a_mission_marks_it_completed_once()
    {
        var saveData = SaveGameData.CreateFreshGame("District_01", "DefaultSpawn");

        var updated = SaveGameMutator.MarkMissionCompleted(saveData, "mission-vincent-ledger");
        updated = SaveGameMutator.MarkMissionCompleted(updated, "mission-vincent-ledger");

        Assert.That(updated.CompletedMissionIds, Is.EquivalentTo(new[] { "mission-vincent-ledger" }));
    }
}
