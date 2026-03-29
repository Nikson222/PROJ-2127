using _Scripts.Game.Ball;

namespace _Scripts.Game.Effects
{
    public class SpeedBoostEffect : IEffectStrategy
    {
        public EffectType Type => EffectType.SpeedBoost;

        public void Apply(BallController ball)
        {
            ball.ApplyTemporarySpeedModifier(1.5f, 2f);
        }
    }
}