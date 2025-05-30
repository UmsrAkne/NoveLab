using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Audio
{
    public class BgvPlayer : MonoBehaviour
    {
        [SerializeField] private int channelCount = 4;

        private readonly List<List<AudioClip>> playlists = new ();
        private AudioSource[] sources;
        private AudioOrder[] orderCaches;
        private bool[] playlistUpdatedFlags;
        private CancellationTokenSource[] cancelTokens;

        private void Awake()
        {
            sources = new AudioSource[channelCount];
            orderCaches = new AudioOrder[channelCount];
            cancelTokens = new CancellationTokenSource[channelCount];
            playlistUpdatedFlags = new bool[channelCount];

            for (var i = 0; i < channelCount; i++)
            {
                var go = new GameObject($"BgvChannel_{i}");
                go.transform.SetParent(transform);
                sources[i] = go.AddComponent<AudioSource>();

                playlists.Add(new List<AudioClip>());
                cancelTokens[i] = new CancellationTokenSource();
            }
        }

        public void Play(int channelIndex)
        {
            if (!IsValidChannel(channelIndex))
            {
                return;
            }

            if (sources[channelIndex].isPlaying && !playlistUpdatedFlags[channelIndex])
            {
                return;
            }

            // プレイリストが空なら再生しない
            if (playlists[channelIndex].Count == 0)
            {
                Stop(channelIndex);
                return;
            }

            // 既存ループを止めて、再スタート
            Stop(channelIndex);

            var tokenSource = new CancellationTokenSource();
            cancelTokens[channelIndex] = tokenSource;

            var order = orderCaches[channelIndex] ?? new AudioOrder() { ChannelIndex = channelIndex, };
            _ = LoopPlayAsync(order.ChannelIndex, order.Volume, order.Pan, tokenSource.Token);
            playlistUpdatedFlags[channelIndex] = false;
        }

        public void PrepareBgVoiceClips(AudioOrder audioOrder, List<AudioClip> clips)
        {
            if (!IsValidChannel(audioOrder.ChannelIndex))
            {
                return;
            }

            Stop(audioOrder.ChannelIndex);
            playlists[audioOrder.ChannelIndex].Clear();
            playlists[audioOrder.ChannelIndex].AddRange(clips);
            orderCaches[audioOrder.ChannelIndex] = audioOrder;

            playlistUpdatedFlags[audioOrder.ChannelIndex] = true;
        }

        public void Stop(int channelIndex)
        {
            if (channelIndex < 0 || channelIndex >= sources.Length)
            {
                return;
            }

            sources[channelIndex].Stop();
        }

        private async UniTaskVoid LoopPlayAsync(int channelIndex, float volume, float pan, CancellationToken token)
        {
            var source = sources[channelIndex];

            while (!token.IsCancellationRequested)
            {
                // シャッフルして順番に再生
                var shuffled = new List<AudioClip>(playlists[channelIndex]);
                Shuffle(shuffled);

                foreach (var clip in shuffled)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    source.Stop();
                    source.clip = clip;
                    source.volume = volume;
                    source.panStereo = pan;
                    source.Play();

                    try
                    {
                        await UniTask.WaitUntil(() => !source.isPlaying, cancellationToken: token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }
        }

        private void Shuffle<T>(List<T> list)
        {
            var rng = new System.Random();
            for (var i = list.Count - 1; i > 0; i--)
            {
                var swapIndex = rng.Next(i + 1);
                (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
            }
        }

        private bool IsValidChannel(int index) => index >= 0 && index < channelCount;
    }
}