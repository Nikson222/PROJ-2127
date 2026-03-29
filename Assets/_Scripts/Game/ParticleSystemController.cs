using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ParticleSystemController : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Sprite[] _possibleSprites;
    [SerializeField] private Color[] _possibleColors;
    [SerializeField] private int _maxParticles = 50;
    [SerializeField] private float _spawnRate = 0.5f;
    [SerializeField] private float _particleLifetime = 10f;
    [SerializeField] private float _spawnDepth = 5f;

    [Header("Wave Settings")]
    [SerializeField] private float _waveForceMultiplier = 0.2f;

    private List<FloatingParticle> _activeParticles = new();
    private FloatingParticle.Pool _particlePool;
    private float _spawnTimer;
    private Camera _mainCamera;

    [Inject]
    private void Construct(FloatingParticle.Pool particlePool)
    {
        _particlePool = particlePool;
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnRate && _activeParticles.Count < _maxParticles)
        {
            SpawnParticle();
            _spawnTimer = 0f;
        }

        _activeParticles.RemoveAll(p => p == null || !p.gameObject.activeInHierarchy);
    }

    private void SpawnParticle()
    {
        Vector2 viewportPos = new Vector2(
            Random.Range(0.05f, 0.95f),
            Random.Range(0.05f, 0.95f)
        );

        Vector3 spawnPos = _mainCamera.ViewportToWorldPoint(new Vector3(
            viewportPos.x, viewportPos.y, _spawnDepth
        ));

        Sprite sprite = _possibleSprites[Random.Range(0, _possibleSprites.Length)];
        Color color = _possibleColors[Random.Range(0, _possibleColors.Length)];

        var particle = _particlePool.Spawn();
        particle.transform.SetParent(transform);
        particle.transform.position = spawnPos;
        particle.Initialize(sprite, color, _particleLifetime, DespawnParticle);

        _activeParticles.Add(particle);
    }

    private void DespawnParticle(FloatingParticle particle)
    {
        _particlePool.Despawn(particle);
    }

    public void ApplyForceWave(Vector2 origin, float force)
    {
        foreach (var particle in _activeParticles)
        {
            if (particle == null) continue;

            Vector2 toParticle = (Vector2)particle.transform.position - origin;
            float distance = toParticle.magnitude + 0.1f;
            Vector2 impulse = toParticle.normalized * (force * _waveForceMultiplier / distance);

            particle.ApplyImpact(impulse);
        }
    }

    public void SpawnExplosion(Vector3 position, int particleCount, float explosionForce)
    {
        for (int i = 0; i < particleCount; i++)
        {
            if (_activeParticles.Count >= _maxParticles)
                break;

            var particle = _particlePool.Spawn();
            particle.transform.SetParent(transform);
            particle.transform.position = position;

            Sprite sprite = _possibleSprites[Random.Range(0, _possibleSprites.Length)];
            Color color = _possibleColors[Random.Range(0, _possibleColors.Length)];
            particle.Initialize(sprite, color, _particleLifetime, DespawnParticle);

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 impulse = randomDirection * explosionForce * Random.Range(0.5f, 1.2f);
            particle.ApplyImpact(impulse);

            _activeParticles.Add(particle);
        }
    }

    public void SpawnCollisionBurst(Vector3 position, int particleCount, float upwardForce)
    {
        for (int i = 0; i < particleCount; i++)
        {
            if (_activeParticles.Count >= _maxParticles)
                break;

            var particle = _particlePool.Spawn();
            particle.transform.SetParent(transform);
            particle.transform.position = position;

            Sprite sprite = _possibleSprites[Random.Range(0, _possibleSprites.Length)];
            Color color = _possibleColors[Random.Range(0, _possibleColors.Length)];
            particle.Initialize(sprite, color, _particleLifetime, DespawnParticle);

            Vector2 direction = (Vector2.up + Random.insideUnitCircle * 1.0f).normalized;
            Vector2 impulse = direction * upwardForce * Random.Range(1.0f, 1.5f);
            particle.ApplyImpact(impulse);

            _activeParticles.Add(particle);
        }
    }

}
