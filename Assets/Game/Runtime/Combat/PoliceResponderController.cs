using MafiaTopDown.Gameplay.Runtime.Player;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Combat
{
    public sealed class PoliceResponderController : MonoBehaviour
    {
        [SerializeField] private CombatHealth combatHealth;
        [SerializeField] private CombatHealth targetHealth;
        [SerializeField] private TopDownPlayerController targetPlayer;
        [SerializeField] private float moveSpeed = 3.6f;
        [SerializeField] private float aggroRange = 18f;
        [SerializeField] private float attackRange = 1.35f;
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private float attackCooldown = 0.9f;

        private float _nextAttackAt;

        private void Reset()
        {
            combatHealth = GetComponent<CombatHealth>();
        }

        private void Start()
        {
            if (targetPlayer == null)
            {
                targetPlayer = FindAnyObjectByType<TopDownPlayerController>();
            }

            if (targetHealth == null && targetPlayer != null)
            {
                targetHealth = targetPlayer.GetComponent<CombatHealth>();
            }
        }

        private void Update()
        {
            if (!gameObject.activeInHierarchy || combatHealth == null || !combatHealth.IsAlive || targetPlayer == null || targetHealth == null || !targetHealth.IsAlive)
            {
                return;
            }

            var direction = targetPlayer.transform.position - transform.position;
            direction.y = 0f;
            var distance = direction.magnitude;
            if (distance > aggroRange)
            {
                return;
            }

            if (direction.sqrMagnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 9f * Time.deltaTime);
            }

            if (distance > attackRange)
            {
                transform.position += direction.normalized * (moveSpeed * Time.deltaTime);
                return;
            }

            if (Time.time < _nextAttackAt)
            {
                return;
            }

            _nextAttackAt = Time.time + attackCooldown;
            targetHealth.ApplyDamage(contactDamage, combatHealth.DisplayName);
        }
    }
}
