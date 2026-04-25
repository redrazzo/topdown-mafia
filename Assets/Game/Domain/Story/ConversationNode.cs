namespace MafiaTopDown.Gameplay.Domain.Story
{
    public sealed class ConversationNode
    {
        public ConversationNode(
            string id,
            string speakerName,
            string text,
            string? nextNodeId)
        {
            Id = id;
            SpeakerName = speakerName;
            Text = text;
            NextNodeId = nextNodeId;
        }

        public string Id { get; }

        public string SpeakerName { get; }

        public string Text { get; }

        public string? NextNodeId { get; }
    }
}
