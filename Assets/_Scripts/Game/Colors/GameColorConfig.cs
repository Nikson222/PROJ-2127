using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Colors
{
    [CreateAssetMenu(fileName = "GameColorConfig", menuName = "Configs/Game Color Config")]
    public class GameColorConfig : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public GameColorType ColorType;
            public Color Color;
        }

        [SerializeField] private List<Entry> _entries;

        private Dictionary<GameColorType, Color> _lookup;

        public Color GetColor(GameColorType type)
        {
            _lookup ??= BuildLookup();
            return _lookup.TryGetValue(type, out var color) ? color : Color.white;
        }

        private Dictionary<GameColorType, Color> BuildLookup()
        {
            var dict = new Dictionary<GameColorType, Color>();
            foreach (var entry in _entries)
            {
                dict[entry.ColorType] = entry.Color;
            }

            return dict;
        }
    }
}