using System.Collections.Generic;
using Factura.Configs;
using Factura.Core;
using Factura.Events;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Factura.Combat
{
    public sealed class ProjectileService : IResettable, System.IDisposable
    {
        private readonly WeaponConfig _config;
        private readonly IEventBus _events;
        private readonly ObjectPool<Projectile> _pool;
        private readonly List<Projectile> _active = new();
        private readonly Transform _root;

        public ProjectileService(WeaponConfig config, IEventBus events)
        {
            _config = config;
            _events = events;
            _root = new GameObject("[Projectiles]").transform;

            _pool = new ObjectPool<Projectile>(
                CreateProjectile,
                projectile => projectile.gameObject.SetActive(true),
                projectile => projectile.gameObject.SetActive(false),
                projectile =>
                {
                    if (projectile)
                        Object.Destroy(projectile.gameObject);
                },
                collectionCheck: false,
                defaultCapacity: config.PoolCapacity
            );

            Prewarm();
        }

        /// <summary>
        /// Instantiating on the first burst of fire is exactly when a hitch is most noticeable,
        /// so the whole pool is built up front while the player is still on the start screen.
        /// </summary>
        private void Prewarm()
        {
            if (_config.ProjectilePrefab == null)
                return;

            Projectile[] warmed = new Projectile[_config.PoolCapacity];
            for (int i = 0; i < warmed.Length; i++)
                warmed[i] = _pool.Get();

            foreach (Projectile projectile in warmed)
                _pool.Release(projectile);
        }

        public void Fire(Vector3 position, Quaternion rotation)
        {
            if (!_config.ProjectilePrefab)
            {
                Debug.LogError($"{nameof(WeaponConfig)} has no projectile prefab assigned.");
                return;
            }

            Projectile projectile = _pool.Get();
            _active.Add(projectile);
            projectile.Launch(position, rotation, _config, Release);
        }

        public void ResetState()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                _pool.Release(_active[i]);

            _active.Clear();
        }

        public void Dispose()
        {
            _pool.Dispose();

            if (_root)
                Object.Destroy(_root.gameObject);
        }


        private Projectile CreateProjectile()
        {
            Projectile projectile = Object.Instantiate(_config.ProjectilePrefab, _root);
            projectile.Hit += point => _events.Publish(new ProjectileHit(point));
            return projectile;
        }

        private void Release(Projectile projectile)
        {
            // A bullet that hit something is released from its own Update; guard against a
            // second release when the round is reset on the same frame.
            if (!_active.Remove(projectile))
                return;

            _pool.Release(projectile);
        }
    }
}
