using Factura.Core;
using TMPro;
using UnityEngine;
using VContainer;

namespace Factura.UI
{
    public sealed class GameResultView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _hint;

        private GameStateMachine _stateMachine;

        [Inject]
        public void Construct(GameStateMachine stateMachine) => _stateMachine = stateMachine;

        private void Start()
        {
            if (_stateMachine == null)
                return;

            _stateMachine.StateChanged += OnStateChanged;
            OnStateChanged(_stateMachine.Current);
        }

        private void OnDestroy()
        {
            if (_stateMachine != null)
                _stateMachine.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Ready:
                    Show("Tap to start", string.Empty);
                    break;
                case GameState.Playing:
                    Hide();
                    break;
                case GameState.Win:
                    Show("You win", "Tap to restart");
                    break;
                case GameState.Lose:
                    Show("You lose", "Tap to restart");
                    break;
            }
        }

        private void Show(string title, string hint)
        {
            if (_panel)
                _panel.SetActive(true);

            if (_title)
                _title.text = title;

            if (_hint)
                _hint.text = hint;
        }

        private void Hide()
        {
            if (_panel)
                _panel.SetActive(false);
        }
    }
}
