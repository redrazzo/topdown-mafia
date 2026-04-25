using MafiaTopDown.Gameplay.Domain.Progression;
using MafiaTopDown.Gameplay.Domain.World;

namespace MafiaTopDown.Domain.Tests;

public sealed class WorldTravelPlannerTests
{
    [Test]
    public void Returns_only_unlocked_districts_other_than_the_current_one()
    {
        var saveData = new SaveGameData(
            currentMissionId: null,
            currentMissionStageId: null,
            lastSceneName: "District_01",
            lastSpawnPointId: "PickupSpawn",
            cash: 0,
            reputation: 0,
            heatLevel: 0,
            completedMissionIds: new[] { "mission-a-quiet-favor" },
            unlockedDistrictIds: new[] { "docks", "business-core" },
            unlockedActivityIds: Array.Empty<string>());

        var districts = new[]
        {
            new DistrictDefinition("docks", "Harbor Docks", "District_01", DistrictTheme.Docks, true, null, new[] { "landmark-harbor-office" }, new[] { "activity-dock-courier" }),
            new DistrictDefinition("business-core", "Business Core", "District_BusinessCore_01", DistrictTheme.BusinessCore, false, "mission-a-quiet-favor", new[] { "landmark-union-square" }, new[] { "activity-ledger-run" }),
            new DistrictDefinition("old-quarter", "Old Quarter", "District_OldQuarter_01", DistrictTheme.Residential, false, "mission-paid-debts", new[] { "landmark-saint-vera" }, new[] { "activity-lookout-run" })
        };

        var destinations = WorldTravelPlanner.GetTravelDestinations(saveData, districts, "docks");

        Assert.That(destinations.Select(item => item.Id), Is.EquivalentTo(new[] { "business-core" }));
        Assert.That(destinations[0].SceneName, Is.EqualTo("District_BusinessCore_01"));
    }
}
