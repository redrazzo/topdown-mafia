using System;
using System.Collections.Generic;
using System.Linq;

namespace MafiaTopDown.Gameplay.Domain.Story
{
    public sealed class DialogueSequence
    {
        private readonly Dictionary<string, ConversationNode> _nodesById;

        public DialogueSequence(
            string id,
            string displayName,
            string startingNodeId,
            IEnumerable<ConversationNode> nodes)
        {
            Id = id;
            DisplayName = displayName;
            StartingNodeId = startingNodeId;
            Nodes = nodes.ToArray();
            _nodesById = Nodes.ToDictionary(node => node.Id);

            if (!_nodesById.ContainsKey(startingNodeId))
            {
                throw new ArgumentException("Starting node must exist in the sequence node list.", nameof(startingNodeId));
            }
        }

        public string Id { get; }

        public string DisplayName { get; }

        public string StartingNodeId { get; }

        public IReadOnlyList<ConversationNode> Nodes { get; }

        public ConversationNode GetNode(string nodeId)
        {
            return _nodesById[nodeId];
        }
    }
}
