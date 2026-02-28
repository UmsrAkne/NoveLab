using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ScenarioModel;
using UnityEngine;

namespace Audio
{
    public class SePlayer : MonoBehaviour
    {
        [SerializeField] private int channelCount = 4;

        private List<SeChannel> channels;

        private void Awake()
        {
            channels = new List<SeChannel>(channelCount);
            for (var i = 0; i < channelCount; i++)
            {
                var go = new GameObject($"SeChannel_{i}");
                go.transform.SetParent(transform);
                var source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;

                var channel = new SeChannel
                {
                    Source = source,
                    Queue = new Queue<(AudioClip, AudioOrder)>(),
                    CancelTokenSource = new CancellationTokenSource(),
                };

                channels.Add(channel);
                RunChannelLoop(channel).Forget(); // 非同期ループ起動
            }
        }

        public void Stop(int channelIndex)
        {
            if (channelIndex < 0 || channelIndex >= channels.Count)
            {
                Debug.LogError($"Invalid SE channel index: {channelIndex}");
                return;
            }

            var channel = channels[channelIndex];

            // 再生停止 & キャンセル
            channel.CancelTokenSource.Cancel();
            channel.Source.Stop();

            // キャンセルソース再生成（ループ継続のため）
            channel.CancelTokenSource = new CancellationTokenSource();

            // キュークリア
            channel.Queue.Clear();

            // 再実行（Forget で良い）
            RunChannelLoop(channel).Forget();
        }

        /// <summary>
        /// Enqueues a sound effect for playback on the specified channel.
        /// This does not play the clip immediately — it is added to a queue and played in order.
        /// </summary>
        /// <param name="clip">The AudioClip to be played.</param>
        /// <param name="order">Playback settings including channel, volume, pan, delay, and repeat count.</param>
        public void PlaySe(AudioClip clip, AudioOrder order)
        {
            if (order.ChannelIndex < 0 || order.ChannelIndex >= channels.Count)
            {
                Debug.LogError($"Invalid SE channel index: {order.ChannelIndex}");
                return;
            }

            var channel = channels[order.ChannelIndex];
            if (channel.CurrentScenarioId != order.ScenarioId)
            {
                channel.CurrentScenarioId = order.ScenarioId;
                Stop(order.ChannelIndex);
            }

            channel.Queue.Enqueue((clip, order));
        }

        private async UniTaskVoid RunChannelLoop(SeChannel channel)
        {
            // このメソッドはクラスの初期化後、常時実行され続ける。
            // キューが入力された時にだけ、メイン処理が実行される。

            var token = channel.CancelTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                if (channel.Queue.Count == 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    continue;
                }

                var (clip, order) = channel.Queue.Dequeue();

                channel.Source.clip = clip;
                channel.Source.volume = order.Volume;
                channel.Source.panStereo = order.Pan;

                try
                {
                    if (order.Delay > 0f)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(order.Delay), cancellationToken: token);
                    }

                    var repeat = Mathf.Max(1, order.RepeatCount);
                    for (var i = 0; i < repeat; i++)
                    {
                        channel.Source.Play();
                        await UniTask.WaitUntil(() => !channel.Source.isPlaying, cancellationToken: token);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Stopされた場合など
                    break;
                }
            }
        }

        private class SeChannel
        {
            public AudioSource Source;
            public Queue<(AudioClip, AudioOrder)> Queue;
            public CancellationTokenSource CancelTokenSource;
            public int CurrentScenarioId;
        }
    }
}