using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Domain.Tests;

public sealed class ResumeLocationResolverTests
{
    [Test]
    public void Exterior_resume_keeps_the_saved_location()
    {
        var save = SaveGameData.CreateFreshGame("District_BusinessCore_01", "BusinessSquareExitSpawn");

        var location = ResumeLocationResolver.Resolve(save, "District_01", "PickupSpawn");

        Assert.That(location.SceneName, Is.EqualTo("District_BusinessCore_01"));
        Assert.That(location.SpawnPointId, Is.EqualTo("BusinessSquareExitSpawn"));
    }

    [Test]
    public void Interior_resume_returns_to_the_matching_street_spawn()
    {
        var save = SaveGameData.CreateFreshGame("Interior_BackOffice_01", "LedgerDeskSpawn");

        var location = ResumeLocationResolver.Resolve(save, "District_01", "PickupSpawn");

        Assert.That(location.SceneName, Is.EqualTo("District_01"));
        Assert.That(location.SpawnPointId, Is.EqualTo("ExteriorReturn"));
    }

    [Test]
    public void Empty_resume_uses_the_configured_new_game_spawn()
    {
        var save = SaveGameData.CreateFreshGame(string.Empty, string.Empty);

        var location = ResumeLocationResolver.Resolve(save, "District_01", "PickupSpawn");

        Assert.That(location.SceneName, Is.EqualTo("District_01"));
        Assert.That(location.SpawnPointId, Is.EqualTo("PickupSpawn"));
    }
}
