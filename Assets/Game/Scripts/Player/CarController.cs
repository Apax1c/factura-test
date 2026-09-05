using Factura.Combat;
using Factura.Configs;
using Factura.Core;
using Factura.Events;
using UnityEngine;
using VContainer;

namespace Factura.Player
{
    /// <summary>
    /// Drives the car forward along +Z. Speed is eased in and out rather than switched on and off.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class CarController : MonoBehaviour, IResettable
    {
        [SerializeField] private Health _health;

        private CarConfig _config;
        private GameStateMachine _stateMachine;
        private IEventBus _events;
        private Rigidbody _rigidbody;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private float _speed;

        public float DistanceTravelled { get; private set; }

        public float NormalizedSpeed => _config == null ? 0f : _speed / _config.MaxSpeed;

        public Health Health => _health;

        public bool IsAlive => !_health || _health.IsAlive;

        [Inject]
        public void Construct(CarConfig config, GameStateMachine stateMachine, IEventBus events)
        {
            _config = config;
            _stateMachine = stateMachine;
            _events = events;

            // Health is a generic component shared with the enemies, so the owner is the one that
            // gives its damage a meaning the rest of the game can react to.
            if (_health)
                _health.Damaged += OnDamaged;
        }

        private void OnDestroy()
        {
            if (_health)
                _health.Damaged -= OnDamaged;
        }

        private void OnDamaged(int amount) => _events.Publish(new CarDamaged(amount));

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            Transform t = transform;
            _startPosition = t.position;
            _startRotation = t.rotation;
        }

        private void FixedUpdate()
        {
            if (!_config)
                return;

            bool driving = _stateMachine.IsPlaying;
            float targetSpeed = driving ? _config.MaxSpeed : 0f;
            float rate = driving ? _config.Acceleration : _config.Braking;
            _speed = Mathf.MoveTowards(_speed, targetSpeed, rate * Time.fixedDeltaTime);

            if (Mathf.Approximately(_speed, 0f))
                return;

            float step = _speed * Time.fixedDeltaTime;
            DistanceTravelled += step;
            _rigidbody.MovePosition(_rigidbody.position + transform.forward * step);
        }

        public void ResetState()
        {
            _speed = 0f;
            DistanceTravelled = 0f;
            transform.SetPositionAndRotation(_startPosition, _startRotation);

            if (_health)
                _health.Initialize(_config.MaxHealth);
        }
    }
}
