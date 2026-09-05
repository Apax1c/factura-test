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
        [SerializeField] private Slider _healthBar;
        [SerializeField] private TMP_Text _healthLabel;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _distanceLabel;
        [SerializeField] private TMP_Text _killsLabel;

        private CarController _car;
        private LevelProgress _levelProgress;
        private ScoreService _score;
        private int _shownDistance = -1;

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
            if (_levelProgress == null)
                return;

            if (_progressBar)
                _progressBar.value = _levelProgress.Normalized;

            // Distance changes every frame but only ever needs redrawing when the whole metre
            // does, so the string is rebuilt at most once per metre instead of once per frame.
            int metres = Mathf.CeilToInt(_levelProgress.Remaining);
            if (metres == _shownDistance)
                return;

            _shownDistance = metres;

            if (_distanceLabel)
                _distanceLabel.text = metres + "m";
        }

        private void OnHealthChanged(float normalized)
        {
            if (_healthBar)
                _healthBar.value = normalized;

            if (_healthLabel && _car && _car.Health)
                _healthLabel.text = _car.Health.Current.ToString();
        }

        private void OnKillsChanged(int kills)
        {
            if (_killsLabel)
                _killsLabel.text = kills.ToString();
        }
    }
}
