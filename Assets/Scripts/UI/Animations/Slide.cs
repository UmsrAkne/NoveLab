using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    public class Slide : IUIAnimation
    {
        private IDisplayImage image;
        private readonly IImageContainer container;
        private CancellationTokenSource cts;

        public bool IsPlaying { get; private set; }

        public int TargetLayerIndex { get; set; }

        public event Action OnCompleted;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Duration { get; set; } = 0.5f;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Distance { get; set; } = 100f;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Angle { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public int RepeatCount { get; set; } = 1;

        public Slide(IDisplayImage image, IImageContainer container = null)
        {
            this.image = image;
            this.container = container;
        }

        public void Start()
        {
            if (image == null && container != null)
            {
                image = container.GetFront();
            }

            Stop(); // 前回のキャンセル
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

            var start = image.GameObject.GetComponent<RectTransform>().anchoredPosition;
            var radians = Angle * Mathf.Deg2Rad;
            var offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * Distance;
            var end = start + offset;

            var remaining = RepeatCount;

            // 初回の方向：true = start -> end, false = end -> start
            var forward = true;

            while (remaining != 0)
            {
                var from = forward ? start : end;
                var to = forward ? end : start;

                var elapsed = 0f;

                while (elapsed < Duration)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    elapsed += Time.deltaTime;

                    var t = Mathf.Clamp01(elapsed / Duration);
                    var eased = Mathf.SmoothStep(0, 1, t);
                    var pos = Vector2.LerpUnclamped(from, to, eased);
                    image.SetBasePosition(pos);
                }

                image.SetBasePosition(to);

                if (remaining > 0)
                {
                    remaining--;
                }

                forward = !forward;
            }

            IsPlaying = false;
            OnCompleted?.Invoke();
        }
    }
}