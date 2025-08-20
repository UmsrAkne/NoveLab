using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    /// <summary>
    /// A no-op animation that only waits for the specified time.
    /// Useful for sequencing with delays or pauses in an animation chain.
    /// </summary>
    public class WaitAnimation : IUIAnimation
    {
        private CancellationTokenSource cts;

        public int TargetLayerIndex { get; set; }

        public string GroupId { get; set; } = string.Empty;

        public event Action OnCompleted;

        public bool IsPlaying { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary> Total time for one iteration (in seconds). </summary>
        public float Duration { get; set; } = 1f;

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary> Optional delay before the animation starts (in seconds). </summary>
        public float Delay { get; set; } = 0f;

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary> How many times to repeat the wait. 1 = play once. </summary>
        public int RepeatCount { get; set; } = 1;

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

            IsPlaying = false;
        }

        private async UniTaskVoid Run(CancellationToken token)
        {
            IsPlaying = true;

            try
            {
                // optional delay before start
                if (Delay > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(Delay), cancellationToken: token);
                }

                for (var i = 0; i < RepeatCount; i++)
                {
                    var elapsed = 0f;
                    while (elapsed < Duration)
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                        elapsed += Time.deltaTime;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }

            IsPlaying = false;
            OnCompleted?.Invoke();
        }
    }
}