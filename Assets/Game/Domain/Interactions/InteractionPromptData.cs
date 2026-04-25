namespace MafiaTopDown.Gameplay.Domain.Interactions
{
    public sealed record InteractionPromptData(
        string PromptText,
        string ActionName,
        bool IsEnabled,
        string DisabledReason)
    {
        public static InteractionPromptData Enabled(string promptText, string actionName)
        {
            return new InteractionPromptData(promptText, actionName, true, null);
        }

        public static InteractionPromptData Disabled(string promptText, string actionName, string disabledReason)
        {
            return new InteractionPromptData(promptText, actionName, false, disabledReason);
        }
    }
}
