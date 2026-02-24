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
                    State = VoicePlaybackState.Idle,
                    PlayId = 0,
                });
            }
        }

        // 「再生中」= 実際に音が鳴ってる
        public bool IsPlaying(int channelIndex)
        {
            if (!IsValidChannel(channelIndex)) return false;
            return channels[channelIndex].State == VoicePlaybackState.Playing;
        }

        // 「稼働中」= Delay待ち or 再生中（Bgv即再生判定にはこっちが便利）
        public bool IsActive(int channelIndex)
        {
            if (!IsValidChannel(channelIndex)) return false;
            return channels[channelIndex].State != VoicePlaybackState.Idle;
        }

        public VoicePlaybackState GetState(int channelIndex)
        {
            if (!IsValidChannel(channelIndex)) return VoicePlaybackState.Idle;
            return channels[channelIndex].State;
        }

        public void Stop(int channelIndex)
        {
            if (!IsValidChannel(channelIndex)) return;

            var ch = channels[channelIndex];
            ch.PlayId++; // 世代更新：これで古いタスクの後処理を無効化
            ch.State = VoicePlaybackState.Idle;

            ch.CancelTokenSource.Cancel();
            ch.CancelTokenSource.Dispose();
            ch.CancelTokenSource = new CancellationTokenSource();

            ch.Source.Stop();
            ch.Source.clip = null;
        }

        public async UniTask PlayVoiceAsync(AudioClip clip, AudioOrder order)
        {
            if (!IsValidChannel(order.ChannelIndex))
            {
                Debug.LogError($"Invalid channel index: {order.ChannelIndex}");
                return;
            }

            var ch = channels[order.ChannelIndex];

            // 既存の再生を中断し、世代ID を更新する
            ch.PlayId++;
            var myPlayId = ch.PlayId;

            ch.CancelTokenSource.Cancel();
            ch.CancelTokenSource.Dispose();
            ch.CancelTokenSource = new CancellationTokenSource();
            var token = ch.CancelTokenSource.Token;

            ch.Source.Stop();
            ch.Source.clip = clip;
            ch.Source.volume = order.Volume;
            ch.Source.panStereo = order.Pan;

            try
            {
                if (order.Delay > 0f)
                {
                    ch.State = VoicePlaybackState.Delaying;
                    await UniTask.Delay(TimeSpan.FromSeconds(order.Delay), cancellationToken: token);
                }

                // Delay中に別の再生が入ったら古い処理なので撤退
                if (ch.PlayId != myPlayId || token.IsCancellationRequested)
                {
                    return;
                }

                ch.State = VoicePlaybackState.Playing;
                ch.Source.Play();

                await UniTask.WaitUntil(() => !ch.Source.isPlaying, cancellationToken: token);

                // 再生完了（中断されていない＆最新世代のみ）
                if (!token.IsCancellationRequested && ch.PlayId == myPlayId)
                {
                    OnPlaybackCompleted?.Invoke(order.ChannelIndex);
                }
            }
            catch (OperationCanceledException)
            {
                // 中断時は何もしない
            }
            finally
            {
                // finally で最新世代のみ Idle に戻すのも
                if (ch.PlayId == myPlayId)
                {
                    ch.State = VoicePlaybackState.Idle;
                }
            }
        }

        private bool IsValidChannel(int channelIndex)
            => channelIndex >= 0 && channelIndex < channels.Count;

        private class VoiceChannel
        {
            public AudioSource Source;
            public CancellationTokenSource CancelTokenSource;
            public VoicePlaybackState State;
            public int PlayId;
        }
    }
}