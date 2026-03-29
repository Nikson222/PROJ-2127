using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.MyEditorCustoms;
using _Scripts._Infrastructure.UI;
using _Scripts.Game.Services;

namespace _Scripts._Infrastructure.UI
{
    public class PauseMenuPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private RectTransform _pauseMenuRectTransform;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _menuButton;
        [Scene] [SerializeField] private string _menuSceneName;
        [Scene] [SerializeField] private string _gameSceneName;

        private SceneLoader _sceneLoader;
        private AudioService _audioService;
        private ScoreCounter _scoreCounter;
        private Vector3 _startScale;

        [Inject]
        private void Construct(SceneLoader sceneLoader, AudioService audioService,
            ScoreCounter scoreCounter)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _scoreCounter = scoreCounter;
            
            _startScale = transform.localScale;
        }

        private void Awake()
        {
            _restartButton.onClick.AddListener(OnClickRestart);
            _continueButton.onClick.AddListener(OnClickContinue);
            _menuButton.onClick.AddListener(OnClickMenu);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _pauseMenuRectTransform.localScale = Vector3.zero;
            _pauseMenuRectTransform.DOScale(_startScale, 0.1f)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    Time.timeScale = 0f;
                });
        }

        public void Close(Action onClosed = null)
        {
            Time.timeScale = 1f;
            _pauseMenuRectTransform.DOScale(Vector3.zero, 0.1f)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    onClosed?.Invoke();
                });
        }


        private void OnClickRestart()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            Time.timeScale = 1f;
            _sceneLoader.Load(_gameSceneName, () =>
            {
                _audioService.PlayMusic();
                _scoreCounter.Reset();
            });
            _audioService.StopMusic();
        }

        private void OnClickContinue()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            Close();
        }

        private void OnClickMenu()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            Time.timeScale = 1f;
            _sceneLoader.Load(_menuSceneName, () =>
            {
                _audioService.PlayMusic();
                _scoreCounter.Reset();
            });
            _audioService.StopMusic();
        }
    }
}
