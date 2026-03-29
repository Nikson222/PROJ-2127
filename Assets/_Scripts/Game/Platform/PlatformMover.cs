using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;

namespace _Scripts.Game.Platform
{
    public class PlatformMover
    {
        private readonly float _shiftDistance;
        private readonly float _shiftDuration;

        public PlatformMover(float shiftDistance, float shiftDuration)
        {
            _shiftDistance = shiftDistance;
            _shiftDuration = shiftDuration;
        }

        public void ShiftAll(List<PlatformController> platforms, float spacing, Dictionary<Transform, Tween> tweens)
        {
            
            foreach (var platform in platforms)
            {
                var tr = platform.transform;
                var targetPos = tr.localPosition + new Vector3(0, spacing, 0);

                if (DOTween.IsTweening(tr))
                {
                    var existing = DOTween.TweensByTarget(tr, true).FirstOrDefault();
                    if (existing is Tweener tweener)
                    {
                        tweener.ChangeEndValue(targetPos, true).Restart();
                        tweens[tr] = tweener;
                        continue;
                    }
                }

                var moveTween = tr.DOLocalMove(targetPos, _shiftDuration)
                    .SetEase(Ease.InOutBack);
                tweens[tr] = moveTween;
            }
        }

        public void MoveAndDespawnTop(PlatformController platform, float spacing, System.Action onComplete)
        {
            platform.EnableCollider(false);

            var tr = platform.transform;
            var targetPos = tr.localPosition + new Vector3(0, spacing, 0);

            tr.DOLocalMove(targetPos, _shiftDuration)
                .SetEase(Ease.InOutBack)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}