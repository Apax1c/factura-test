using Factura.Combat;
using UnityEngine;

namespace Factura.Configs
{
    /// <summary>Turret handling and projectile behaviour.</summary>
    [CreateAssetMenu(menuName = "Factura/Weapon Config", fileName = "WeaponConfig")]
    public sealed class WeaponConfig : ScriptableObject
    {
        [Header("Aiming")]
        [SerializeField, Range(10f, 89f)] private float _maxYawAngle = 60f;

        [Tooltip("Degrees the turret turns when the finger crosses the full width of the screen. " +
                 "Expressed this way so the control feels identical on any resolution or DPI.")]
        [SerializeField, Min(1f)] private float _degreesPerScreenWidth = 150f;

        [Tooltip("Smoothing on the turret angle. Small values feel responsive, larger ones feel heavy.")]
        [SerializeField, Min(0f)] private float _aimSmoothTime = 0.06f;

        [Header("Firing")]
        [SerializeField, Min(0.02f)] private float _fireInterval = 0.14f;
        [SerializeField, Min(1)] private int _damage = 10;

        [Header("Projectile")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField, Min(1f)] private float _projectileSpeed = 70f;
        [SerializeField, Min(0.1f)] private float _projectileLifetime = 2.5f;
        [SerializeField] private LayerMask _hitMask;
        [SerializeField, Min(1)] private int _poolCapacity = 32;

        public float MaxYawAngle => _maxYawAngle;
        public float DegreesPerScreenWidth => _degreesPerScreenWidth;
        public float AimSmoothTime => _aimSmoothTime;

        public float FireInterval => _fireInterval;
        public int Damage => _damage;

        public Projectile ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public LayerMask HitMask => _hitMask;
        public int PoolCapacity => _poolCapacity;
    }
}
