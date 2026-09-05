using Factura.Configs;
using Factura.Player;
using UnityEngine;

namespace Factura.Level
{
    public sealed class LevelProgress
    {
        private readonly CarController _car;
        private readonly LevelConfig _config;

        public LevelProgress(CarController car, LevelConfig config)
        {
            _car = car;
            _config = config;
        }

        /// <summary>Progress along the level in the 0..1 range.</summary>
        public float Normalized => Mathf.Clamp01(_car.DistanceTravelled / _config.Length);

        public bool IsFinished => _car.DistanceTravelled >= _config.Length;
    }
}
