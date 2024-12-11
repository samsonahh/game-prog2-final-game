using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TweenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float duration = 0.1f;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.KillAll();
        transform.DOScale(hoverScale * originalScale, duration).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.KillAll();
        transform.DOScale(originalScale, duration).SetUpdate(true);
    }
}
