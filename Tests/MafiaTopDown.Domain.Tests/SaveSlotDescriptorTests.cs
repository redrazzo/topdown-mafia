using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Domain.Tests;

public sealed class SaveSlotDescriptorTests
{
    [Test]
    public void EmptySlotReportsNoPersistedData()
    {
        var descriptor = SaveSlotDescriptor.Empty(2);

        Assert.That(descriptor.SlotIndex, Is.EqualTo(2));
        Assert.That(descriptor.Exists, Is.False);
        Assert.That(descriptor.DisplayName, Is.EqualTo("Empty Slot"));
        Assert.That(descriptor.Summary, Is.EqualTo("No save data yet"));
    }

    [Test]
    public void DescriptorFromSaveDataExposesResumeSummary()
    {
        var save = new SaveGameData(
            currentMissionId: "quiet-favor",
            currentMissionStageId: "reach-office",
            lastSceneName: "District_Docks",
            lastSpawnPointId: "BackOfficeReturn",
            cash: 125,
            reputation: 1,
            heatLevel: 2,
            completedMissionIds: new[] { "intro" },
            unlockedDistrictIds: new[] { "docks" },
            unlockedActivityIds: new[] { "courier-docks" },
            currentChapterId: "act-1",
            completedChapterIds: System.Array.Empty<string>());

        var descriptor = SaveSlotDescriptor.FromSaveGameData(1, save, 99L);

        Assert.That(descriptor.Exists, Is.True);
        Assert.That(descriptor.DisplayName, Is.EqualTo("act-1 - quiet-favor"));
        Assert.That(descriptor.Summary, Is.EqualTo("quiet-favor | District_Docks | $125 | Heat 2"));
        Assert.That(descriptor.LastWriteUtcTicks, Is.EqualTo(99L));
    }
}
