using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class BgvPlayer : MonoBehaviour
    {
        [SerializeField] private int channelCount = 4;

        private AudioSource[] sources;
        private List<List<AudioClip>> playlists = new ();

        private void Awake()
        {
            sources = new AudioSource[channelCount];
            for (var i = 0; i < channelCount; i++)
            {
                var go = new GameObject($"BgvChannel_{i}");
                go.transform.SetParent(transform);
                sources[i] = go.AddComponent<AudioSource>();
            }
        }

        public void Play(AudioClip clip, AudioOrder order)
        {
            if (order.ChannelIndex < 0 || order.ChannelIndex >= sources.Length)
            {
                return;
            }

            var source = sources[order.ChannelIndex];
            source.Stop(); // 再生中なら止める
            source.clip = clip;
            source.volume = order.Volume;
            source.panStereo = order.Pan;
            source.Play();
        }

        public void PrepareBgVoiceClips(int channelIndex, List<AudioClip> clip)
        {
            playlists[channelIndex].Clear();
            playlists[channelIndex].AddRange(clip);
        }

        public void Stop(int channelIndex)
        {
            if (channelIndex < 0 || channelIndex >= sources.Length)
            {
                return;
            }

            sources[channelIndex].Stop();
        }
    }
}