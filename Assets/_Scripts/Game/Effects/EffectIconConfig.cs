using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Effects
{
    [CreateAssetMenu(fileName = "EffectIconConfig", menuName = "Configs/Effect Icon Config")]
    public class EffectIconConfig : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public EffectType EffectType;
            public Sprite Icon;
        }

        [SerializeField] private List<Entry> _entries;

        private Dictionary<EffectType, Sprite> _lookup;

        public List<Entry> Icons => _entries;

        
        public Sprite GetIcon(EffectType type)
        {
            _lookup ??= BuildLookup();
            return _lookup.TryGetValue(type, out var sprite) ? sprite : null;
        }

        private Dictionary<EffectType, Sprite> BuildLookup()
        {
            var dict = new Dictionary<EffectType, Sprite>();
            foreach (var entry in _entries)
            {
                dict[entry.EffectType] = entry.Icon;
            }
            return dict;
        }
    }
}