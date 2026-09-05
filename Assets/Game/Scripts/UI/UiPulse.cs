using UnityEngine;

namespace Factura.UI
{
    /// <summary>
    /// Gentle breathing scale. Used on the tap prompt: a static line of text reads as a label,
    /// a moving one reads as something waiting for the player to act.
    /// </summary>
    public sealed class UiPulse : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _amplitude = 0.06f;
        [SerializeField, Min(0.1f)] private float _cyclesPerSecond = 0.9f;

        private Vector3 _baseScale = Vector3.one;

        private void Awake() => _baseScale = transform.localScale;

        private void OnDisable() => transform.localScale = _baseScale;

        private void Update()
        {
            float wave = Mathf.Sin(Time.unscaledTime * _cyclesPerSecond * Mathf.PI * 2f);
            transform.localScale = _baseScale * (1f + wave * _amplitude);
        }
    }
}
