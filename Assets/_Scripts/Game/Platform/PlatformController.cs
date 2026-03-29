using UnityEngine;
using Zenject;
using _Scripts.Game.Colors;
using _Scripts.Game.Effects;

namespace _Scripts.Game.Platform
{
    public class PlatformController : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private Rigidbody2D _rb;

        [Header("Effect Icon")]
        [SerializeField] private float _iconOffsetX = 0.15f;

        private GameColorService _colorService;
        private EffectIconService _iconService;
        private EffectIconView _currentIcon;

        private GameColorType _colorType;
        private EffectType _effectType;
        private float _platformWidth;

        public EffectType EffectType => _effectType;
        public GameColorType ColorType => _colorType;
        public Collider2D Collider => _collider;
        public float Height => _spriteRenderer.bounds.size.y;
        

        [Inject]
        public void Construct(GameColorService colorService, EffectIconService iconService)
        {
            _colorService = colorService;
            _iconService = iconService;
        }

        private void Awake()
        {
            _platformWidth = _spriteRenderer.bounds.size.x;
        }

        public void SetEffect(EffectType effect)
        {
            if (_effectType == effect) return;

            _effectType = effect;
            UpdateEffectIcon();
        }

        public void SetColor(GameColorType colorType)
        {
            _colorType = colorType;
            _spriteRenderer.color = _colorService.GetColor(colorType);
        }

        private void UpdateEffectIcon()
        {
            if (_currentIcon != null)
            {
                _iconService.DespawnIcon(_currentIcon);
                _currentIcon = null;
            }

            if (_effectType != EffectType.None)
            {
                _currentIcon = _iconService.SpawnIcon(
                    _effectType, transform, _platformWidth, _iconOffsetX
                );
            }
        }

        public void EnableCollider(bool enable) => _collider.enabled = enable;

        public class Pool : MonoMemoryPool<PlatformController>
        {
            protected override void Reinitialize(PlatformController item)
            {
                item.gameObject.SetActive(true);
                item.EnableCollider(true);
                item._spriteRenderer.enabled = true;
                item._spriteRenderer.color = Color.white;
                item._platformWidth = item._spriteRenderer.bounds.size.x;
            }

            protected override void OnDespawned(PlatformController item)
            {
                item.EnableCollider(false);
                item._spriteRenderer.color = Color.white;
                item._spriteRenderer.enabled = true;
                item.gameObject.SetActive(false);

                if (item._currentIcon != null)
                {
                    item._iconService.DespawnIcon(item._currentIcon);
                    item._currentIcon = null;
                }
            }
        }
    }
}
