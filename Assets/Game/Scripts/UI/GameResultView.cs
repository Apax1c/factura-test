using Factura.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Factura.UI
{
    public sealed class GameResultView : MonoBehaviour
    {
        [SerializeField] private GameObject _startPrompt;
        [SerializeField] private GameObject _resultPopup;
        [SerializeField] private TMP_Text _resultTitle;
        [SerializeField] private TMP_Text _resultHint;
        [SerializeField] private Image _resultIcon;
        [SerializeField] private Sprite _winIcon;
        [SerializeField] private Sprite _loseIcon;

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
                    ShowStartPrompt();
                    break;
                case GameState.Playing:
                    HideAll();
                    break;
                case GameState.Win:
                    ShowResult("You win", _winIcon);
                    break;
                case GameState.Lose:
                    ShowResult("You lose", _loseIcon);
                    break;
            }
        }

        private void ShowStartPrompt()
        {
            SetActive(_startPrompt, true);
            SetActive(_resultPopup, false);
        }

        private void HideAll()
        {
            SetActive(_startPrompt, false);
            SetActive(_resultPopup, false);
        }

        private void ShowResult(string title, Sprite icon)
        {
            SetActive(_startPrompt, false);
            SetActive(_resultPopup, true);

            if (_resultTitle)
                _resultTitle.text = title;

            if (_resultHint)
                _resultHint.text = "Tap to restart";

            if (_resultIcon && icon)
                _resultIcon.sprite = icon;
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target)
                target.SetActive(active);
        }
    }
}
