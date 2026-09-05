using UnityEngine;

namespace Factura.Configs
{
    [CreateAssetMenu(menuName = "Factura/Level Config", fileName = "LevelConfig")]
    public sealed class LevelConfig : ScriptableObject
    {
        [Header("Layout")]
        [Tooltip("Distance the car travels from the start line to the finish line.")]
        [SerializeField, Min(10f)] private float _length = 300f;
        
        [Tooltip("Half-width of the drivable road, used for spawning and for clamping the car.")]
        [SerializeField, Min(1f)] private float _roadHalfWidth = 4.5f;

        [Header("Finish")]
        [Tooltip("How far past the last enemy the car keeps driving before the level counts as cleared.")]
        [SerializeField, Min(0f)] private float _finishRunOut = 15f;

        [Header("Enemies")]
        [SerializeField, Min(0)] private int _enemyCount = 28;

        [Tooltip("Nothing spawns closer to the start line than this, so the player is never ambushed at t=0.")]
        [SerializeField, Min(0f)] private float _spawnStartZ = 30f;

        [Tooltip("Minimum gap between two spawn points, to avoid enemies standing inside each other.")]
        [SerializeField, Min(0f)] private float _minSpawnSpacing = 5f;

        public float Length => _length;

        public int EnemyCount => _enemyCount;

        public float SpawnStartZ => _spawnStartZ;

        public float MinSpawnSpacing => _minSpawnSpacing;

        public float RoadHalfWidth => _roadHalfWidth;

        public float FinishRunOut => _finishRunOut;

        /// <summary>Total ground that must be covered by tiles, including the run-out and a margin behind the start.</summary>
        public float TiledLength => _length + _finishRunOut;
    }
}
