using _Scripts.Game.Ball;

namespace _Scripts.Game.Effects
{
    public interface IEffectStrategy
    {
        EffectType Type { get; }
        void Apply(BallController ball);
    }
}