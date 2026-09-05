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

        private Transform _target;
        private Vector3 _followVelocity;

        [Inject]
        public void Construct(CarController car) => _target = car.transform;

        private void LateUpdate()
        {
            if (!_target) return;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                _target.position + _offset,
                ref _followVelocity,
                _followSmoothTime
            );

            AimAtTarget();
        }

        public void ResetState()
        {
            if (!_target) return;

            _followVelocity = Vector3.zero;
            transform.position = _target.position + _offset;
            AimAtTarget();
        }

        private void AimAtTarget() =>
            transform.rotation = Quaternion.LookRotation(_target.position + _lookAtOffset - transform.position);
    }
}
