using UnityEngine;

namespace _Scripts.Game.Effects
{
    public class RandomEffectSelector
    {
        public EffectType GetRandomEffect()
        {
            var r = Random.value;

            return r switch
            {
                < 0.15f => EffectType.SpeedBoost,
                < 0.3f => EffectType.SlowDown,
                < 0.45f => EffectType.ExtraPoints,
                _ => EffectType.None
            };
        }
    }
}