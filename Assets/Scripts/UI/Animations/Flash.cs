using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    public class Flash : IUIAnimation
    {
        private readonly IImageContainer stacker;
        private CancellationTokenSource cts;

        public float Duration { get; set; } = 0.3f;

        public float MaxAlpha { get; set; } = 1f;

        public int TargetLayerIndex { get; set; }

        public int RepeatCount { get; set; } = 1; // 1 = 1回実行

        public float Delay { get; set; } = 0f; // 開始前待機

        public float Interval { get; set; } = 0f; // 各ループ間待機

        public event Action OnCompleted;

        public bool IsPlaying { get; private set; }

        public string GroupId { get; set; }

        public Flash(IImageContainer stacker)
        {
            this.stacker = stacker;
        }

        public void Start()
        {
            Stop();
            cts = new CancellationTokenSource();
            Run(cts.Token).Forget();
        }

        public void Stop()
        {
            if (cts is { IsCancellationRequested: false, })
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }

            stacker?.SetEffectAlpha(0f);
            IsPlaying = false;
        }

        private async UniTaskVoid Run(CancellationToken token)
        {
            if (stacker == null)
            {
                return;
            }

            IsPlaying = true;

            try
            {
                // --- 開始前ディレイ ---
                if (Delay > 0f)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(Delay),
                        cancellationToken: token);
                }

                var loop = 0;

                // RepeatCount <= 0 を無限扱いにするならこう
                while (RepeatCount <= 0 || loop < RepeatCount)
                {
                    var elapsed = 0f;

                    // --- フラッシュ本体 ---
                    while (elapsed < Duration)
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                        elapsed += Time.deltaTime;

                        var t = elapsed / Duration;

                        // 山型 0→1→0
                        var alpha = Mathf.Sin(t * Mathf.PI) * MaxAlpha;
                        stacker.SetEffectAlpha(alpha);
                    }

                    stacker.SetEffectAlpha(0f);

                    loop++;

                    // --- 最後のループ後は Interval 不要 ---
                    if (RepeatCount > 0 && loop >= RepeatCount)
                    {
                        break;
                    }

                    if (Interval > 0f)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(Interval),
                            cancellationToken: token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセルは正常終了扱い
            }
            finally
            {
                stacker?.SetEffectAlpha(0f);
                IsPlaying = false;
                OnCompleted?.Invoke();
            }
        }
    }
}