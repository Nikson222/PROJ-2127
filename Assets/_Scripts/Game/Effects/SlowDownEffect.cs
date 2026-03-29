using _Scripts.Game.Ball;

namespace _Scripts.Game.Effects
{
    public class SlowDownEffect : IEffectStrategy
    {
        public EffectType Type => EffectType.SlowDown;

        public void Apply(BallController ball)
        {
            ball.ApplyTemporaryGravityModifier(0.3f, 2.5f);
            ball.ApplyTemporaryJumpModifier(0.6f, 2.5f);
        }
    }
}