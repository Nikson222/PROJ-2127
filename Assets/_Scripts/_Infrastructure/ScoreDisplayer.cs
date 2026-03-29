using System;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI;
using _Scripts.Game.Services;
using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game.States;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game.UI
{
    public class ScoreDisplayer : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private RectTransform bonusPopupPosition;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _bonusColor = Color.yellow;
        [SerializeField] private float _bonusEffectDuration = 0.5f;
        [SerializeField] private Button _pauseButton;

        private int _lastScore = 0;
        private ScoreCounter _scoreCounter;
        private PopupTextService _popupTextService;
        private UIPanelService _uiPanelService;
        private GameStateMachine _gameStateMachine;

        [Inject]
        private void Construct(
            ScoreCounter scoreCounter,
            PopupTextService popupTextService,
            UIPanelService uiPanelService,
            GameStateMachine gameStateMachine)
        {
            _scoreCounter = scoreCounter;
            _popupTextService = popupTextService;
            _uiPanelService = uiPanelService;
            _gameStateMachine = gameStateMachine;

            _scoreCounter.OnScoreChanged += OnScoreChanged;
            _pauseButton.onClick.AddListener(OnPauseClick);
        }

        private void Start()
        {
            OnScoreChanged(_scoreCounter.Score, false);
        }

        private void OnScoreChanged(int score, bool isBonus)
        {
            if (isBonus)
            {
                ShowBonusPopup(score - _lastScore);
            }

            _lastScore = score;
            _text.text = score.ToString();
        }

        private void ShowBonusPopup(int score)
        {
            var popupText = "+" + score.ToString();
            _popupTextService.ShowPopupAs(popupText, bonusPopupPosition, _bonusColor, 0.3f, 0.3f);
        }

        private void OnPauseClick()
        {
            if (_gameStateMachine.IsInState<GameOverState>())
                return;

            _uiPanelService.OpenPanel<PauseMenuPanel>();
        }

        private void OnDestroy()
        {
            _scoreCounter.OnScoreChanged -= OnScoreChanged;
        }
    }
}
