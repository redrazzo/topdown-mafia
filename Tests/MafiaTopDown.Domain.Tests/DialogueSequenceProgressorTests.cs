using MafiaTopDown.Gameplay.Domain.Story;

namespace MafiaTopDown.Domain.Tests;

public sealed class DialogueSequenceProgressorTests
{
    [Test]
    public void Starts_at_the_sequence_starting_node()
    {
        var sequence = BuildSequence();
        var progressor = new DialogueSequenceProgressor(sequence);

        Assert.That(progressor.CurrentNodeId, Is.EqualTo("luca-intro"));
        Assert.That(progressor.CurrentNode.SpeakerName, Is.EqualTo("Luca Moretti"));
        Assert.That(progressor.IsComplete, Is.False);
    }

    [Test]
    public void Advance_moves_to_the_next_node_until_complete()
    {
        var sequence = BuildSequence();
        var progressor = new DialogueSequenceProgressor(sequence);

        progressor.Advance();
        Assert.That(progressor.CurrentNodeId, Is.EqualTo("luca-warning"));

        progressor.Advance();
        Assert.That(progressor.CurrentNodeId, Is.EqualTo("tommy-answer"));

        progressor.Advance();
        Assert.That(progressor.IsComplete, Is.True);
        Assert.That(progressor.CurrentNode, Is.Null);
    }

    private static DialogueSequence BuildSequence()
    {
        return new DialogueSequence(
            "dialogue-luca-briefing",
            "Luca Briefing",
            "luca-intro",
            new[]
            {
                new ConversationNode("luca-intro", "Luca Moretti", "Take the Fleetline and keep your head down.", "luca-warning"),
                new ConversationNode("luca-warning", "Luca Moretti", "Nobody opens the ledger but Vincent.", "tommy-answer"),
                new ConversationNode("tommy-answer", "Tommy Bell", "I make the delivery and come straight back.", null)
            });
    }
}
