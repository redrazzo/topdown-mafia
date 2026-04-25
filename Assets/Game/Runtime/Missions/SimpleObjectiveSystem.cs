using System.Linq;
using MafiaTopDown.Gameplay.Domain.Missions;
using UnityEngine;
using UnityEngine.Events;

namespace MafiaTopDown.Gameplay.Runtime.Missions
{
    public sealed class SimpleObjectiveSystem : MonoBehaviour
    {
        [SerializeField] private ObjectiveStep[] steps = new ObjectiveStep[0];
        [SerializeField] private string startingObjectiveId = string.Empty;
        [SerializeField] private UnityEvent<string> onObjectiveChanged = new();
        [SerializeField] private UnityEvent onMissionCompleted = new();

        private ObjectiveTracker _tracker;

        public string CurrentObjectiveId => _tracker == null ? null : _tracker.CurrentObjectiveId;

        public string CurrentObjectiveTitle
        {
            get
            {
                if (_tracker == null || _tracker.CurrentObjective == null)
                {
                    return string.Empty;
                }

                return _tracker.CurrentObjective.Title;
            }
        }

        private void Awake()
        {
            RebuildTracker();
        }

        public void CompleteCurrentObjective()
        {
            if (_tracker == null || _tracker.IsComplete)
            {
                return;
            }

            _tracker.MarkCurrentObjectiveComplete();

            if (_tracker.IsComplete)
            {
                onMissionCompleted.Invoke();
                return;
            }

            onObjectiveChanged.Invoke(CurrentObjectiveTitle);
        }

        public void Configure(ObjectiveStep[] configuredSteps, string configuredStartingObjectiveId)
        {
            steps = configuredSteps ?? new ObjectiveStep[0];
            startingObjectiveId = configuredStartingObjectiveId ?? string.Empty;
            RebuildTracker();
        }

        public void ClearObjectives()
        {
            steps = new ObjectiveStep[0];
            startingObjectiveId = string.Empty;
            _tracker = null;
            onObjectiveChanged.Invoke(string.Empty);
        }

        private void RebuildTracker()
        {
            _tracker = null;
            if (steps.Length == 0)
            {
                onObjectiveChanged.Invoke(string.Empty);
                return;
            }

            var firstId = string.IsNullOrWhiteSpace(startingObjectiveId) ? steps[0].Id : startingObjectiveId;
            var definitions = steps.Select(step => step.ToDefinition());
            _tracker = new ObjectiveTracker(definitions, firstId);
            onObjectiveChanged.Invoke(CurrentObjectiveTitle);
        }

        [System.Serializable]
        public sealed class ObjectiveStep
        {
            public string Id = string.Empty;
            public string Title = string.Empty;
            public string SuccessCondition = string.Empty;
            public string NextObjectiveId = string.Empty;

            public ObjectiveDefinition ToDefinition()
            {
                var nextId = string.IsNullOrWhiteSpace(NextObjectiveId) ? null : NextObjectiveId;
                return new ObjectiveDefinition(Id, Title, SuccessCondition, nextId);
            }
        }
    }
}
