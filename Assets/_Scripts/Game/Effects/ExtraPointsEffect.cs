using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using _Scripts.Game.Ball;
using UnityEngine;

namespace _Scripts.Game.Effects
{
    public class ExtraPointsEffect : IEffectStrategy
    {
        private AudioService _audioService;

        public ExtraPointsEffect(AudioService audioService)
        {
            _audioService = audioService;
        }

        public EffectType Type => EffectType.ExtraPoints;
        
        public void Apply(BallController ball)
        {
            ball.ResetSpeedModifier();
            _audioService.PlaySound(SoundType.Bonus);
        }
    }
}