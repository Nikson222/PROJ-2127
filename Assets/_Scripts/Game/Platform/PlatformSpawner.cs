using System;
using System.Collections.Generic;
using _Scripts.Game.Colors;
using _Scripts.Game.Effects;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Scripts.Game.Platform
{
    public class PlatformSpawner
    {
        private readonly PlatformController.Pool _pool;
        private readonly Transform _parent;
        private readonly RandomEffectSelector _effectSelector;

        private readonly List<GameColorType> _recentColors = new();
        private const int MaxRecentColors = 2;

        public PlatformSpawner(
            PlatformController.Pool pool,
            [Inject(Id = "PlatformParent")] Transform parent,
            RandomEffectSelector effectSelector)
        {
            _pool = pool;
            _parent = parent;
            _effectSelector = effectSelector;
        }

        public PlatformController Spawn(Vector3 localPosition, GameColorType? preferredColor = null)
        {
            var platform = _pool.Spawn();
            platform.transform.SetParent(_parent, false);
            platform.transform.localPosition = localPosition;

            var effect = _effectSelector.GetRandomEffect();
            platform.SetEffect(effect);

            var color = SelectSmartColor(preferredColor);
            platform.SetColor(color);
            TrackColor(color);

            return platform;
        }

        private GameColorType SelectSmartColor(GameColorType? preferred)
        {
            Array values = Enum.GetValues(typeof(GameColorType));
            List<GameColorType> possibleColors = new();

            foreach (GameColorType type in values)
            {
                if (!_recentColors.Contains(type))
                    possibleColors.Add(type);
            }

            GameColorType selected;

            if (preferred.HasValue && Random.value < 0.5f)
            {
                selected = preferred.Value;
            }
            else if (possibleColors.Count > 0)
            {
                selected = possibleColors[Random.Range(0, possibleColors.Count)];
            }
            else
            {
                selected = (GameColorType)values.GetValue(Random.Range(0, values.Length));
            }

            return selected;
        }

        private void TrackColor(GameColorType color)
        {
            _recentColors.Add(color);
            if (_recentColors.Count > MaxRecentColors)
                _recentColors.RemoveAt(0);
        }

        public void Despawn(PlatformController platform)
        {
            _pool.Despawn(platform);
        }
    }
}
