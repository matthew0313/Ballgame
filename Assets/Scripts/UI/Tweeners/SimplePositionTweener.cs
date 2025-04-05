using UnityEngine;
using DG.Tweening;
using System;

public class SimplePositionTweener : MonoBehaviour
{
    public Vector3 targetPosition;
    [SerializeField] float duration = 0.5f, delay;
    [SerializeField] Ease ease;
    [SerializeField] bool independantUpdate = true;

    [Header("Display")]
    [SerializeField] RectTransform rectTransform;
    Tween tween;
    private void OnEnable()
    {
        tween = rectTransform.DOAnchorPos(targetPosition, duration).SetDelay(delay).SetEase(ease).SetAutoKill(false).SetUpdate(independantUpdate);
    }
    private void OnDisable()
    {
        tween.Rewind(false);
        tween.Kill();
    }
}