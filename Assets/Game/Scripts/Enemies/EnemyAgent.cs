using System;
using Factura.Combat;
using Factura.Configs;
using UnityEngine;

namespace Factura.Enemies
{
    public enum EnemyState { Idle, Chase, Attack, Dead }

    public sealed class EnemyAgent : MonoBehaviour
    {
        /// <summary>Slack on the range recheck, so a blow is not lost to a few centimetres.</summary>
        private const float ATTACK_REACH_TOLERANCE = 1.25f;

        [SerializeField] private Health _health;
        [SerializeField] private Collider _bodyCollider;

        private EnemyConfig _config;
        private Transform _target;
        private IDamageable _targetHealth;
        private Action<EnemyAgent> _onFinished;
        private EnemyState _state;
        private float _attackCooldown;
        private float _deathTimer;

        public EnemyState State => _state;

        public event Action Attacked;

        public event Action Killed;

        private void OnEnable()
        {
            if (_health) _health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (_health) _health.Died -= OnDied;
        }

        public void Spawn(
            Vector3 position,
            EnemyConfig config,
            Transform target,
            IDamageable targetHealth,
            Action<EnemyAgent> onFinished)
        {
            _config = config;
            _target = target;
            _targetHealth = targetHealth;
            _onFinished = onFinished;

            _state = EnemyState.Idle;
            _attackCooldown = 0f;
            _deathTimer = 0f;

            transform.SetPositionAndRotation(position, Quaternion.identity);

            if (_bodyCollider)
                _bodyCollider.enabled = true;

            _health.Initialize(config.MaxHealth);
        }

        private void Update()
        {
            if (!_config || !_target)
                return;

            // A corpse is on its linger timer and is recycled by that, not by the distance cull.
            if (_state != EnemyState.Dead && HasBeenLeftBehind())
            {
                Finish();
                return;
            }

            switch (_state)
            {
                case EnemyState.Idle:
                    TickIdle();
                    break;
                case EnemyState.Chase:
                    TickChase();
                    break;
                case EnemyState.Attack:
                    TickAttack();
                    break;
                case EnemyState.Dead:
                    TickDead();
                    break;
            }
        }

        private void TickIdle()
        {
            if (DistanceToTarget() <= _config.AggroRadius)
                _state = EnemyState.Chase;
        }

        private void TickChase()
        {
            if (DistanceToTarget() <= _config.AttackRange)
            {
                _state = EnemyState.Attack;
                return;
            }

            Vector3 direction = FlatDirectionToTarget();
            transform.position += direction * (_config.MoveSpeed * Time.deltaTime);
            FaceDirection(direction);
        }

        private void TickAttack()
        {
            // The car keeps moving, so an attacker that loses contact drops back to chasing.
            if (DistanceToTarget() > _config.AttackRange)
            {
                _state = EnemyState.Chase;
                return;
            }

            FaceDirection(FlatDirectionToTarget());

            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown > 0f)
                return;

            // Only the swing starts here. The blow itself lands when the animation says so,
            // which is what DeliverAttack is for.
            _attackCooldown = _config.AttackInterval;
            Attacked?.Invoke();
        }

        public void DeliverAttack()
        {
            if (_state != EnemyState.Attack || !_target)
                return;

            if (DistanceToTarget() > _config.AttackRange * ATTACK_REACH_TOLERANCE)
                return;

            _targetHealth?.TakeDamage(_config.AttackDamage);
        }

        private void TickDead()
        {
            _deathTimer -= Time.deltaTime;
            if (_deathTimer <= 0f)
                Finish();
        }

        private void OnDied()
        {
            _state = EnemyState.Dead;
            _deathTimer = _config.CorpseLingerSeconds;

            if (_bodyCollider)
                _bodyCollider.enabled = false;

            Killed?.Invoke();
        }

        private float DistanceToTarget()
        {
            Vector3 delta = _target.position - transform.position;
            delta.y = 0f;
            return delta.magnitude;
        }

        private Vector3 FlatDirectionToTarget()
        {
            Vector3 delta = _target.position - transform.position;
            delta.y = 0f;
            return delta.sqrMagnitude > Mathf.Epsilon ? delta.normalized : transform.forward;
        }

        private void FaceDirection(Vector3 direction)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                _config.TurnSpeed * Time.deltaTime);
        }

        private bool HasBeenLeftBehind() =>
            _target.position.z - transform.position.z > _config.DespawnBehindDistance;

        private void Finish()
        {
            Action<EnemyAgent> callback = _onFinished;
            _onFinished = null;
            callback?.Invoke(this);
        }
    }
}
