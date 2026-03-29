using UnityEngine;
using Zenject;
using _Scripts.Game.Effects;
using _Scripts.Game.Platform;

[RequireComponent(typeof(Camera))]
public class CameraSizeInitializer : MonoBehaviour
{
    [Header("Reference Settings")]
    [Tooltip("Базовое разрешение, под которое выверены размеры и orthoSize")]
    [SerializeField] private Vector2 _referenceResolution = new Vector2(1080f, 1920f);

    [Tooltip("OrthoSize, который должен быть при базовом разрешении")]
    [SerializeField] private float _referenceOrthoSize = 5f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        AdjustCameraSize();
    }

    private void AdjustCameraSize()
    {
        float targetAspect = _referenceResolution.x / _referenceResolution.y;
        float currentAspect = (float)Screen.width / Screen.height;

        float adjustedOrthoSize = _referenceOrthoSize * (targetAspect / currentAspect);

        _camera.orthographicSize = adjustedOrthoSize;
    }
}