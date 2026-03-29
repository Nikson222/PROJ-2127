using UnityEngine;

namespace _Scripts.Game.Effects
{
    public class EffectIconView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _iconRoot;
        [SerializeField] private SpriteRenderer _spriteRenderer;   
        
        [Header("Settings")]
        [SerializeField] private float _targetSize = 0.3f;          

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            ResizeToFit(sprite);
        }

        private void ResizeToFit(Sprite sprite)
        {
            if (sprite == null) return;

            var bounds = sprite.bounds.size;
            float maxSize = Mathf.Max(bounds.x, bounds.y);

            if (maxSize == 0f)
            {
                _iconRoot.localScale = Vector3.one;
                return;
            }

            float scaleFactor = _targetSize / maxSize;
            _iconRoot.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }

        public class Pool : Zenject.MonoMemoryPool<EffectIconView>
        {
            protected override void OnDespawned(EffectIconView item)
            {
                item.gameObject.SetActive(false);
            }

            protected override void OnSpawned(EffectIconView item)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
}