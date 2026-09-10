using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHealthBarUI : MonoBehaviour
{
    private const Ease fillEase = Ease.OutCirc;
    private const float fillDuration = 0.5f;
    private const float fillDelay = 0.1f;
    [SerializeField] private Transform healthBarBody;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healthBarFillDelayed;
    [SerializeField] private FloatEventChannel TEMPbossHealthBarEventChannel;
    private float currentFillAmount;
    private Tween fillDelayTween;
    private void Awake()
    {
        TEMPbossHealthBarEventChannel.OnRaised += SetHealthBar;
    }
    public void SetHealthBar(float value)
    {
        healthBarFill.fillAmount = value;
        fillDelayTween?.Kill();
        fillDelayTween = DOVirtual.DelayedCall(fillDelay, () =>
        {
            healthBarFillDelayed.DOFillAmount(value, fillDuration).SetEase(fillEase).SetUpdate(true);
        }).SetUpdate(true);
        currentFillAmount = value;
    }
}
