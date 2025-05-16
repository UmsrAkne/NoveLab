using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    public class FadeIn : IUIAnimation
    {
        private readonly IDisplayImage image;
        private CancellationTokenSource cts;

        public bool IsPlaying { get; private set; }

        public event Action OnCompleted;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Duration { get; set; } = 0.5f;

        public FadeIn(IDisplayImage image)
        {
            this.image = image;
        }

        public void Start()
        {
            Stop(); // キャンセルして再スタート
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

            IsPlaying = false;
        }

        private async UniTaskVoid Run(CancellationToken token)
        {
            IsPlaying = true;
            var elapsed = 0f;

            image.SetAlpha(0);

            while (elapsed < Duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                elapsed += Time.deltaTime;

                var t = Mathf.Clamp01(elapsed / Duration);
                var eased = Mathf.SmoothStep(0, 1, t); // イージング適用
                image.SetAlpha(eased);
            }

            image.SetAlpha(1);
            IsPlaying = false;
            OnCompleted?.Invoke();
        }
    }
}