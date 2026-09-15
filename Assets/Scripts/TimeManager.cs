using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    public static TimeManager Instance => instance;
    private float hitStopEndTime;
    private CancellationTokenSource cts;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    public void HitStop(float duration)
    {
        float endTime = Time.unscaledTime;
        if (endTime <= hitStopEndTime) return;
        hitStopEndTime = endTime;

        cts?.Cancel();
        cts = new();
        ActivateHitStop(duration).Forget();
    }
    private async UniTaskVoid ActivateHitStop(float duration)
    {
        Time.timeScale = 0f;
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(duration), cancellationToken: cts.Token, ignoreTimeScale: true);
        }
        catch (System.OperationCanceledException)
        {

        }
        Time.timeScale = 1f;
    }
}
