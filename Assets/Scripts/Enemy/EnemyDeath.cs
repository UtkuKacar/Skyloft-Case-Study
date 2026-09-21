using System;
using UnityEngine;

namespace Skyloft.Enemy
{
    /// <summary>One retirement path; replace deactivation with pool return when pooling is added.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyHealth), typeof(EnemyMovement))]
    public sealed class EnemyDeath : MonoBehaviour
    {
        private EnemyHealth health;
        private EnemyMovement movement;
        private CharacterController characterController;
        private EnemyAttack attack;
        public event Action<EnemyDeath> Retired;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
            movement = GetComponent<EnemyMovement>();
            characterController = GetComponent<CharacterController>();
            attack = GetComponent<EnemyAttack>();
        }

        private void OnEnable()
        {
            health.Died += Retire;
            if (health.IsDead)
            {
                Retire();
            }
        }

        private void OnDisable()
        {
            health.Died -= Retire;
        }

        private void Retire()
        {
            movement.enabled = false;
            if (attack != null)
                attack.enabled = false;
            characterController.enabled = false;
            gameObject.SetActive(false);
            Retired?.Invoke(this);
        }
    }
}
