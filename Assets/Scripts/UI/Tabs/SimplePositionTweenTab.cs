using UnityEngine;
using DG.Tweening;
using System;

public class SimplePositionTweenTab : Tab
{
    public Vector3 targetPosition;
    [SerializeField] float duration = 0.5f, delay;
    [SerializeField] Ease ease;
    [SerializeField] bool independantUpdate = true;

    [Header("Display")]
    [SerializeField] RectTransform rectTransform;
    Tween tween;
    public override void OpenTab()
    {
        base.OpenTab();
        if(tween == null)
        {
            tween = rectTransform.DOAnchorPos(targetPosition, duration).SetDelay(delay).SetEase(ease).SetAutoKill(false).SetUpdate(independantUpdate);
        }
        tween.OnComplete(null);
        tween.PlayForward();
    }
    public override void CloseTab()
    {
        tween.OnComplete(base.CloseTab);
        tween.PlayBackwards();
    }
    private void OnDisable()
    {
        tween?.Rewind(false);
    }
}