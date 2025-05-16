using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    public class Slide : IUIAnimation
    {
        private readonly IDisplayImage image;
        private CancellationTokenSource cts;

        public bool IsPlaying { get; private set; }

        public event Action OnCompleted;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Duration { get; set; } = 0.5f;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Distance { get; set; } = 100f;

        // ReSharper disable once MemberCanBePrivate.Global
        public float Angle { get; set; }

        public Slide(IDisplayImage image)
        {
            this.image = image;
        }

        public void Start()
        {
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

        private async UniTaskVoid Run(CancellationToken token)
        {
            IsPlaying = true;

            // 開始位置を取得
            var start = image.GameObject.GetComponent<RectTransform>().anchoredPosition;

            // 移動方向（角度から算出）
            var radians = Angle * Mathf.Deg2Rad;
            var offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * Distance;
            var end = start + offset;

            var elapsed = 0f;

            while (elapsed < Duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                elapsed += Time.deltaTime;

                var t = Mathf.Clamp01(elapsed / Duration);
                var eased = Mathf.SmoothStep(0, 1, t); // イージング

                var pos = Vector2.LerpUnclamped(start, end, eased);
                image.SetPosition(pos);
            }

            image.SetPosition(end);

            IsPlaying = false;
            OnCompleted?.Invoke();
        }
    }
}