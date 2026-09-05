using Factura.Core;
using Factura.Level;
using Factura.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Factura.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private Image _healthFill;
        [SerializeField] private Image _progressFill;
        [SerializeField] private TMP_Text _killsLabel;

        private CarController _car;
        private LevelProgress _levelProgress;
        private ScoreService _score;

        [Inject]
        public void Construct(CarController car, LevelProgress levelProgress, ScoreService score)
        {
            _car = car;
            _levelProgress = levelProgress;
            _score = score;
        }

        private void Start()
        {
            if (_score == null)
                return;

            _score.KillsChanged += OnKillsChanged;
            OnKillsChanged(_score.Kills);

            if (_car && _car.Health)
            {
                _car.Health.NormalizedChanged += OnHealthChanged;
                OnHealthChanged(_car.Health.Normalized);
            }
        }

        private void OnDestroy()
        {
            if (_score != null)
                _score.KillsChanged -= OnKillsChanged;

            if (_car && _car.Health)
                _car.Health.NormalizedChanged -= OnHealthChanged;
        }

        private void Update()
        {
            if (_progressFill && _levelProgress != null)
                _progressFill.fillAmount = _levelProgress.Normalized;
        }

        private void OnHealthChanged(float normalized)
        {
            if (_healthFill)
                _healthFill.fillAmount = normalized;
        }

        private void OnKillsChanged(int kills)
        {
            if (_killsLabel)
                _killsLabel.text = kills.ToString();
        }
    }
}
