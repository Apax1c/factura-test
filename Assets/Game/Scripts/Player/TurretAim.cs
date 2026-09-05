using Factura.Configs;
using Factura.Core;
using Factura.Input;
using UnityEngine;
using VContainer;

namespace Factura.Player
{
    public sealed class TurretAim : MonoBehaviour, IResettable
    {
        private IInputService _input;
        private GameStateMachine _stateMachine;
        private WeaponConfig _config;

        private float _requestedYaw;
        private float _appliedYaw;
        private float _yawVelocity;

        public Quaternion AimRotation => transform.rotation;

        [Inject]
        public void Construct(IInputService input, GameStateMachine stateMachine, WeaponConfig config)
        {
            _input = input;
            _stateMachine = stateMachine;
            _config = config;
        }

        private void Update()
        {
            if (!_config) return;

            if (_stateMachine.IsPlaying && _input.IsPressed)
            {
                float degrees = _input.HorizontalDelta / Mathf.Max(1, Screen.width) * _config.DegreesPerScreenWidth;
                _requestedYaw = Mathf.Clamp(_requestedYaw + degrees, -_config.MaxYawAngle, _config.MaxYawAngle);
            }

            _appliedYaw = Mathf.SmoothDamp(_appliedYaw, _requestedYaw, ref _yawVelocity, _config.AimSmoothTime);
            transform.localRotation = Quaternion.Euler(0f, _appliedYaw, 0f);
        }

        public void ResetState()
        {
            _requestedYaw = 0f;
            _appliedYaw = 0f;
            _yawVelocity = 0f;
            transform.localRotation = Quaternion.identity;
        }
    }
}
