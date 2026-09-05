using Factura.Configs;
using Factura.Core;
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
        private CarConfig _config;
        private GameStateMachine _stateMachine;
        private Rigidbody _rigidbody;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private float _speed;

        public float DistanceTravelled { get; private set; }

        public float NormalizedSpeed => _config == null ? 0f : _speed / _config.MaxSpeed;

        [Inject]
        public void Construct(CarConfig config, GameStateMachine stateMachine)
        {
            _config = config;
            _stateMachine = stateMachine;
        }

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
        }
    }
}
