using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.MyEditorCustoms;
using _Scripts.Game.Services;

namespace _Scripts._Infrastructure.UI
{
    public class GameOverPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private RectTransform _gameOverRectTransform;
        [Scene] [SerializeField] private string _menuSceneName;
        [Scene] [SerializeField] private string _gameSceneName;

        private SceneLoader _sceneLoader;
        private AudioService _audioService;
        private ScoreCounter _scoreCounter;
        private PlayerProfile _playerProfile;
        private Vector3 _startScale;

        [Inject]
        private void Construct(SceneLoader sceneLoader, AudioService audioService,
            ScoreCounter scoreCounter, PlayerProfile playerProfile)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _startScale = transform.localScale;
            _playerProfile = playerProfile;
            
            _scoreCounter = scoreCounter;
            
            _restartButton.onClick.AddListener(OnClickRestart);
            _menuButton.onClick.AddListener(OnClickMenu);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            _gameOverRectTransform.DOScale(_startScale, 0.1f).SetEase(Ease.Linear);
        }

        public void Close(Action onClosed = null)
        {
            gameObject.SetActive(false);
            onClosed?.Invoke();
        }

        private void OnClickRestart()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load(_gameSceneName, () =>
            {
                _audioService.PlayMusic();
                _playerProfile.SetMaxScore(_scoreCounter.Score);
                _scoreCounter.Reset();
            });
            _audioService.StopMusic();
        }

        private void OnClickMenu()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load(_menuSceneName, () =>
            {
                _audioService.PlayMusic();
                _playerProfile.SetMaxScore(_scoreCounter.Score);
                _scoreCounter.Reset();
            });
            _audioService.StopMusic();
        }
    }
}