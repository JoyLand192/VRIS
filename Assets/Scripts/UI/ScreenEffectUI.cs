using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectUI : MonoBehaviour
{
    [SerializeField] private Image hurtFlashImage;
    [SerializeField] private IntEventChannel hurtFlashEventChannel;
    public void Awake()
    {
        hurtFlashEventChannel.OnRaised += HurtFlash;
    }
    public void HurtFlash(int intensity) => FlashScreen(Color.red, intensity);
    public void FlashScreen(Color color, int intensity)
    {
        var startColor = color;
        var endColor = color;

        startColor.a = intensity * 0.25f;
        endColor.a = 0;

        hurtFlashImage.DOKill();
        hurtFlashImage.color = startColor;
        hurtFlashImage.DOColor(endColor, 0.5f + intensity * 0.125f);
    }
}
