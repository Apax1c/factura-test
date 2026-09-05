using Factura.Configs;
using UnityEngine;
using VContainer;

namespace Factura.Level
{
    /// <summary>
    /// Lays copies of the ground model end to end to cover the whole level.
    /// Tile length is measured from the model's own renderers rather than typed into the
    /// inspector, so swapping the model or rescaling it needs no change here.
    /// </summary>
    public sealed class GroundTiler : MonoBehaviour
    {
        [SerializeField] private GameObject _groundTilePrefab;

        [Tooltip("Extra ground kept behind the start line so the camera never sees past the edge.")]
        [SerializeField, Min(0f)] private float _trailingLength = 25f;

        private LevelConfig _config;

        [Inject]
        public void Construct(LevelConfig config) => _config = config;

        private void Start() => Build();

        private void Build()
        {
            if (_groundTilePrefab == null)
            {
                Debug.LogError($"{nameof(GroundTiler)}: ground tile prefab is not assigned.", this);
                return;
            }

            GameObject first = Instantiate(_groundTilePrefab, transform);
            float tileLength = MeasureLengthAlongZ(first, out var pivotToCentreZ);

            if (tileLength <= Mathf.Epsilon)
            {
                Debug.LogError($"{nameof(GroundTiler)}: '{_groundTilePrefab.name}' has no renderers to measure.", this);
                Destroy(first);
                return;
            }

            float totalLength = _config.TiledLength + _trailingLength;
            int tileCount = Mathf.CeilToInt(totalLength / tileLength) + 1;
            float startZ = -_trailingLength;

            PlaceTile(first, 0, startZ, tileLength, pivotToCentreZ);
            for (int i = 1; i < tileCount; i++)
                PlaceTile(Instantiate(_groundTilePrefab, transform), i, startZ, tileLength, pivotToCentreZ);
        }

        private static void PlaceTile(GameObject tile, int index, float startZ, float tileLength, float pivotToCentreZ)
        {
            var z = startZ + (index + 0.5f) * tileLength - pivotToCentreZ;
            tile.transform.localPosition = new Vector3(0f, 0f, z);
            tile.name = $"Ground_{index:D2}";
        }

        /// <summary>
        /// Returns the tile's extent along Z and how far its bounds centre sits from its pivot,
        /// so tiles can be butted together regardless of where the model's origin is.
        /// </summary>
        private static float MeasureLengthAlongZ(GameObject instance, out float pivotToCentreZ)
        {
            pivotToCentreZ = 0f;

            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return 0f;

            instance.transform.localPosition = Vector3.zero;
            
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            pivotToCentreZ = bounds.center.z - instance.transform.position.z;
            return bounds.size.z;
        }
    }
}
