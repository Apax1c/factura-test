using Factura.Combat;
using Factura.Configs;
using Factura.Core;
using UnityEngine;
using VContainer;

namespace Factura.Player
{
    public sealed class TurretShooter : MonoBehaviour, IResettable
    {
        [Tooltip("Bullets spawn here and inherit this transform's forward direction.")]
        [SerializeField] private Transform _muzzle;

        private ProjectileService _projectiles;
        private GameStateMachine _stateMachine;
        private WeaponConfig _config;
        private float _cooldown;

        [Inject]
        public void Construct(ProjectileService projectiles, GameStateMachine stateMachine, WeaponConfig config)
        {
            _projectiles = projectiles;
            _stateMachine = stateMachine;
            _config = config;
        }

        private void Update()
        {
            if (!_config || !_stateMachine.IsPlaying) return;

            _cooldown -= Time.deltaTime;
            if (_cooldown > 0f)
                return;

            _cooldown = _config.FireInterval;
            _projectiles.Fire(_muzzle.position, _muzzle.rotation);
        }

        public void ResetState() => _cooldown = 0f;
    }
}
