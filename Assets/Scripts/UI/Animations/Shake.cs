using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    public class Shake : IUIAnimation
    {
        private readonly IImageContainer container;
        private IDisplayImage image;
        private CancellationTokenSource cts;

        public int TargetLayerIndex { get; set; }

        public event Action OnCompleted;

        public bool IsPlaying { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public float Duration { get; set; } = 0.5f;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Strength { get; set; } = 10f;

        public int Vibrato { get; set; } = 20;

        public Shake(IDisplayImage image, IImageContainer container = null)
        {
            this.image = image;
            this.container = container;
        }

        public void Start()
        {
            image = container?.GetFront();

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

            IsPlaying = false;
        }

        public string GroupId { get; set; }

        private async UniTaskVoid Run(CancellationToken token)
        {
            IsPlaying = true;

            var elapsed = 0f;

            while (elapsed < Duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                elapsed += Time.deltaTime;

                var decay = 1f - elapsed / Duration;
                var angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                var displacement = UnityEngine.Random.Range(0f, Strength) * decay;
                var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * displacement;

                image.SetOffsetPosition(offset);
            }

            image.SetOffsetPosition(Vector2.zero);

            IsPlaying = false;
            OnCompleted?.Invoke();
        }
    }
}