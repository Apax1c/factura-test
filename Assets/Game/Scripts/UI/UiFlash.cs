using UnityEngine;
using UnityEngine.UI;

namespace Factura.UI
{
    /// <summary>Briefly tints a graphic and fades back, to mark a hit landing.</summary>
    public sealed class UiFlash : MonoBehaviour
    {
        [SerializeField] private Graphic _target;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField, Min(0.01f)] private float _duration = 0.18f;

        private Color _baseColor = Color.white;
        private float _elapsed = -1f;

        private void Awake()
        {
            if (_target)
                _baseColor = _target.color;
        }

        private void OnDisable()
        {
            _elapsed = -1f;

            if (_target)
                _target.color = _baseColor;
        }

        public void Play() => _elapsed = 0f;

        private void Update()
        {
            if (_elapsed < 0f || !_target)
                return;

            _elapsed += Time.unscaledDeltaTime;
            float progress = _elapsed / _duration;

            if (progress >= 1f)
            {
                _elapsed = -1f;
                _target.color = _baseColor;
                return;
            }

            _target.color = Color.Lerp(_flashColor, _baseColor, progress);
        }
    }
}
