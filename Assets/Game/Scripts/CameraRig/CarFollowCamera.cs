using Factura.Core;
using Factura.Player;
using UnityEngine;
using VContainer;

namespace Factura.CameraRig
{
    /// <summary>
    /// Trails the car from behind and above. The car never turns, so a world-space offset is
    /// both correct and the cheapest thing that stays perfectly stable at speed.
    /// The damping is what sells the acceleration: the camera lags slightly on the launch
    /// and settles back as the car reaches cruising speed.
    /// </summary>
    public sealed class CarFollowCamera : MonoBehaviour, IResettable
    {
        [SerializeField] private Vector3 _offset = new(0f, 7.5f, -9f);

        [Tooltip("Point the camera aims at, relative to the car. Pushing it forward shows more road.")]
        [SerializeField] private Vector3 _lookAtOffset = new(0f, 1.5f, 8f);

        [SerializeField, Min(0f)] private float _followSmoothTime = 0.18f;

        [SerializeField] private CameraShake _shake;

        private Transform _target;
        private Vector3 _followVelocity;

        private Vector3 _basePosition;

        [Inject]
        public void Construct(CarController car) => _target = car.transform;

        private void LateUpdate()
        {
            if (!_target) return;

            _basePosition = Vector3.SmoothDamp(
                _basePosition,
                _target.position + _offset,
                ref _followVelocity,
                _followSmoothTime
            );

            Apply();
        }

        public void ResetState()
        {
            if (!_target) return;

            _followVelocity = Vector3.zero;
            _basePosition = _target.position + _offset;
            Apply();
        }

        private void Apply()
        {
            transform.position = _basePosition + (_shake ? _shake.Offset : Vector3.zero);
            transform.rotation = Quaternion.LookRotation(_target.position + _lookAtOffset - transform.position);
        }
    }
}
