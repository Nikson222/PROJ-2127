using _Scripts.Game.Colors;
using _Scripts.Game.Effects;
using _Scripts.Game.Platform;
using _Scripts.Game.Services;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;

namespace _Scripts.Game.Ball
{
    public class BallController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Jump Settings")]
        [SerializeField] private float _jumpForce = 9f;

        [Header("Dynamic Gravity")]
        [SerializeField] private float _minGravity = 0.5f;
        [SerializeField] private float _maxGravity = 20f;
        [SerializeField] private float _maxSpeedForGravity = 10f;

        [Header("Movement Limits")]
        [SerializeField] private bool _limitMovementX = false;
        [SerializeField] private float _minX = -2f;
        [SerializeField] private float _maxX = 2f;

        private Collider2D _collider;

        private EffectStrategyFactory _effectFactory;
        private PlatformManager _platformManager;
        private BallStateMachine _stateMachine;
        private GameColorService _colorService;
        private ScoreCounter _scoreCounter;
        private ParticleSystemController _particleSystem;
        private AudioService _audioService;

        private float _speedMultiplier = 1f;
        private float _effectTimer;
        private bool _hasActiveEffect;

        private float _gravityModifier = 1f;
        private float _gravityTimer;
        private bool _isGravityModified;

        private float _jumpModifier = 1f;
        private float _jumpModifierTimer;
        private bool _isJumpModified;

        private GameColorType _currentColor;
        private bool _waitingForStart = true;

        public GameColorType CurrentColor => _currentColor;
        public bool IsRising { get; private set; }

        [Inject]
        public void Construct(
            EffectStrategyFactory effectFactory,
            PlatformManager platformManager,
            BallStateMachine stateMachine,
            GameColorService colorService,
            ScoreCounter scoreCounter,
            ParticleSystemController particleSystemController,
            AudioService audioService)
        {
            _effectFactory = effectFactory;
            _platformManager = platformManager;
            _stateMachine = stateMachine;
            _colorService = colorService;
            _scoreCounter = scoreCounter;
            _particleSystem = particleSystemController;
            _audioService = audioService;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            _rb.gravityScale = 0f;
            _currentColor = GetRandomColorFromActivePlatforms();
            SetColor(_currentColor);
        }

        private void FixedUpdate()
        {
            if (_waitingForStart) return;

            ApplyVelocityBasedGravity();

            if (IsRising && _rb.linearVelocity.y <= 0f)
            {
                _collider.enabled = true;
                IsRising = false;
            }

            if (_hasActiveEffect && (_effectTimer -= Time.fixedDeltaTime) <= 0f)
                ResetSpeedModifier();

            if (_isGravityModified && (_gravityTimer -= Time.fixedDeltaTime) <= 0f)
            {
                _gravityModifier = 1f;
                _isGravityModified = false;
            }

            if (_isJumpModified && (_jumpModifierTimer -= Time.fixedDeltaTime) <= 0f)
            {
                _jumpModifier = 1f;
                _isJumpModified = false;
            }

            if (_limitMovementX)
            {
                Vector2 pos = _rb.position;
                pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
                _rb.position = pos;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_platformManager.TryHandleCollision(collision.collider, _currentColor, out var effect))
            {
                _audioService.PlaySound(SoundType.BallTouch);

                Vector2 contact = collision.contacts[0].point;

                _particleSystem.ApplyForceWave(contact, 2f);
                _particleSystem.SpawnCollisionBurst(contact, 8, 2f);

                var strategy = _effectFactory.GetStrategy(effect);
                strategy.Apply(this);

                int scoreToAdd = effect == EffectType.ExtraPoints ? 2 : 1;
                bool isBonus = effect == EffectType.ExtraPoints;
                _scoreCounter.AddScore(scoreToAdd, isBonus);

                Jump();
                _currentColor = GetRandomColorFromActivePlatforms();
                SetColor(_currentColor);
            }
            else
            {
                _audioService.PlaySound(SoundType.GameOver);
                Stop();
            }
        }

        public void EnableGravity() => _waitingForStart = false;

        private void Jump()
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * _jumpForce * _speedMultiplier * _jumpModifier, ForceMode2D.Impulse);
            _collider.enabled = false;
            IsRising = true;
        }

        private void ApplyVelocityBasedGravity()
        {
            float speed = Mathf.Abs(_rb.linearVelocity.y);
            float t = Mathf.InverseLerp(0, _maxSpeedForGravity * 0.8f, speed);
            float gravity = Mathf.Lerp(_minGravity, _maxGravity, t) * _gravityModifier;

            _rb.AddForce(Vector2.down * gravity, ForceMode2D.Force);
        }

        public void ApplyTemporarySpeedModifier(float multiplier, float duration)
        {
            _speedMultiplier = multiplier;
            _effectTimer = duration;
            _hasActiveEffect = true;
        }

        public void ResetSpeedModifier()
        {
            _speedMultiplier = 1f;
            _effectTimer = 0f;
            _hasActiveEffect = false;
        }

        public void ApplyTemporaryGravityModifier(float modifier, float duration)
        {
            _gravityModifier = modifier;
            _gravityTimer = duration;
            _isGravityModified = true;
        }

        public void ApplyTemporaryJumpModifier(float modifier, float duration)
        {
            _jumpModifier = modifier;
            _jumpModifierTimer = duration;
            _isJumpModified = true;
        }

        private GameColorType GetRandomColorFromActivePlatforms()
        {
            List<GameColorType> activeColors = _platformManager.GetActivePlatformColors();
            return activeColors.Count > 0
                ? activeColors[Random.Range(0, activeColors.Count)]
                : (GameColorType)Random.Range(0, System.Enum.GetValues(typeof(GameColorType)).Length);
        }

        private void SetColor(GameColorType colorType)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.color = _colorService.GetColor(colorType);
        }

        public void Stop()
        {
            _waitingForStart = true;
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0f;
            _collider.enabled = false;

            if (_spriteRenderer != null)
                _spriteRenderer.enabled = false;

            _particleSystem.SpawnExplosion(transform.position, 20, 5f);
            StartCoroutine(HandleDeath());
        }

        private IEnumerator HandleDeath()
        {
            yield return new WaitForSeconds(1f);
            _stateMachine.Enter<BallDeadState>();
        }
    }
}
