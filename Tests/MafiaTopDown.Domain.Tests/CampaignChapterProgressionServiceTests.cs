using MafiaTopDown.Gameplay.Domain.Campaign;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Domain.Tests;

public sealed class CampaignChapterProgressionServiceTests
{
    [Test]
    public void Starting_a_chapter_sets_the_current_chapter_and_first_mission()
    {
        var saveData = SaveGameData.CreateFreshGame("District_01", "DefaultSpawn");
        var chapter = BuildChapter();
        var missions = BuildMissions();

        var updated = CampaignChapterProgressionService.StartChapter(saveData, chapter, missions);

        Assert.That(updated.CurrentChapterId, Is.EqualTo("chapter-act-1"));
        Assert.That(updated.CurrentMissionId, Is.EqualTo("mission-a-quiet-favor"));
        Assert.That(updated.CurrentMissionStageId, Is.EqualTo("pickup"));
        Assert.That(updated.LastSceneName, Is.EqualTo("District_01"));
        Assert.That(updated.LastSpawnPointId, Is.EqualTo("PickupSpawn"));
    }

    [Test]
    public void Advancing_a_chapter_after_a_completed_mission_selects_the_next_mission()
    {
        var saveData = SaveGameMutator.MarkMissionCompleted(
            new SaveGameData(
                currentMissionId: "mission-a-quiet-favor",
                currentMissionStageId: null,
                lastSceneName: "District_01",
                lastSpawnPointId: "ExteriorReturn",
                cash: 0,
                reputation: 0,
                heatLevel: 0,
                completedMissionIds: Array.Empty<string>(),
                unlockedDistrictIds: new[] { "docks", "business-core" },
                unlockedActivityIds: Array.Empty<string>(),
                currentChapterId: "chapter-act-1",
                completedChapterIds: Array.Empty<string>()),
            "mission-a-quiet-favor");

        var updated = CampaignChapterProgressionService.AdvanceAfterMissionCompletion(saveData, BuildChapter(), BuildMissions());

        Assert.That(updated.CurrentChapterId, Is.EqualTo("chapter-act-1"));
        Assert.That(updated.CurrentMissionId, Is.EqualTo("mission-union-due"));
        Assert.That(updated.CurrentMissionStageId, Is.EqualTo("meet-bookkeeper"));
        Assert.That(updated.LastSceneName, Is.EqualTo("District_BusinessCore_01"));
    }

    [Test]
    public void Completing_the_last_mission_marks_the_chapter_complete()
    {
        var saveData = SaveGameMutator.MarkMissionCompleted(
            new SaveGameData(
                currentMissionId: "mission-yard-heat",
                currentMissionStageId: null,
                lastSceneName: "District_RailYard_01",
                lastSpawnPointId: "RailYardStart",
                cash: 0,
                reputation: 0,
                heatLevel: 0,
                completedMissionIds: new[]
                {
                    "mission-a-quiet-favor",
                    "mission-union-due",
                    "mission-chapel-debt"
                },
                unlockedDistrictIds: new[] { "docks", "business-core", "old-quarter", "rail-yard" },
                unlockedActivityIds: Array.Empty<string>(),
                currentChapterId: "chapter-act-1",
                completedChapterIds: Array.Empty<string>()),
            "mission-yard-heat");

        var updated = CampaignChapterProgressionService.AdvanceAfterMissionCompletion(saveData, BuildChapter(), BuildMissions());

        Assert.That(updated.CurrentChapterId, Is.Null);
        Assert.That(updated.CurrentMissionId, Is.Null);
        Assert.That(updated.CurrentMissionStageId, Is.Null);
        Assert.That(updated.CompletedChapterIds, Is.EquivalentTo(new[] { "chapter-act-1" }));
    }

    [Test]
    public void Completing_the_last_mission_can_advance_into_the_next_chapter()
    {
        var saveData = SaveGameMutator.MarkMissionCompleted(
            new SaveGameData(
                currentMissionId: "mission-yard-heat",
                currentMissionStageId: null,
                lastSceneName: "District_RailYard_01",
                lastSpawnPointId: "RailYardStart",
                cash: 0,
                reputation: 0,
                heatLevel: 0,
                completedMissionIds: new[]
                {
                    "mission-a-quiet-favor",
                    "mission-union-due",
                    "mission-chapel-debt"
                },
                unlockedDistrictIds: new[] { "docks", "business-core", "old-quarter", "rail-yard" },
                unlockedActivityIds: Array.Empty<string>(),
                currentChapterId: "chapter-act-1",
                completedChapterIds: Array.Empty<string>()),
            "mission-yard-heat");

        var updated = CampaignChapterProgressionService.AdvanceAfterMissionCompletion(
            saveData,
            BuildChapter(),
            BuildMissions(),
            BuildActTwoChapter(),
            BuildActTwoMissions());

        Assert.That(updated.CompletedChapterIds, Is.EquivalentTo(new[] { "chapter-act-1" }));
        Assert.That(updated.CurrentChapterId, Is.EqualTo("chapter-act-2"));
        Assert.That(updated.CurrentMissionId, Is.EqualTo("mission-bellori-books"));
        Assert.That(updated.CurrentMissionStageId, Is.EqualTo("inspect-ledgers"));
        Assert.That(updated.LastSceneName, Is.EqualTo("Interior_BusinessBookkeeper_01"));
        Assert.That(updated.LastSpawnPointId, Is.EqualTo("BusinessInteriorSpawn"));
    }

    private static CampaignChapterDefinition BuildChapter()
    {
        return new CampaignChapterDefinition(
            "chapter-act-1",
            "Act I - First Debts",
            new[]
            {
                "mission-a-quiet-favor",
                "mission-union-due",
                "mission-chapel-debt",
                "mission-yard-heat"
            },
            "chapter-act-2");
    }

    private static CampaignChapterDefinition BuildActTwoChapter()
    {
        return new CampaignChapterDefinition(
            "chapter-act-2",
            "Act II - Open Accounts",
            new[]
            {
                "mission-bellori-books",
                "mission-saint-vera-silence",
                "mission-pier-night-watch",
                "mission-ironline-ledger",
                "mission-blood-ledger"
            },
            "chapter-act-3");
    }

    private static MissionDefinition[] BuildMissions()
    {
        return new[]
        {
            BuildMission("mission-a-quiet-favor", "pickup", "District_01", "PickupSpawn"),
            BuildMission("mission-union-due", "meet-bookkeeper", "District_BusinessCore_01", "BusinessCoreStart"),
            BuildMission("mission-chapel-debt", "find-debtor", "District_OldQuarter_01", "OldQuarterStart"),
            BuildMission("mission-yard-heat", "check-garage", "District_RailYard_01", "RailYardStart")
        };
    }

    private static MissionDefinition[] BuildActTwoMissions()
    {
        return new[]
        {
            BuildMission("mission-bellori-books", "inspect-ledgers", "Interior_BusinessBookkeeper_01", "BusinessInteriorSpawn"),
            BuildMission("mission-saint-vera-silence", "search-vestry", "Interior_OldQuarter_Chapel_01", "ChapelInteriorSpawn"),
            BuildMission("mission-pier-night-watch", "check-pier", "District_01", "ExteriorReturn"),
            BuildMission("mission-ironline-ledger", "inspect-garage-ledger", "Interior_RailYard_Garage_01", "GarageInteriorSpawn"),
            BuildMission("mission-blood-ledger", "read-ledger", "Interior_BackOffice_01", "LedgerDeskSpawn")
        };
    }

    private static MissionDefinition BuildMission(string id, string stageId, string sceneName, string spawnPointId)
    {
        return new MissionDefinition(
            id,
            id,
            id,
            stageId,
            RewardPayload.Empty,
            new[]
            {
                new MissionStage(stageId, stageId, stageId, null, true)
            },
            new[]
            {
                new MissionCheckpoint(id + "-checkpoint", sceneName, spawnPointId, stageId)
            },
            Array.Empty<FailureCondition>());
    }
}
