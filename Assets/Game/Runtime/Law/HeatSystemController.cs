using MafiaTopDown.Gameplay.Domain.Law;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Law
{
    public sealed class HeatSystemController : MonoBehaviour
    {
        [SerializeField] private SaveGameFileService? saveGameFileService;
        [SerializeField] private float cooldownDelay = 7f;
        [SerializeField] private float cooldownTickSeconds = 1.5f;
        [SerializeField] private int cooldownAmount = 5;

        private HeatState _currentState = HeatState.Cold;
        private float _nextCooldownAt;

        public int CurrentIntensity => _currentState.Intensity;

        public PoliceResponseTier CurrentTier => _currentState.ResponseTier;

        private void Awake()
        {
            if (saveGameFileService == null)
            {
                return;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            _currentState = HeatService.FromIntensity(save.HeatLevel);
        }

        private void Update()
        {
            if (_currentState.Intensity <= 0)
            {
                return;
            }

            if (Time.time < _nextCooldownAt)
            {
                return;
            }

            var cooled = HeatService.CoolDown(_currentState, cooldownAmount);
            ApplyState(cooled, false);
            _nextCooldownAt = Time.time + cooldownTickSeconds;
        }

        public void AddViolation(int severity)
        {
            var updated = HeatService.AddViolation(_currentState, severity);
            ApplyState(updated, true);
        }

        private void ApplyState(HeatState state, bool delayCooldown)
        {
            _currentState = state;
            if (saveGameFileService != null)
            {
                saveGameFileService.UpdateHeatLevel(_currentState.Intensity);
            }

            if (delayCooldown)
            {
                _nextCooldownAt = Time.time + cooldownDelay;
            }
        }
    }
}
