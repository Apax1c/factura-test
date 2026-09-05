using Factura.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Factura.Enemies
{
    public sealed class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private GameObject _bar;
        [SerializeField] private Image _fill;

        [Header("Damage trail")]
        [Tooltip("Sits behind the fill and lags it, so the size of the last hit stays readable " +
                 "for a moment after the bar itself has already dropped.")]
        [SerializeField] private Image _chipFill;

        [SerializeField, Min(0f)] private float _chipHoldSeconds = 0.22f;
        [SerializeField, Min(0.01f)] private float _chipDrainSeconds = 0.35f;

        [Header("Hit jolt")]
        [SerializeField, Min(0f)] private float _shakeAmplitude = 0.045f;
        [SerializeField, Min(0.01f)] private float _shakeDuration = 0.16f;
        [SerializeField, Min(1f)] private float _shakeFrequency = 40f;

        private Transform _cameraTransform;
        private Vector3 _baseLocalPosition;
        private float _chipHoldRemaining;
        private float _shakeRemaining;

        private void Awake()
        {
            if (Camera.main)
                _cameraTransform = Camera.main.transform;

            if (_bar)
                _baseLocalPosition = _bar.transform.localPosition;
        }

        private void OnEnable()
        {
            Hide();

            _chipHoldRemaining = 0f;
            _shakeRemaining = 0f;

            if (_chipFill)
                _chipFill.fillAmount = 1f;

            if (!_health)
                return;

            _health.Damaged += OnDamaged;
            _health.NormalizedChanged += OnNormalizedChanged;
            _health.Died += Hide;
        }

        private void OnDisable()
        {
            if (!_health)
                return;

            _health.Damaged -= OnDamaged;
            _health.NormalizedChanged -= OnNormalizedChanged;
            _health.Died -= Hide;
        }

        private void LateUpdate()
        {
            if (!_bar || !_bar.activeSelf)
                return;

            TickChip();
            PlaceBar();
        }

        private void TickChip()
        {
            if (!_chipFill || !_fill || _chipFill.fillAmount <= _fill.fillAmount)
                return;

            if (_chipHoldRemaining > 0f)
            {
                _chipHoldRemaining -= Time.deltaTime;
                return;
            }

            _chipFill.fillAmount = Mathf.MoveTowards(
                _chipFill.fillAmount,
                _fill.fillAmount,
                Time.deltaTime / _chipDrainSeconds);
        }

        private void PlaceBar()
        {
            if (!_cameraTransform)
                return;

            Vector3 position = transform.TransformPoint(_baseLocalPosition);

            if (_shakeRemaining > 0f)
            {
                _shakeRemaining -= Time.deltaTime;

                float strength = Mathf.Clamp01(_shakeRemaining / _shakeDuration) * _shakeAmplitude;
                float wave = Time.time * _shakeFrequency;
                position += _cameraTransform.right * (Mathf.Sin(wave) * strength)
                            + _cameraTransform.up * (Mathf.Cos(wave * 1.7f) * strength);
            }

            _bar.transform.position = position;

            _bar.transform.rotation = _cameraTransform.rotation;
        }

        private void OnDamaged(int amount)
        {
            if (_bar)
                _bar.SetActive(true);

            _shakeRemaining = _shakeDuration;
        }

        private void OnNormalizedChanged(float normalized)
        {
            if (_fill)
                _fill.fillAmount = normalized;

            if (!_chipFill)
                return;

            if (_chipFill.fillAmount < normalized)
                _chipFill.fillAmount = normalized;
            else
                _chipHoldRemaining = _chipHoldSeconds;
        }

        private void Hide()
        {
            if (_bar)
                _bar.SetActive(false);
        }
    }
}
