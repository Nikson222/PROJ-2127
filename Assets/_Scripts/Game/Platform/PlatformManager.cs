using System.Collections.Generic;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;
using _Scripts.Game.Colors;
using _Scripts.Game.Effects;

namespace _Scripts.Game.Platform
{
    public class PlatformManager : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private PlatformController _platformPrefab;
        [SerializeField] private Sprite _maskSprite;

        private PlatformSpawner _spawner;
        private PlatformMover _mover;
        private PlatformSettings _settings;
        private InputController _input;
        private AudioService _audioService;

        private readonly List<PlatformController> _activePlatforms = new();
        private readonly Dictionary<Transform, Tween> _moveTweens = new();
        private readonly Dictionary<Collider2D, PlatformController> _platformMap = new();

        private bool _queuedShift;
        private bool _isShifting;
        private bool _isDestroying;
        private bool _isGameStarted;

        [Inject]
        public void Construct(
            PlatformSpawner spawner,
            PlatformMover mover,
            PlatformSettings settings,
            InputController input,
            AudioService audioService)
        {
            _spawner = spawner;
            _mover = mover;
            _settings = settings;
            _input = input;
            _audioService = audioService;

            _input.Clicked += ShiftPlatforms;
        }

        private void Start()
        {
            InitPlatforms(_settings.InitialPlatformCount);
            CreateTopMask();
        }

        private void OnDestroy()
        {
            _input.Clicked -= ShiftPlatforms;
        }

        private void InitPlatforms(int count)
        {
            float startY = CalculateSpawnY();
            float spacing = GetPlatformSpacing();

            for (int i = 0; i < count; i++)
            {
                var pos = new Vector3(0, startY + i * spacing, 0);
                var platform = _spawner.Spawn(pos);
                _activePlatforms.Insert(0, platform);
                _platformMap[platform.Collider] = platform;
            }
        }

        public List<GameColorType> GetActivePlatformColors()
        {
            HashSet<GameColorType> uniqueColors = new();
            foreach (var platform in _activePlatforms)
                uniqueColors.Add(platform.ColorType);

            return new List<GameColorType>(uniqueColors);
        }

        private void ShiftPlatforms()
        {
            if (!_isGameStarted)
            {
                _isGameStarted = true;
                return;
            }

            if (_isShifting || _isDestroying)
            {
                _queuedShift = true;
                return;
            }

            _isShifting = true;
            _moveTweens.Clear();

            var topPlatform = _activePlatforms[0];
            _activePlatforms.RemoveAt(0);
            _platformMap.Remove(topPlatform.Collider);

            float spacing = GetPlatformSpacing();

            _audioService.PlaySound(SoundType.Swap);

            _mover.MoveAndDespawnTop(topPlatform, spacing, () =>
            {
                _spawner.Despawn(topPlatform);

                var spawnY = CalculateSpawnY();
                var newPlatform = _spawner.Spawn(new Vector3(0, spawnY, 0));
                _activePlatforms.Add(newPlatform);
                _platformMap[newPlatform.Collider] = newPlatform;

                _isShifting = false;

                if (_queuedShift)
                {
                    _queuedShift = false;
                    ShiftPlatforms();
                }
            });

            _mover.ShiftAll(_activePlatforms, spacing, _moveTweens);
        }

        private void ShiftAfterDestruction(PlatformController destroyedPlatform)
        {
            if (_isShifting || _isDestroying)
                return;

            _isDestroying = true;
            _isShifting = true;

            DOTween.KillAll(true);
            foreach (var tween in _moveTweens.Values)
                tween.Kill();

            _moveTweens.Clear();

            _activePlatforms.Remove(destroyedPlatform);
            _platformMap.Remove(destroyedPlatform.Collider);

            var spacing = GetPlatformSpacing();
            var nextTop = _activePlatforms.Count > 0 ? _activePlatforms[0] : null;
            nextTop?.EnableCollider(false);
            
            _audioService.PlaySound(SoundType.Swap);

            var platformsToShift = new List<PlatformController>(_activePlatforms) { destroyedPlatform };
            _mover.ShiftAll(platformsToShift, spacing, _moveTweens);

            _moveTweens[destroyedPlatform.transform].OnComplete(() =>
            {
                _spawner.Despawn(destroyedPlatform);
            });

            DOTween.Sequence()
                .AppendInterval(_settings.ShiftDuration)
                .OnComplete(() =>
                {
                    var spawnY = CalculateSpawnY();
                    var newPlatform = _spawner.Spawn(new Vector3(0, spawnY, 0));
                    _activePlatforms.Add(newPlatform);
                    _platformMap[newPlatform.Collider] = newPlatform;

                    nextTop?.EnableCollider(true);

                    _isDestroying = false;
                    _isShifting = false;

                    if (_queuedShift)
                    {
                        _queuedShift = false;
                        ShiftPlatforms();
                    }
                });
        }

        public bool TryHandleCollision(Collider2D collider, GameColorType ballColor, out EffectType effectType)
        {
            effectType = EffectType.None;

            if (!_platformMap.TryGetValue(collider, out var platform))
                return false;

            if (platform.ColorType != ballColor)
                return false;

            effectType = platform.EffectType;
            ShiftAfterDestruction(platform);
            return true;
        }

        private float CalculateSpawnY()
        {
            float bottom = _mainCamera.ViewportToWorldPoint(Vector3.zero).y;
            return bottom - _platformPrefab.Height / 2f - _settings.ShiftDistance;
        }

        private float GetPlatformSpacing()
        {
            return _platformPrefab.Height + _settings.ShiftDistance;
        }

        private void CreateTopMask()
        {
            if (_activePlatforms.Count == 0) return;

            var maskGO = new GameObject("TopMask");
            var spriteMask = maskGO.AddComponent<SpriteMask>();
            spriteMask.sprite = _maskSprite;

            spriteMask.isCustomRangeActive = true;
            spriteMask.frontSortingOrder = 10;
            spriteMask.backSortingOrder = -10;

            float bottomY = _activePlatforms[0].transform.position.y;
            float topY = _activePlatforms[^1].transform.position.y;
            float middleY = (topY + bottomY) / 2f;
            float height = Mathf.Abs(topY - bottomY) + GetPlatformSpacing();
            float width = _mainCamera.orthographicSize * 2f * _mainCamera.aspect;

            maskGO.transform.position = new Vector3(0f, middleY, 0f);
            maskGO.transform.localScale = new Vector3(width, height, 1f);
        }
    }
}
