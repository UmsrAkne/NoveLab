using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using ScenarioModel;
using UnityEngine;
using AudioType = ScenarioModel.AudioType;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private BgmPlayer bgmPlayer;

        [SerializeField]
        private VoicePlayer voicePlayer;

        [SerializeField]
        private BgvPlayer bgvPlayer;

        [SerializeField]
        private SePlayer sePlayer;

        public GlobalScenarioContext ScenarioContext { private get; set; }

        public async UniTask PlayAsync(AudioOrder order)
        {
            if (order.AudioType == AudioType.Bgm)
            {
                ScenarioContext.BGMs.TryGetValue(order.FileName, out var clip);
                await bgmPlayer.PlayBgmAsync(clip, fadeDuration: 1f);
            }

            if (order.AudioType == AudioType.Voice)
            {
                ScenarioContext.Voices.TryGetValue(order.FileName, out var clip);
                await voicePlayer.PlayVoiceAsync(clip, order);
                bgvPlayer.FadeOutVolume(order.ChannelIndex, 0, 0.25f);
            }

            if (order.AudioType == AudioType.Bgv)
            {
                var clips = order.FileNames
                    .Select(n => ScenarioContext.Bgvs.GetValueOrDefault(n))
                    .ToList();

                bgvPlayer.PrepareBgVoiceClips(order, clips);
            }

            if (order.AudioType == AudioType.Se)
            {
                ScenarioContext.Ses.TryGetValue(order.FileName, out var clip);
                sePlayer.PlaySe(clip, order);
            }

            // 他の AudioType に応じた処理もここに追加予定
        }

        private void Awake()
        {
            voicePlayer.OnPlaybackCompleted += OnVoicePlaybackCompleted;
        }

        private void OnVoicePlaybackCompleted(int channel)
        {
            bgvPlayer.Play(channel);
            bgvPlayer.FadeInVolume(channel, 1f, 0.4f);
        }
    }
}