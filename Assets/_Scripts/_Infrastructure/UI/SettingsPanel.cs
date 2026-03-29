using System;
using _Scripts._Infrastructure.Configs;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using _Scripts._Infrastructure.Services;

namespace _Scripts._Infrastructure.UI
{
    public class SettingsPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private Button _soundToggleButton;
        [SerializeField] private Button _musicToggleButton;
        [SerializeField] private Image _soundCheckmark;
        [SerializeField] private Image _musicCheckmark;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _closeButton;

        private AudioService _audioService;
        private UIPanelService _uiPanelService;

        private Vector3 _startScale;

        private float _defaultSoundVolume = 1f;
        private float _defaultMusicVolume = 0.3f;

        [Inject]
        public void Construct(AudioService audioService, UIPanelService uiPanelService)
        {
            _audioService = audioService;
            _uiPanelService = uiPanelService;

            _startScale = _rectTransform.localScale;
            
            _closeButton.onClick.AddListener(() => Close());

            UpdateUI();
        }

        private void Awake()
        {
            _soundToggleButton.onClick.AddListener(ToggleSound);
            _musicToggleButton.onClick.AddListener(ToggleMusic);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _rectTransform.localScale = Vector3.zero;
            _rectTransform.DOScale(_startScale, 0.1f).SetEase(Ease.Linear);
        }

        public void Close(Action onClosed = null)
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _rectTransform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActive(false);
                onClosed?.Invoke();
                _uiPanelService.OpenPanel<MenuPanel>();
            });
        }

        private void ToggleSound()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            
            bool isCurrentlyOn = _audioService.SoundVolume > 0f;
            _audioService.SoundVolume = isCurrentlyOn ? 0f : _defaultSoundVolume;
            _audioService.PlaySound(SoundType.ButtonClick);
            UpdateUI();
        }

        private void ToggleMusic()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            
            bool isCurrentlyOn = _audioService.MusicVolume > 0f;
            _audioService.MusicVolume = isCurrentlyOn ? 0f : _defaultMusicVolume;
            _audioService.PlaySound(SoundType.ButtonClick);
            UpdateUI();
        }


        private void UpdateUI()
        {
            _soundCheckmark.gameObject.SetActive(_audioService.SoundVolume > 0f);
            _musicCheckmark.gameObject.SetActive(_audioService.MusicVolume > 0f);
        }
    }
}
