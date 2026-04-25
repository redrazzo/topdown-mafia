using MafiaTopDown.Gameplay.Domain.Progression;
using MafiaTopDown.Gameplay.Domain.World;

namespace MafiaTopDown.Domain.Tests;

public sealed class SideJobProgressionServiceTests
{
    [Test]
    public void Starting_an_unlocked_side_job_sets_the_current_activity_id()
    {
        var saveData = new SaveGameData(
            currentMissionId: null,
            currentMissionStageId: null,
            lastSceneName: "District_01",
            lastSpawnPointId: "PickupSpawn",
            cash: 25,
            reputation: 0,
            heatLevel: 0,
            completedMissionIds: Array.Empty<string>(),
            unlockedDistrictIds: new[] { "docks", "business-core" },
            unlockedActivityIds: new[] { "activity-dock-courier" },
            currentChapterId: null,
            completedChapterIds: Array.Empty<string>(),
            currentActivityId: null,
            completedActivityIds: Array.Empty<string>());

        var activity = new ActivityDefinition("activity-dock-courier", "Dock Courier", ActivityType.CourierJob, "docks", 35, true, null);

        var updated = SideJobProgressionService.StartActivity(saveData, activity);

        Assert.That(updated.CurrentActivityId, Is.EqualTo("activity-dock-courier"));
        Assert.That(updated.Cash, Is.EqualTo(25));
    }

    [Test]
    public void Completing_the_active_side_job_awards_cash_and_clears_the_activity()
    {
        var saveData = new SaveGameData(
            currentMissionId: null,
            currentMissionStageId: null,
            lastSceneName: "District_BusinessCore_01",
            lastSpawnPointId: "BusinessCoreStart",
            cash: 25,
            reputation: 0,
            heatLevel: 0,
            completedMissionIds: Array.Empty<string>(),
            unlockedDistrictIds: new[] { "docks", "business-core" },
            unlockedActivityIds: new[] { "activity-dock-courier" },
            currentChapterId: null,
            completedChapterIds: Array.Empty<string>(),
            currentActivityId: "activity-dock-courier",
            completedActivityIds: Array.Empty<string>());

        var activity = new ActivityDefinition("activity-dock-courier", "Dock Courier", ActivityType.CourierJob, "docks", 35, true, null);

        var updated = SideJobProgressionService.CompleteActivity(saveData, activity);

        Assert.That(updated.CurrentActivityId, Is.Null);
        Assert.That(updated.Cash, Is.EqualTo(60));
        Assert.That(updated.CompletedActivityIds, Is.EquivalentTo(new[] { "activity-dock-courier" }));
    }

    [Test]
    public void Locked_side_jobs_cannot_start()
    {
        var saveData = SaveGameData.CreateFreshGame("District_01", "DefaultSpawn");
        var activity = new ActivityDefinition("activity-ledger-run", "Ledger Run", ActivityType.CourierJob, "business-core", 90, true, "mission-a-quiet-favor");

        var updated = SideJobProgressionService.StartActivity(saveData, activity);

        Assert.That(updated.CurrentActivityId, Is.Null);
    }
}
