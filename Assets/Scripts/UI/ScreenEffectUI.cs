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
    public void HurtFlash(int intensity)
    {
        hurtFlashImage.DOKill();
        hurtFlashImage.color = new Color(1f, 0f, 0f, intensity * 0.25f);
        hurtFlashImage.DOColor(new Color(1f, 0f, 0f, 0f), 0.5f + intensity * 0.125f);
    }
}
