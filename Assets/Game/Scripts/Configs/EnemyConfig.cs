using UnityEngine;

namespace Factura.Configs
{
    [CreateAssetMenu(menuName = "Factura/Enemy Config", fileName = "EnemyConfig")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [Header("Health")]
        [SerializeField, Min(1)] private int _maxHealth = 30;

        [Header("Senses")]
        [Tooltip("Distance at which an idle enemy notices the car and charges.")]
        [SerializeField, Min(1f)] private float _aggroRadius = 24f;

        [Tooltip("Once the car is this far ahead, the enemy can never catch up and is recycled.")]
        [SerializeField, Min(5f)] private float _despawnBehindDistance = 35f;

        [Header("Movement")]
        [SerializeField, Min(0.1f)] private float _moveSpeed = 5.5f;
        [SerializeField, Min(1f)] private float _turnSpeed = 14f;

        [Header("Attack")]
        [SerializeField, Min(0.5f)] private float _attackRange = 2.8f;
        [SerializeField, Min(0.1f)] private float _attackInterval = 0.9f;
        [SerializeField, Min(1)] private int _attackDamage = 7;

        [Header("Death")]
        [Tooltip("How long the body lingers before returning to the pool.")]
        [SerializeField, Min(0f)] private float _corpseLingerSeconds = 1.1f;

        public int MaxHealth => _maxHealth;
        public float AggroRadius => _aggroRadius;
        public float DespawnBehindDistance => _despawnBehindDistance;
        public float MoveSpeed => _moveSpeed;
        public float TurnSpeed => _turnSpeed;
        public float AttackRange => _attackRange;
        public float AttackInterval => _attackInterval;
        public int AttackDamage => _attackDamage;
        public float CorpseLingerSeconds => _corpseLingerSeconds;
    }
}
