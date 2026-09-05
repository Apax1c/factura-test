using System;
using Factura.Core;
using Factura.Events;
using UnityEngine;
using VContainer;

namespace Factura.CameraRig
{
    public sealed class CameraShake : MonoBehaviour, IResettable
    {
        [SerializeField, Min(0f)] private float _amplitude = 0.22f;
        [SerializeField, Min(0.05f)] private float _duration = 0.25f;
        [SerializeField, Min(1f)] private float _frequency = 24f;

        private IDisposable _subscription;
        private float _remaining;
        private float _seed;

        public Vector3 Offset { get; private set; }

        [Inject]
        public void Construct(IEventBus events) => _subscription = events.Subscribe<CarDamaged>(OnCarDamaged);

        private void OnDestroy() => _subscription?.Dispose();

        private void Update()
        {
            if (_remaining <= 0f)
            {
                Offset = Vector3.zero;
                return;
            }

            _remaining -= Time.deltaTime;

            // Fading the amplitude out rather than cutting it keeps the settle from reading as a
            // second, smaller jolt.
            float strength = Mathf.Clamp01(_remaining / _duration) * _amplitude;
            float time = Time.time * _frequency;

            Offset = new Vector3(
                (Mathf.PerlinNoise(_seed, time) - 0.5f) * 2f * strength,
                (Mathf.PerlinNoise(_seed + 11f, time) - 0.5f) * 2f * strength,
                0f
            );
        }

        public void ResetState()
        {
            _remaining = 0f;
            Offset = Vector3.zero;
        }

        private void OnCarDamaged(CarDamaged message)
        {
            _remaining = _duration;
            _seed = UnityEngine.Random.value * 100f;
        }
    }
}
