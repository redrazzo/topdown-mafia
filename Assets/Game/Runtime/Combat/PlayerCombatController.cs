using MafiaTopDown.Gameplay.Runtime.Law;
using MafiaTopDown.Gameplay.Runtime.Player;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Combat
{
    public sealed class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private TopDownPlayerController? playerController;
        [SerializeField] private CombatHealth? combatHealth;
        [SerializeField] private HeatSystemController? heatSystemController;
        [SerializeField] private KeyCode attackKey = KeyCode.Space;
        [SerializeField] private float attackRange = 2.8f;
        [SerializeField] private int attackDamage = 1;
        [SerializeField] private int heatPerAttack = 15;
        [SerializeField] private float attackCooldown = 0.35f;

        private float _nextAttackAt;

        private void Reset()
        {
            playerController = GetComponent<TopDownPlayerController>();
            combatHealth = GetComponent<CombatHealth>();
        }

        private void Update()
        {
            if (playerController == null || combatHealth == null || !combatHealth.IsAlive || playerController.ControlsLocked)
            {
                return;
            }

            if (!Input.GetKeyDown(attackKey) && !Input.GetMouseButtonDown(0))
            {
                return;
            }

            if (Time.time < _nextAttackAt)
            {
                return;
            }

            _nextAttackAt = Time.time + attackCooldown;
            var target = FindTarget();
            if (target == null)
            {
                return;
            }

            target.ApplyDamage(attackDamage, combatHealth.DisplayName);
            heatSystemController?.AddViolation(heatPerAttack);
        }

        private CombatHealth? FindTarget()
        {
            var hits = Physics.OverlapSphere(transform.position, attackRange);
            CombatHealth? closestTarget = null;
            var closestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                var enemy = hit.GetComponentInParent<EnemyThreatController>();
                if (enemy == null)
                {
                    continue;
                }

                var targetHealth = enemy.GetComponent<CombatHealth>();
                if (targetHealth == null || !targetHealth.IsAlive)
                {
                    continue;
                }

                var distance = Vector3.Distance(transform.position, targetHealth.transform.position);
                if (distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = distance;
                closestTarget = targetHealth;
            }

            return closestTarget;
        }
    }
}
