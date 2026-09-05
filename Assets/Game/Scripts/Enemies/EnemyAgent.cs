using System;
using Factura.Combat;
using Factura.Configs;
using UnityEngine;

namespace Factura.Enemies
{
    public sealed class EnemyAgent : MonoBehaviour
    {
        private enum State { Idle, Chase, Attack, Dead }

        [SerializeField] private Health _health;
        [SerializeField] private Collider _bodyCollider;

        private EnemyConfig _config;
        private Transform _target;
        private IDamageable _targetHealth;
        private Action<EnemyAgent> _onFinished;
        private State _state;
        private float _attackCooldown;
        private float _deathTimer;

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

            _state = State.Idle;
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
            if (_state != State.Dead && HasBeenLeftBehind())
            {
                Finish();
                return;
            }

            switch (_state)
            {
                case State.Idle:
                    TickIdle();
                    break;
                case State.Chase:
                    TickChase();
                    break;
                case State.Attack:
                    TickAttack();
                    break;
                case State.Dead:
                    TickDead();
                    break;
            }
        }

        private void TickIdle()
        {
            if (DistanceToTarget() <= _config.AggroRadius)
                _state = State.Chase;
        }

        private void TickChase()
        {
            if (DistanceToTarget() <= _config.AttackRange)
            {
                _state = State.Attack;
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
                _state = State.Chase;
                return;
            }

            FaceDirection(FlatDirectionToTarget());

            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown > 0f)
                return;

            _attackCooldown = _config.AttackInterval;
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
            _state = State.Dead;
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
