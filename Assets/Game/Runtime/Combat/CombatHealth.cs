using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.Combat
{
    public sealed class CombatHealth : MonoBehaviour
    {
        [SerializeField] private string displayName = "Combatant";
        [SerializeField] private int maximumHealth = 3;
        [SerializeField] private bool destroyOnDeath = true;
        [SerializeField] private bool reloadSceneOnDeath;

        private int _currentHealth;

        public string DisplayName => displayName;

        public int MaximumHealth => maximumHealth;

        public int CurrentHealth => _currentHealth;

        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            _currentHealth = maximumHealth;
        }

        public bool ApplyDamage(int amount, string source)
        {
            if (amount <= 0 || !IsAlive)
            {
                return false;
            }

            _currentHealth = Mathf.Max(0, _currentHealth - amount);
            Debug.Log(displayName + " took " + amount + " damage from " + source + ". Remaining health: " + _currentHealth);

            if (_currentHealth > 0)
            {
                return false;
            }

            if (reloadSceneOnDeath)
            {
                Debug.Log(displayName + " went down. Returning to the latest checkpoint.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return true;
            }

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }

            return true;
        }
    }
}
