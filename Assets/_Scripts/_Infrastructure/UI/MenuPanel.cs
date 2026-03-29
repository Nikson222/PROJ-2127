using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.Configs;

namespace _Scripts._Infrastructure.UI
{
    public class MenuPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _setttingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _gameCenterButton;
    
        private AudioService _audioController;
        private UIPanelService _uiPanelService;
        private SceneLoader _sceneLoader;
    
        private Vector3 _startScale;
    
        [Inject]
        public void Construct(SceneLoader sceneLoader, PlayerProfile playerProfile, AudioService audioController, UIPanelService uiPanelService)
        {
            _sceneLoader = sceneLoader;
            _audioController = audioController;
            _uiPanelService = uiPanelService;
            
            _startScale = transform.localScale;
            _playButton.onClick.AddListener(OnClickPlay);
            _setttingsButton.onClick.AddListener(OnClickSettings);
            _exitButton.onClick.AddListener(OnClickExit);
        }
    
        private void Start()
        {
            _audioController.PlayMusic();
        }
    
        public void Open()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            _rectTransform.DOScale(_startScale, 0.1f).SetEase(Ease.Linear);
        }
    
        public void Close(Action onClosed = null)
        {
            _rectTransform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                onClosed?.Invoke();
                gameObject.SetActive(false);
            });
        }
    
        private void OnClickPlay()
        {
            _audioController.StopMusic();
            _audioController.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load("LevelScene", () => _audioController.PlayMusic());
        }
    
        private void OnClickSettings()
        {
            _audioController.PlaySound(SoundType.ButtonClick);
            Close(() => _uiPanelService.OpenPanel<SettingsPanel>());
        }
    
        private void OnClickExit()
        {
            Application.Quit();
        }
    }
}
