using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Game.Effects
{
    public class EffectStrategyFactory
    {
        private readonly Dictionary<EffectType, IEffectStrategy> _strategies;

        public EffectStrategyFactory(IEnumerable<IEffectStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(
                strategy => strategy.Type,
                strategy => strategy
            );
        }

        public IEffectStrategy GetStrategy(EffectType type)
        {
            if (_strategies.TryGetValue(type, out var strategy))
                return strategy;

            throw new KeyNotFoundException($"No strategy found for EffectType: {type}");
        }
    }
}