using UnityEngine;

namespace Factura.Configs
{
    [CreateAssetMenu(menuName = "Factura/Vfx Config", fileName = "VfxConfig")]
    public sealed class VfxConfig : ScriptableObject
    {
        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private ParticleSystem _deathEffect;

        [Tooltip("Instances kept per effect. Sized so a burst never has to interrupt a live one.")]
        [SerializeField, Min(1)] private int _instancesPerEffect = 10;

        public ParticleSystem HitEffect => _hitEffect;
        public ParticleSystem DeathEffect => _deathEffect;
        public int InstancesPerEffect => _instancesPerEffect;
    }
}
