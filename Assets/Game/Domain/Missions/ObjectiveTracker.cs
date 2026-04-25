using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MafiaTopDown.Gameplay.Domain.Missions
{
    public sealed class ObjectiveTracker
    {
        private readonly Dictionary<string, ObjectiveDefinition> _objectivesById;
        private readonly List<string> _completedObjectiveIds = new();

        public ObjectiveTracker(IEnumerable<ObjectiveDefinition> objectives, string firstObjectiveId)
        {
            _objectivesById = objectives.ToDictionary(objective => objective.Id);

            if (_objectivesById.Count == 0)
            {
                throw new ArgumentException("At least one objective is required.", nameof(objectives));
            }

            if (!_objectivesById.ContainsKey(firstObjectiveId))
            {
                throw new ArgumentException($"Unknown first objective '{firstObjectiveId}'.", nameof(firstObjectiveId));
            }

            CurrentObjectiveId = firstObjectiveId;
        }

        public string CurrentObjectiveId { get; private set; }

        public bool IsComplete => CurrentObjectiveId is null;

        public ReadOnlyCollection<string> CompletedObjectiveIds => _completedObjectiveIds.AsReadOnly();

        public ObjectiveDefinition CurrentObjective
        {
            get
            {
                if (CurrentObjectiveId is null)
                {
                    return null;
                }

                return _objectivesById[CurrentObjectiveId];
            }
        }

        public void MarkCurrentObjectiveComplete()
        {
            if (CurrentObjectiveId is null)
            {
                return;
            }

            var completedObjective = _objectivesById[CurrentObjectiveId];
            _completedObjectiveIds.Add(completedObjective.Id);
            CurrentObjectiveId = completedObjective.NextObjectiveId;
        }
    }
}
