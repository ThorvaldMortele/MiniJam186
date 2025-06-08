using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float HoverScale = 1.1f;
    public float Duration = 0.2f;

    private Vector3 _originalScale;
    private Tween _scaleTween;

    void Start()
    {
        _originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(_originalScale * HoverScale, Duration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(_originalScale, Duration).SetEase(Ease.OutBack);
    }
}
