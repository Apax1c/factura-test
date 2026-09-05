using UnityEngine;

namespace Factura.UI
{
    /// <summary>
    /// A single scale kick. Used where a number changes: without it a counter incrementing is
    /// easy to miss, because nothing on screen moved to draw the eye to it.
    /// </summary>
    public sealed class UiPunch : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float _duration = 0.2f;
        [SerializeField, Min(0f)] private float _strength = 0.3f;

        private Vector3 _baseScale = Vector3.one;
        private float _elapsed = -1f;

        private void Awake() => _baseScale = transform.localScale;

        private void OnDisable()
        {
            _elapsed = -1f;
            transform.localScale = _baseScale;
        }

        public void Play() => _elapsed = 0f;

        private void Update()
        {
            if (_elapsed < 0f)
                return;

            _elapsed += Time.unscaledDeltaTime;
            float progress = _elapsed / _duration;

            if (progress >= 1f)
            {
                _elapsed = -1f;
                transform.localScale = _baseScale;
                return;
            }

            float punch = Mathf.Sin(progress * Mathf.PI) * (1f - progress) * _strength;
            transform.localScale = _baseScale * (1f + punch);
        }
    }
}
