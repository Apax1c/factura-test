using UnityEngine;

namespace Factura.Configs
{
    [CreateAssetMenu(menuName = "Factura/Car Config", fileName = "CarConfig")]
    public sealed class CarConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField, Min(1f)] private float _maxSpeed = 11f;
        
        [Tooltip("Units per second squared while spinning up to max speed.")]
        [SerializeField, Min(0.1f)] private float _acceleration = 7f;
        
        [Tooltip("Units per second squared while coming to a stop on win or loss.")]
        [SerializeField, Min(0.1f)] private float _braking = 14f;

        [Header("Health")]
        [SerializeField, Min(1)] private int _maxHealth = 100;

        public float MaxSpeed => _maxSpeed;
        public float Acceleration => _acceleration;
        public float Braking => _braking;
        public int MaxHealth => _maxHealth;
    }
}
