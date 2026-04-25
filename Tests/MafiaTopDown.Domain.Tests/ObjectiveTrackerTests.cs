using MafiaTopDown.Gameplay.Domain.Missions;

namespace MafiaTopDown.Domain.Tests;

public sealed class ObjectiveTrackerTests
{
    [Test]
    public void Starts_with_the_first_objective_as_current()
    {
        var objectives = new[]
        {
            new ObjectiveDefinition("meet-vincent", "Meet Vincent", "Reach the curbside pickup point", "deliver-ledger"),
            new ObjectiveDefinition("deliver-ledger", "Deliver the ledger", "Enter the back office with the package", null)
        };

        var tracker = new ObjectiveTracker(objectives, "meet-vincent");

        Assert.That(tracker.CurrentObjectiveId, Is.EqualTo("meet-vincent"));
        Assert.That(tracker.IsComplete, Is.False);
    }

    [Test]
    public void Completing_the_current_objective_advances_to_the_next_one()
    {
        var objectives = new[]
        {
            new ObjectiveDefinition("meet-vincent", "Meet Vincent", "Reach the curbside pickup point", "deliver-ledger"),
            new ObjectiveDefinition("deliver-ledger", "Deliver the ledger", "Enter the back office with the package", null)
        };

        var tracker = new ObjectiveTracker(objectives, "meet-vincent");

        tracker.MarkCurrentObjectiveComplete();

        Assert.That(tracker.CurrentObjectiveId, Is.EqualTo("deliver-ledger"));
        Assert.That(tracker.CompletedObjectiveIds, Is.EquivalentTo(new[] { "meet-vincent" }));
        Assert.That(tracker.IsComplete, Is.False);
    }

    [Test]
    public void Completing_the_last_objective_marks_the_tracker_complete()
    {
        var objectives = new[]
        {
            new ObjectiveDefinition("meet-vincent", "Meet Vincent", "Reach the curbside pickup point", "deliver-ledger"),
            new ObjectiveDefinition("deliver-ledger", "Deliver the ledger", "Enter the back office with the package", null)
        };

        var tracker = new ObjectiveTracker(objectives, "meet-vincent");

        tracker.MarkCurrentObjectiveComplete();
        tracker.MarkCurrentObjectiveComplete();

        Assert.That(tracker.CurrentObjectiveId, Is.Null);
        Assert.That(tracker.IsComplete, Is.True);
        Assert.That(
            tracker.CompletedObjectiveIds,
            Is.EquivalentTo(new[] { "meet-vincent", "deliver-ledger" }));
    }
}
