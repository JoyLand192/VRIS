using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    private const float hurtFlashDuration = 0.15f;
    private static readonly int hurtFlashAmountID = Shader.PropertyToID("_HurtFlashAmount");
    private float hurtFlashAmount = 0;
    private SpriteRenderer render;
    private MaterialPropertyBlock mpb;
    private void Awake()
    {
        mpb = new();
        render = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (hurtFlashAmount >= 0)
        {
            hurtFlashAmount -= Time.deltaTime / hurtFlashDuration;

            render.GetPropertyBlock(mpb);
            mpb.SetFloat(hurtFlashAmountID, Mathf.Max(hurtFlashAmount, 0));
            render.SetPropertyBlock(mpb);
        }
    }
    public void HurtFlash()
    {
        hurtFlashAmount = 1f;
    }
}
