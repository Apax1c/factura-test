using System;
using Factura.Configs;
using UnityEngine;

namespace Factura.Combat
{
    /// <summary>
    /// A pooled bullet. Movement is resolved with a swept raycast from the previous position to
    /// the next one rather than with a collider: at this speed a collider-based bullet covers
    /// more than a body's width per frame and would step straight through its target.
    /// </summary>
    public sealed class Projectile : MonoBehaviour
    {
        private WeaponConfig _config;
        private Action<Projectile> _onFinished;
        private float _lifeRemaining;

        public event Action<Vector3> Hit;

        public void Launch(Vector3 position, Quaternion rotation, WeaponConfig config, Action<Projectile> onFinished)
        {
            transform.SetPositionAndRotation(position, rotation);
            _config = config;
            _onFinished = onFinished;
            _lifeRemaining = config.ProjectileLifetime;
        }

        private void Update()
        {
            if (!_config) return;

            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;
            float step = _config.ProjectileSpeed * Time.deltaTime;

            if (Physics.Raycast(origin, direction, out var hit, step, _config.HitMask, QueryTriggerInteraction.Ignore))
            {
                IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
                target?.TakeDamage(_config.Damage);
                Hit?.Invoke(hit.point);
                Finish();
                return;
            }

            transform.position = origin + direction * step;

            _lifeRemaining -= Time.deltaTime;
            if (_lifeRemaining <= 0f)
                Finish();
        }

        private void Finish()
        {
            Action<Projectile> callback = _onFinished;
            _onFinished = null;
            callback?.Invoke(this);
        }
    }
}
