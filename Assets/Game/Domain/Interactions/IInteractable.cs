namespace MafiaTopDown.Gameplay.Domain.Interactions
{
    public interface IInteractable
    {
        bool CanInteract { get; }

        InteractionPromptData GetPrompt();

        void Interact();
    }
}
