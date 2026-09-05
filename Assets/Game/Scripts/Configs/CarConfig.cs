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

        [Header("Lane sway")]
        [Tooltip("How far the car drifts to either side of its start line.")]
        [SerializeField, Min(0f)] private float _swayAmplitude = 1f;

        [Tooltip("Metres of road per full right-left-right cycle. Larger is lazier. " +
                 "Tied to distance rather than time so the drift reads as part of the road.")]
        [SerializeField, Min(1f)] private float _swayWavelength = 130f;

        [Header("Health")]
        [SerializeField, Min(1)] private int _maxHealth = 100;

        public float MaxSpeed => _maxSpeed;
        public float Acceleration => _acceleration;
        public float Braking => _braking;
        public float SwayAmplitude => _swayAmplitude;
        public float SwayWavelength => _swayWavelength;
        public int MaxHealth => _maxHealth;
    }
}
