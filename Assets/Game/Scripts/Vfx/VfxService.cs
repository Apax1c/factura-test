using System;
using System.Collections.Generic;
using Factura.Configs;
using Factura.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Factura.Vfx
{
    public sealed class VfxService : IDisposable
    {
        private readonly List<IDisposable> _subscriptions = new();
        private readonly Transform _root;
        private readonly EffectRing _hits;
        private readonly EffectRing _deaths;

        public VfxService(VfxConfig config, IEventBus events)
        {
            _root = new GameObject("[Vfx]").transform;

            _hits = EffectRing.Create(config.HitEffect, _root, config.InstancesPerEffect);
            _deaths = EffectRing.Create(config.DeathEffect, _root, config.InstancesPerEffect);

            _subscriptions.Add(events.Subscribe<ProjectileHit>(message => _hits?.Play(message.Position)));
            _subscriptions.Add(events.Subscribe<EnemyKilled>(message => _deaths?.Play(message.Position)));
        }

        public void Dispose()
        {
            foreach (IDisposable subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();

            if (_root)
                Object.Destroy(_root.gameObject);
        }

        /// <summary>
        /// A fixed ring of instances rather than a pool: a one-shot burst has no moment where it
        /// reports being finished, and cycling through enough copies costs less than tracking
        /// lifetimes for something that lasts a few tenths of a second.
        /// </summary>
        private sealed class EffectRing
        {
            private readonly ParticleSystem[] _instances;
            private int _next;

            private EffectRing(ParticleSystem[] instances) => _instances = instances;

            public static EffectRing Create(ParticleSystem prefab, Transform parent, int count)
            {
                if (!prefab)
                {
                    Debug.LogWarning($"{nameof(VfxConfig)} is missing an effect prefab.");
                    return null;
                }

                ParticleSystem[] instances = new ParticleSystem[Mathf.Max(1, count)];
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i] = Object.Instantiate(prefab, parent);
                    instances[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }

                return new EffectRing(instances);
            }

            public void Play(Vector3 position)
            {
                ParticleSystem instance = _instances[_next];
                _next = (_next + 1) % _instances.Length;

                instance.transform.position = position;
                instance.Clear();
                instance.Play();
            }
        }
    }
}
