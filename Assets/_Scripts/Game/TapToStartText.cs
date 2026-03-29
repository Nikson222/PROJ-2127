using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TapToStartText : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private float _fadeDuration = 1f;

    private bool _hasTapped;
    private Tween _blinkTween;

    private void Start()
    {
        if (_text == null)
            _text = GetComponent<Text>();

        StartBlinking();
    }

    private void Update()
    {
        if (_hasTapped) return;

        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            _hasTapped = true;
            StopBlinking();
            _text.gameObject.SetActive(false);
        }
    }

    private void StartBlinking()
    {
        _blinkTween = _text.DOFade(0f, _fadeDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopBlinking()
    {
        if (_blinkTween != null && _blinkTween.IsActive())
            _blinkTween.Kill();
    }
}