using System.Collections.Generic;
using Factura.Configs;
using Factura.Core;
using Factura.Player;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Factura.Enemies
{
    public sealed class EnemySpawner : MonoBehaviour, IResettable
    {
        private const int MAX_PLACEMENT_ATTEMPTS = 30;

        [SerializeField] private EnemyAgent _enemyPrefab;

        private readonly List<EnemyAgent> _active = new();
        private readonly List<Vector3> _placed = new();

        private LevelConfig _levelConfig;
        private EnemyConfig _enemyConfig;
        private CarController _car;
        private ObjectPool<EnemyAgent> _pool;

        [Inject]
        public void Construct(LevelConfig levelConfig, EnemyConfig enemyConfig, CarController car)
        {
            _levelConfig = levelConfig;
            _enemyConfig = enemyConfig;
            _car = car;

            _pool = new ObjectPool<EnemyAgent>(
                CreateEnemy,
                enemy => enemy.gameObject.SetActive(true),
                enemy => enemy.gameObject.SetActive(false),
                enemy =>
                {
                    if (enemy)
                        Destroy(enemy.gameObject);
                },
                collectionCheck: false,
                defaultCapacity: 32
            );
        }

        public void ResetState()
        {
            ReleaseAll();
            SpawnWave();
        }

        private void SpawnWave()
        {
            if (!_enemyPrefab)
            {
                Debug.LogError($"{nameof(EnemySpawner)}: enemy prefab is not assigned.", this);
                return;
            }

            _placed.Clear();

            for (int i = 0; i < _levelConfig.EnemyCount; i++)
            {
                if (!TryFindSpawnPoint(out Vector3 point))
                    continue;

                _placed.Add(point);

                EnemyAgent enemy = _pool.Get();
                _active.Add(enemy);
                enemy.Spawn(point, _enemyConfig, _car.transform, _car.Health, Release);
            }
        }

        private bool TryFindSpawnPoint(out Vector3 point)
        {
            float halfWidth = _levelConfig.RoadHalfWidth;
            float minSpacingSqr = _levelConfig.MinSpawnSpacing * _levelConfig.MinSpawnSpacing;

            // Nothing spawns inside the run-out, so the last stretch to the finish line is clear
            // and the level ends on a breath rather than mid-fight.
            float lastSpawnZ = Mathf.Max(
                _levelConfig.SpawnStartZ,
                _levelConfig.Length - _levelConfig.FinishRunOut);

            for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS; attempt++)
            {
                point = new Vector3(
                    Random.Range(-halfWidth, halfWidth),
                    0f,
                    Random.Range(_levelConfig.SpawnStartZ, lastSpawnZ));

                if (IsFarEnoughFromPlaced(point, minSpacingSqr))
                    return true;
            }

            point = default;
            return false;
        }

        private bool IsFarEnoughFromPlaced(Vector3 candidate, float minSpacingSqr)
        {
            for (int i = 0; i < _placed.Count; i++)
            {
                if ((_placed[i] - candidate).sqrMagnitude < minSpacingSqr)
                    return false;
            }

            return true;
        }

        private EnemyAgent CreateEnemy() => Instantiate(_enemyPrefab, transform);

        private void Release(EnemyAgent enemy)
        {
            if (!_active.Remove(enemy))
                return;

            _pool.Release(enemy);
        }

        private void ReleaseAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                _pool.Release(_active[i]);

            _active.Clear();
        }

        private void OnDrawGizmos()
        {
            if (!_levelConfig)
                return;

            float length = _levelConfig.Length - _levelConfig.SpawnStartZ;
            if (length <= 0f)
                return;

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
            Gizmos.DrawCube(
                new Vector3(0f, 0.05f, _levelConfig.SpawnStartZ + length * 0.5f),
                new Vector3(_levelConfig.RoadHalfWidth * 2f, 0.1f, length));
        }
    }
}
