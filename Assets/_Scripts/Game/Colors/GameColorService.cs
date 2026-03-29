using UnityEngine;

namespace _Scripts.Game.Colors
{
    public class GameColorService
    {
        private readonly GameColorConfig _config;

        public GameColorService(GameColorConfig config)
        {
            _config = config;
        }

        public Color GetColor(GameColorType type)
        {
            return _config.GetColor(type);
        }
    }
}