using _Scripts.Game.Ball;

namespace _Scripts.Game.Effects
{
    public class NoEffect : IEffectStrategy
    {
        public EffectType Type => EffectType.None;

        public void Apply(BallController ball)
        {
            ball.ResetSpeedModifier();
        }
    }
}