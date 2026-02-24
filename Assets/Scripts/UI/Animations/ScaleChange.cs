using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Animations
{
    public class ScaleChange : IUIAnimation
    {
        private IDisplayImage image;
        private readonly IImageContainer container;
        private CancellationTokenSource cts;

        public bool IsPlaying { get; private set; }

        public float Duration { get; set; } = 0.5f;

        // Target scale to reach by the end of animation
        public float TargetScale { get; set; } = 1f;

        public float To { set => TargetScale = value; }

        public ScaleChange(IDisplayImage image, IImageContainer container = null)
        {
            this.image = image;
            this.container = container;
        }

        public void Start()
        {
            image = container?.GetFront() ?? image;

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

        public int TargetLayerIndex { get; set; }

        public event Action OnCompleted;

        private async UniTaskVoid Run(CancellationToken token)
        {
            if (image == null)
            {
                return;
            }

            IsPlaying = true;

            try
            {
                var startScale = ReadCurrentScale(image);
                var endScale = TargetScale;

                var elapsed = 0f;
                while (elapsed < Duration)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    elapsed += Time.deltaTime;

                    var t = Mathf.Clamp01(elapsed / Duration);
                    var eased = Mathf.SmoothStep(0f, 1f, t);
                    var s = Mathf.LerpUnclamped(startScale, endScale, eased);
                    image.SetScale(s);
                }

                // ensure final value
                image.SetScale(endScale);
            }
            catch (OperationCanceledException)
            {
                // treat as a graceful stop
            }
            finally
            {
                IsPlaying = false;
                OnCompleted?.Invoke();
            }
        }

        private static float ReadCurrentScale(IDisplayImage img)
        {
            if (img?.GameObject == null)
            {
                return 1f;
            }

            var go = img.GameObject;
            var rt = go.GetComponent<RectTransform>();
            if (rt == null)
            {
                return 1f;
            }

            // If this is a UI Image-based adapter (e.g., SpriteAdapter), sizeDelta may represent scale
            var uiImage = go.GetComponent<Image>();
            if (uiImage != null && uiImage.sprite != null)
            {
                var baseSize = uiImage.sprite.rect.size;
                if (baseSize.x > 0f && baseSize.y > 0f)
                {
                    var size = rt.sizeDelta;
                    // Use average of x/y to be robust
                    var sx = size.x / baseSize.x;
                    var sy = size.y / baseSize.y;
                    var avg = (sx + sy) * 0.5f;
                    if (avg > 0f)
                    {
                        return avg;
                    }
                }
            }

            // Otherwise, fall back to localScale.x (e.g., ImageSet)
            var ls = rt.localScale;
            return ls.x != 0f ? ls.x : 1f;
        }
    }
}