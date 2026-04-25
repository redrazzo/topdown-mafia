using MafiaTopDown.Gameplay.Domain.Progression;
using MafiaTopDown.Gameplay.Domain.World;

namespace MafiaTopDown.Domain.Tests;

public sealed class WorldUnlockServiceTests
{
    [Test]
    public void Unlocks_districts_and_activities_tied_to_completed_missions()
    {
        var saveData = SaveGameMutator.MarkMissionCompleted(
            SaveGameData.CreateFreshGame("District_01", "DefaultSpawn"),
            "mission-vincent-ledger");

        var districts = new[]
        {
            new DistrictDefinition("docks", "The Docks", "District_01", DistrictTheme.Docks, true, null, new[] { "landmark-harbor" }, new[] { "activity-dock-run" }),
            new DistrictDefinition("business-core", "Business Core", "District_BusinessCore_01", DistrictTheme.BusinessCore, false, "mission-vincent-ledger", new[] { "landmark-bank" }, new[] { "activity-ledger-run" })
        };

        var activities = new[]
        {
            new ActivityDefinition("activity-dock-run", "Harbor Courier", ActivityType.CourierJob, "docks", 35, true, null),
            new ActivityDefinition("activity-ledger-run", "Downtown Ledger Run", ActivityType.CourierJob, "business-core", 90, true, "mission-vincent-ledger")
        };

        var unlocked = WorldUnlockService.ApplyMissionUnlocks(saveData, districts, activities);

        Assert.That(unlocked.UnlockedDistrictIds, Is.EquivalentTo(new[] { "docks", "business-core" }));
        Assert.That(unlocked.UnlockedActivityIds, Is.EquivalentTo(new[] { "activity-dock-run", "activity-ledger-run" }));
    }

    [Test]
    public void Preserves_active_and_completed_activity_state_while_applying_unlocks()
    {
        var saveData = new SaveGameData(
            currentMissionId: null,
            currentMissionStageId: null,
            lastSceneName: "District_OldQuarter_01",
            lastSpawnPointId: "OldQuarterStart",
            cash: 110,
            reputation: 1,
            heatLevel: 0,
            completedMissionIds: new[] { "mission-union-due" },
            unlockedDistrictIds: new[] { "docks", "business-core", "old-quarter" },
            unlockedActivityIds: new[] { "activity-dock-run", "activity-ledger-run", "activity-lookout-run" },
            currentChapterId: "chapter-act-1",
            completedChapterIds: Array.Empty<string>(),
            currentActivityId: "activity-lookout-run",
            completedActivityIds: new[] { "activity-dock-run" });

        var districts = new[]
        {
            new DistrictDefinition("docks", "The Docks", "District_01", DistrictTheme.Docks, true, null, Array.Empty<string>(), Array.Empty<string>()),
            new DistrictDefinition("business-core", "Business Core", "District_BusinessCore_01", DistrictTheme.BusinessCore, false, "mission-a-quiet-favor", Array.Empty<string>(), Array.Empty<string>()),
            new DistrictDefinition("old-quarter", "Old Quarter", "District_OldQuarter_01", DistrictTheme.Residential, false, "mission-union-due", Array.Empty<string>(), Array.Empty<string>())
        };

        var activities = new[]
        {
            new ActivityDefinition("activity-dock-run", "Harbor Courier", ActivityType.CourierJob, "docks", 35, true, null),
            new ActivityDefinition("activity-ledger-run", "Downtown Ledger Run", ActivityType.CourierJob, "business-core", 90, true, "mission-a-quiet-favor"),
            new ActivityDefinition("activity-lookout-run", "Church Lookout", ActivityType.Lookout, "old-quarter", 110, true, "mission-union-due")
        };

        var unlocked = WorldUnlockService.ApplyMissionUnlocks(saveData, districts, activities);

        Assert.That(unlocked.CurrentActivityId, Is.EqualTo("activity-lookout-run"));
        Assert.That(unlocked.CompletedActivityIds, Is.EquivalentTo(new[] { "activity-dock-run" }));
    }
}
