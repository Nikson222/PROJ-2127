using UnityEngine;

namespace _Scripts.Game.Effects
{
    public class EffectIconService
    {
        private readonly EffectIconView.Pool _pool;
        private readonly EffectIconConfig _config;

        public EffectIconService(EffectIconView.Pool pool, EffectIconConfig config)
        {
            _pool = pool;
            _config = config;
        }

        public EffectIconView SpawnIcon(EffectType effectType, Transform platform, float platformWidth, float offset)
        {
            var sprite = _config.GetIcon(effectType);
            if (sprite == null) return null;

            var icon = _pool.Spawn();
            icon.transform.SetParent(null);
            icon.transform.localScale = Vector3.one;
            icon.transform.rotation = Quaternion.identity;

            icon.SetSprite(sprite);

            // Теперь после SetSprite можно получить SpriteRenderer
            SpriteRenderer spriteRenderer = icon.GetComponent<SpriteRenderer>();
            float iconWidth = spriteRenderer != null ? spriteRenderer.bounds.size.x : 0.5f; // Безопасная проверка

            float xOffset = platformWidth / 2f + iconWidth / 2f + offset; // !!! ВАЖНО: /2
            bool isRight = Random.value > 0.5f;

            Vector3 worldOffset = platform.right * (isRight ? xOffset : -xOffset);
            Vector3 worldPos = platform.position + worldOffset;

            icon.transform.position = worldPos;
            icon.transform.SetParent(platform, worldPositionStays: true);

            return icon;
        }


        public void DespawnIcon(EffectIconView icon)
        {
            if (icon != null)
            {
                icon.transform.SetParent(null);
                icon.transform.localScale = Vector3.one;
                icon.transform.rotation = Quaternion.identity;
                icon.gameObject.SetActive(false);
                _pool.Despawn(icon);
            }
        }

    }
}