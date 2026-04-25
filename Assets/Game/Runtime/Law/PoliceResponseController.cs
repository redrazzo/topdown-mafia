using MafiaTopDown.Gameplay.Domain.Law;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Law
{
    public sealed class PoliceResponseController : MonoBehaviour
    {
        [SerializeField] private HeatSystemController heatSystemController;
        [SerializeField] private GameObject[] watchResponders = new GameObject[0];
        [SerializeField] private GameObject[] patrolResponders = new GameObject[0];
        [SerializeField] private GameObject[] huntResponders = new GameObject[0];

        private PoliceResponsePlan _currentPlan = new PoliceResponsePlan(PoliceResponseTier.None, 0, "Clear");

        public string CurrentStatusText => _currentPlan.StatusText;

        public int CurrentResponderCount => _currentPlan.ResponderCount;

        private void Awake()
        {
            ApplyPlan();
        }

        private void Update()
        {
            ApplyPlan();
        }

        private void ApplyPlan()
        {
            var heatState = heatSystemController == null
                ? HeatState.Cold
                : HeatService.FromIntensity(heatSystemController.CurrentIntensity);
            var nextPlan = PoliceResponsePlanner.Plan(heatState);

            if (nextPlan.Tier == _currentPlan.Tier &&
                nextPlan.ResponderCount == _currentPlan.ResponderCount)
            {
                return;
            }

            _currentPlan = nextPlan;
            ApplyGroup(watchResponders, nextPlan.Tier == PoliceResponseTier.Watch ? nextPlan.ResponderCount : 0);
            ApplyGroup(patrolResponders, nextPlan.Tier == PoliceResponseTier.Patrol ? nextPlan.ResponderCount : 0);
            ApplyGroup(huntResponders, nextPlan.Tier == PoliceResponseTier.Hunt ? nextPlan.ResponderCount : 0);
        }

        private static void ApplyGroup(GameObject[] responders, int activeCount)
        {
            for (var index = 0; index < responders.Length; index += 1)
            {
                if (responders[index] == null)
                {
                    continue;
                }

                responders[index].SetActive(index < activeCount);
            }
        }
    }
}
