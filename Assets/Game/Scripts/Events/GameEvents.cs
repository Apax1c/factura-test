using UnityEngine;

namespace Factura.Events
{
    public readonly struct EnemyKilled
    {
        public readonly Vector3 Position;

        public EnemyKilled(Vector3 position) => Position = position;
    }

    public readonly struct ProjectileHit
    {
        public readonly Vector3 Position;

        public ProjectileHit(Vector3 position) => Position = position;
    }

    public readonly struct CarDamaged
    {
        public readonly int Amount;

        public CarDamaged(int amount) => Amount = amount;
    }
}
