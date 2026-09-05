using Factura.Configs;
using Factura.Core;
using UnityEngine;
using VContainer;

namespace Factura.Player
{
    /// <summary>
    /// Draws where the barrel is pointing. Without it the player is steering an angle they cannot
    /// see until a bullet has already travelled, which is what makes free aiming feel vague.
    /// The line stops at the first enemy in the way, so it doubles as a hit confirmation.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public sealed class AimLine : MonoBehaviour, IResettable
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField, Min(1f)] private float _maxLength = 80f;

        private GameStateMachine _stateMachine;
        private WeaponConfig _config;
        private LineRenderer _line;

        [Inject]
        public void Construct(GameStateMachine stateMachine, WeaponConfig config)
        {
            _stateMachine = stateMachine;
            _config = config;
        }

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = 2;
            _line.useWorldSpace = true;
            _line.enabled = false;
        }

        // LateUpdate so the turret has already been rotated for this frame; drawing in Update
        // would trail the barrel by one frame at exactly the moment the player is swiping.
        private void LateUpdate()
        {
            bool visible = _stateMachine != null && _stateMachine.IsPlaying && _muzzle;
            _line.enabled = visible;

            if (!visible)
                return;

            Vector3 origin = _muzzle.position;
            Vector3 direction = _muzzle.forward;
            float length = _maxLength;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, _maxLength, _config.HitMask, QueryTriggerInteraction.Ignore))
                length = hit.distance;

            _line.SetPosition(0, origin);
            _line.SetPosition(1, origin + direction * length);
        }

        public void ResetState()
        {
            if (_line)
                _line.enabled = false;
        }
    }
}
