using System;
using MafiaTopDown.Gameplay.Domain.Story;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.UI;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Story
{
    public sealed class DialogueController : MonoBehaviour
    {
        [SerializeField] private TopDownPlayerController? playerController;
        [SerializeField] private KeyCode advanceKey = KeyCode.Return;
        [SerializeField] private Rect dialogueRect = new Rect(16f, 420f, 760f, 110f);

        private DialogueSequenceProgressor? _progressor;
        private Action? _onComplete;

        public bool IsActive => _progressor != null && !_progressor.IsComplete;

        public void StartSequence(DialogueSequenceAsset sequenceAsset, Action onComplete)
        {
            if (sequenceAsset == null || IsActive)
            {
                return;
            }

            _progressor = new DialogueSequenceProgressor(sequenceAsset.ToDefinition());
            _onComplete = onComplete;
            playerController?.SetControlsLocked(true);
        }

        private void Update()
        {
            var progressor = _progressor;
            if (progressor == null || progressor.IsComplete)
            {
                return;
            }

            if (!Input.GetKeyDown(advanceKey) && !Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }

            progressor.Advance();
            if (!progressor.IsComplete)
            {
                return;
            }

            playerController?.SetControlsLocked(false);
            var callback = _onComplete;
            _progressor = null;
            _onComplete = null;
            callback?.Invoke();
        }

        private void OnGUI()
        {
            if (!IsActive)
            {
                return;
            }

            var node = _progressor?.CurrentNode;
            if (node == null)
            {
                return;
            }

            HudStyleUtility.DrawPanel(
                dialogueRect,
                node.SpeakerName,
                node.Text + "\n[Enter] Continue");
        }
    }
}
