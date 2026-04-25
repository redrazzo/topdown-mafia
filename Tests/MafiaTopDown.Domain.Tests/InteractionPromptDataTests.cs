using MafiaTopDown.Gameplay.Domain.Interactions;

namespace MafiaTopDown.Domain.Tests;

public sealed class InteractionPromptDataTests
{
    [Test]
    public void Creates_an_enabled_prompt_for_available_interactions()
    {
        var prompt = InteractionPromptData.Enabled("Enter the sedan", "Interact");

        Assert.That(prompt.PromptText, Is.EqualTo("Enter the sedan"));
        Assert.That(prompt.ActionName, Is.EqualTo("Interact"));
        Assert.That(prompt.IsEnabled, Is.True);
        Assert.That(prompt.DisabledReason, Is.Null);
    }

    [Test]
    public void Creates_a_disabled_prompt_when_a_reason_is_present()
    {
        var prompt = InteractionPromptData.Disabled("Door is locked", "Interact", "You need the office key");

        Assert.That(prompt.PromptText, Is.EqualTo("Door is locked"));
        Assert.That(prompt.ActionName, Is.EqualTo("Interact"));
        Assert.That(prompt.IsEnabled, Is.False);
        Assert.That(prompt.DisabledReason, Is.EqualTo("You need the office key"));
    }
}
