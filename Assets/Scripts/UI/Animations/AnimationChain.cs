using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;

namespace UI.Animations
{
    /// <summary>
    /// IUIAnimation を順次実行するチェーン。
    /// AnimationChain 自身も IUIAnimation を実装するが、
    /// Add 時に AnimationChain を弾くことで入れ子は禁止にしている。
    /// </summary>
    public class AnimationChain : IUIAnimation
    {
        private readonly List<IUIAnimation> animations = new ();
        private CancellationTokenSource cts;

        public bool IsPlaying { get; private set; }

        /// <summary>
        /// ループ回数。1 なら 1 回だけ再生、2 なら 2 回…。
        /// -1 を指定すると無限ループ。
        /// 0 以下かつ -1 以外の値は無視される（=1 と同等）。
        /// </summary>
        public int LoopCount { get; set; } = 1;

        public event Action OnCompleted;

        public void Add(IUIAnimation animation)
        {
            if (animation is AnimationChain)
            {
                throw new InvalidOperationException("AnimationChain cannot contain another AnimationChain.");
            }

            animations.Add(animation);
        }

        public void Clear()
        {
            animations.Clear();
        }

        public void Start()
        {
            Stop(); // 前回再生をキャンセル
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

            // 進行中の個々のアニメーションも止めておく（安全側）
            foreach (var a in animations)
            {
                if (a.IsPlaying)
                {
                    a.Stop();
                }
            }

            IsPlaying = false;
        }

        public string GroupId { get; set; }

        private async UniTaskVoid Run(CancellationToken token)
        {
            IsPlaying = true;

            try
            {
                if (animations.Count == 0)
                {
                    IsPlaying = false;
                    OnCompleted?.Invoke();
                    return;
                }

                var remaining = NormalizeLoopCount(LoopCount);

                while (!token.IsCancellationRequested && remaining != 0)
                {
                    for (var i = 0; i < animations.Count;)
                    {
                        token.ThrowIfCancellationRequested();

                        var current = animations[i];
                        var currentGroupId = current.GroupId;

                        // 個別実行（GroupIdなし）
                        if (string.IsNullOrEmpty(currentGroupId))
                        {
                            await PlayOneAsync(current, token);
                            i++;
                            continue;
                        }

                        // 同じ GroupId の連続要素をまとめて同時実行
                        var group = new List<IUIAnimation>();
                        do
                        {
                            group.Add(animations[i]);
                            i++;
                        } while (
                            i < animations.Count && animations[i].GroupId == currentGroupId
                        );

                        await PlayManyAsync(group, token);
                    }

                    if (remaining > 0)
                    {
                        remaining--;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                IsPlaying = false;
                if (!token.IsCancellationRequested)
                {
                    OnCompleted?.Invoke();
                }
            }
        }

        private static int NormalizeLoopCount(int loopCount)
        {
            if (loopCount == -1)
            {
                return -1; // infinite
            }

            if (loopCount <= 0)
            {
                return 1; // 0 や -2 などは 1 回として扱う
            }

            return loopCount;
        }

        private static async UniTask PlayOneAsync(IUIAnimation anim, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            void Handler()
            {
                tcs.TrySetResult(true);
            }

            try
            {
                anim.OnCompleted += Handler;
                anim.Start();

                // OnCompleted が来るまで待つ（Cancel 可能）
                await tcs.Task.AttachExternalCancellation(token);
            }
            finally
            {
                anim.OnCompleted -= Handler;

                // Chain 側で Stop した場合など、完了イベントが飛ばないケースでも
                // 念のため止めておく
                if (anim.IsPlaying)
                {
                    anim.Stop();
                }
            }
        }

        private static async UniTask PlayManyAsync(IEnumerable<IUIAnimation> group, CancellationToken token)
        {
            var tasks = new List<UniTask>(16);
            foreach (var anim in group)
            {
                tasks.Add(PlayOneAsync(anim, token));
            }

            await UniTask.WhenAll(tasks);
        }
    }
}