using MafiaTopDown.Gameplay.Domain.Story;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Dialogue Sequence", fileName = "DialogueSequence")]
    public sealed class DialogueSequenceAsset : ScriptableObject
    {
        [SerializeField] private string dialogueId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private string startingNodeId = string.Empty;
        [SerializeField] private DialogueNodeRecord[] nodes = new DialogueNodeRecord[0];

        public DialogueSequence ToDefinition()
        {
            var definitionNodes = new ConversationNode[nodes.Length];
            for (var index = 0; index < nodes.Length; index += 1)
            {
                definitionNodes[index] = nodes[index].ToDefinition();
            }

            return new DialogueSequence(dialogueId, displayName, startingNodeId, definitionNodes);
        }

        [System.Serializable]
        public sealed class DialogueNodeRecord
        {
            public string Id = string.Empty;
            public string SpeakerName = string.Empty;
            public string Text = string.Empty;
            public string NextNodeId = string.Empty;

            public ConversationNode ToDefinition()
            {
                var nextId = string.IsNullOrWhiteSpace(NextNodeId) ? null : NextNodeId;
                return new ConversationNode(Id, SpeakerName, Text, nextId);
            }
        }
    }
}
