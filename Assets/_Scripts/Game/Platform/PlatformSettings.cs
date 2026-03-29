using UnityEngine;

namespace _Scripts.Game.Platform
{
    [CreateAssetMenu(fileName = "PlatformSettings", menuName = "Configs/Platform Settings")]
    public class PlatformSettings : ScriptableObject
    {
        [Header("Platform Movement")]
        [Min(0.01f)] public float ShiftDistance = 1.5f;
        [Min(0.01f)] public float ShiftDuration = 0.2f;

        [Header("Initial Spawn")]
        public int InitialPlatformCount = 5;

        [Header("Spawn Offset")]
        [Min(0f)] public float SpawnYOffset = 0.2f;
    }
}