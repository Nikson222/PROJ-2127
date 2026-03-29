using UnityEngine;
using Zenject;

public class FloatingParticle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _minSize = 0.1f;
    [SerializeField] private float _maxSize = 0.3f;
    [SerializeField] private float _normalSpeed = 0.3f;
    [SerializeField] private float _fadeInTime = 1f;
    [SerializeField] private float _fadeOutTime = 1f;
    [SerializeField] private float _defaultLifetime = 10f;
    [SerializeField] private float _screenCheckDelay = 0.5f;

    private Camera _camera;
    private Vector3 _velocity;
    private Vector3 _targetVelocity;
    private float _lifeTimer;
    private bool _isFadingOut;
    private Color _baseColor;
    private float _maxLifetime;

    private System.Action<FloatingParticle> _onDespawn;

    public void Initialize(Sprite sprite, Color color, float lifetime, System.Action<FloatingParticle> onDespawn)
    {
        gameObject.SetActive(true);

        _camera = Camera.main;
        _baseColor = color;
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);

        float size = Random.Range(_minSize, _maxSize);
        transform.localScale = Vector3.one * size;

        Vector2 dir = Random.insideUnitCircle.normalized;
        _velocity = dir * _normalSpeed;
        _targetVelocity = _velocity;

        _lifeTimer = 0f;
        _isFadingOut = false;
        _maxLifetime = lifetime;

        _onDespawn = onDespawn;
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;

        float alpha = 1f;

        if (_lifeTimer < _fadeInTime)
        {
            alpha = Mathf.Lerp(0f, 1f, _lifeTimer / _fadeInTime);
        }

        if (_lifeTimer > _maxLifetime - _fadeOutTime)
        {
            _isFadingOut = true;
            float fadeProgress = (_lifeTimer - (_maxLifetime - _fadeOutTime)) / _fadeOutTime;
            alpha = Mathf.Lerp(1f, 0f, fadeProgress);
        }

        _spriteRenderer.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alpha);

        if (_lifeTimer >= _maxLifetime || (_lifeTimer > _screenCheckDelay && IsOutOfScreen()))
        {
            _onDespawn?.Invoke(this);
            return;
        }

        _velocity = Vector3.Lerp(_velocity, _targetVelocity, Time.deltaTime * 2f);
        transform.position += _velocity * Time.deltaTime;
    }

    public void ApplyImpact(Vector2 force)
    {
        _velocity += (Vector3)force;
        _targetVelocity = _velocity.normalized * _normalSpeed;
    }

    private bool IsOutOfScreen()
    {
        if (_camera == null) return false;

        Vector3 viewport = _camera.WorldToViewportPoint(transform.position);
        const float margin = 0.1f;

        return viewport.x < -margin || viewport.x > 1f + margin ||
               viewport.y < -margin || viewport.y > 1f + margin ||
               viewport.z < 0f;
    }

    public class Pool : MonoMemoryPool<FloatingParticle>
    {
        protected override void Reinitialize(FloatingParticle item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(FloatingParticle item)
        {
            item._spriteRenderer.sprite = null;
            item._spriteRenderer.color = Color.clear;
            item._velocity = Vector3.zero;
            item._targetVelocity = Vector3.zero;
            item._onDespawn = null;
            item.gameObject.SetActive(false);
        }
    }
}
