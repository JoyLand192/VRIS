using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
public struct GuardStateInfo
{
    public float FillAmount;
    public bool IsBroken;
    public GuardStateInfo(float fillAmount, bool isBroken)
    {
        FillAmount = fillAmount;
        IsBroken = isBroken;
    }
}
[RequireComponent(typeof(Image))]
public class GuardBar : MonoBehaviour
{
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Sprite brokenImage;
    [SerializeField] private Image barFill;
    [SerializeField] private GuardStateInfoEventChannel guardStateEventChannel;
    [SerializeField] private IntEventChannel guardDurabilityEventChannel;
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
        guardStateEventChannel.OnRaised += SetState;
    }
    public void SetState(GuardStateInfo info)
    {
        var fixedValue = Mathf.Clamp01(info.FillAmount);
        barFill.fillAmount = fixedValue;

        image.sprite = info.IsBroken ? brokenImage : defaultImage;
        guardDurabilityEventChannel.Raise((int)(fixedValue * 3));
        Debug.Log($"{(int)(fixedValue * 3)}");
    }
}
