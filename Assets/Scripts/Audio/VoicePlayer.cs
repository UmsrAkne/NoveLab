using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ScenarioModel;
using UnityEngine;

namespace Audio
{
    public class VoicePlayer : MonoBehaviour
    {
        [SerializeField] private int channelCount = 4;

        private List<VoiceChannel> channels;

        public event Action<int> OnPlaybackCompleted; // チャンネル番号を通知

        private void Awake()
        {
            channels = new List<VoiceChannel>(channelCount);
            for (var i = 0; i < channelCount; i++)
            {
                var go = new GameObject($"VoiceChannel_{i}");
                go.transform.SetParent(transform);
                var source = go.AddComponent<AudioSource>();
                channels.Add(new VoiceChannel
                {
                    Source = source,
                    CancelTokenSource = new CancellationTokenSource(),
                });
            }
        }

        public async UniTask PlayVoiceAsync(AudioClip clip, AudioOrder order)
        {
            Debug.Log($"[VOICE START] ch:{order.ChannelIndex} time:{Time.time}");

            if (order.ChannelIndex < 0 || order.ChannelIndex >= channels.Count)
            {
                Debug.LogError($"Invalid channel index: {order.ChannelIndex}");
                return;
            }

            var channel = channels[order.ChannelIndex];

            // 既存の再生を中断
            channel.CancelTokenSource.Cancel();
            channel.Source.Stop();

            // 新しいキャンセルソースを用意
            channel.CancelTokenSource = new CancellationTokenSource();
            var token = channel.CancelTokenSource.Token;

            channel.Source.clip = clip;
            channel.Source.volume = order.Volume;
            channel.Source.panStereo = order.Pan;

            try
            {
                if (order.Delay > 0f)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(order.Delay), cancellationToken: token);
                }

                channel.Source.Play();
                await UniTask.WaitUntil(() => !channel.Source.isPlaying, cancellationToken: token);

                // 再生完了イベント（中断されていない場合のみ）
                if (!token.IsCancellationRequested)
                {
                    OnPlaybackCompleted?.Invoke(order.ChannelIndex);
                }
            }
            catch (OperationCanceledException)
            {
                // 中断された場合、完了イベントは出さない
            }
        }

        private class VoiceChannel
        {
            public AudioSource Source;
            public CancellationTokenSource CancelTokenSource;
        }
    }
}