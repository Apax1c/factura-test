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

        public float Length => _length;

        public float RoadHalfWidth => _roadHalfWidth;

        public float FinishRunOut => _finishRunOut;

        /// <summary>Total ground that must be covered by tiles, including the run-out and a margin behind the start.</summary>
        public float TiledLength => _length + _finishRunOut;
    }
}
