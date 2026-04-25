namespace MafiaTopDown.Gameplay.Domain.Story
{
    public sealed class DialogueSequenceProgressor
    {
        public DialogueSequenceProgressor(DialogueSequence dialogueSequence)
        {
            DialogueSequence = dialogueSequence;
            CurrentNodeId = dialogueSequence.StartingNodeId;
        }

        public DialogueSequence DialogueSequence { get; }

        public string? CurrentNodeId { get; private set; }

        public ConversationNode? CurrentNode => CurrentNodeId == null ? null : DialogueSequence.GetNode(CurrentNodeId);

        public bool IsComplete => CurrentNodeId == null;

        public void Advance()
        {
            if (CurrentNodeId == null)
            {
                return;
            }

            var currentNode = CurrentNode;
            if (currentNode == null)
            {
                CurrentNodeId = null;
                return;
            }

            var nextNodeId = currentNode.NextNodeId;
            CurrentNodeId = string.IsNullOrWhiteSpace(nextNodeId) ? null : nextNodeId;
        }
    }
}
