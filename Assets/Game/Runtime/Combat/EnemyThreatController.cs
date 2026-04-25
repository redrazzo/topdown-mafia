using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Story;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Combat
{
    public sealed class EnemyThreatController : MonoBehaviour
    {
        [SerializeField] private CombatHealth? combatHealth;
        [SerializeField] private CombatHealth? targetHealth;
        [SerializeField] private TopDownPlayerController? targetPlayer;
        [SerializeField] private DialogueController? dialogueController;
        [SerializeField] private float moveSpeed = 2.8f;
        [SerializeField] private float aggroRange = 7f;
        [SerializeField] private float disengageRange = 12f;
        [SerializeField] private float attackRange = 1.35f;
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private float attackCooldown = 1.2f;

        private bool _isAlerted;
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
            if (combatHealth == null || !combatHealth.IsAlive || targetPlayer == null || targetHealth == null || !targetHealth.IsAlive)
            {
                return;
            }

            if (dialogueController != null && dialogueController.IsActive)
            {
                _isAlerted = false;
                return;
            }

            var playerPosition = targetPlayer.transform.position;
            var distance = Vector3.Distance(transform.position, playerPosition);

            if (!_isAlerted && distance <= aggroRange)
            {
                _isAlerted = true;
            }

            if (!_isAlerted)
            {
                return;
            }

            if (distance >= disengageRange)
            {
                _isAlerted = false;
                return;
            }

            var direction = (playerPosition - transform.position);
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
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
