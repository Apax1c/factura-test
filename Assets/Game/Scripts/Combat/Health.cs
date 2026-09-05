using System;
using UnityEngine;

namespace Factura.Combat
{
    /// <summary>Shared by the car and the enemies, so damage sources stay ignorant of their target.</summary>
    public sealed class Health : MonoBehaviour, IDamageable
    {
        private int _max;
        private int _current;

        public event Action<float> NormalizedChanged;
        public event Action Died;

        public bool IsAlive => _current > 0;

        public int Current => _current;

        public int Max => _max;
        public float Normalized => _max > 0 ? (float)_current / _max : 0f;

        public void Initialize(int max)
        {
            _max = Mathf.Max(1, max);
            _current = _max;
            NormalizedChanged?.Invoke(Normalized);
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive || amount <= 0)
                return;

            _current = Mathf.Max(0, _current - amount);
            NormalizedChanged?.Invoke(Normalized);

            if (_current == 0)
                Died?.Invoke();
        }
    }
}
