using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Domain.Tests;

public sealed class SaveGameDataTests
{
    [Test]
    public void Fresh_game_unlocks_the_launch_city_for_free_roam()
    {
        var saveData = SaveGameData.CreateFreshGame("District_01", "DefaultSpawn");

        Assert.That(
            saveData.UnlockedDistrictIds,
            Is.EquivalentTo(new[] { "docks", "business-core", "old-quarter", "rail-yard" }));
    }
}
