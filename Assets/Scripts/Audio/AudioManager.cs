using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using ScenarioModel;
using UnityEngine;
using Utils;
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

        public LogDumper LogDumper { private get; set; }

        public async UniTask PlayAsync(AudioOrder order)
        {
            if (LogDumper != null)
            {
                var fn = !string.IsNullOrWhiteSpace(order.FileName) ? order.FileName : order.FileNames[0];
                LogDumper.Log($"PlayAsync: Type:{order.AudioType} FileName: {fn}");
            }

            if (order.AudioType == AudioType.Bgm)
            {
                ScenarioContext.BGMs.TryGetValue(order.FileName, out var clip);
                await bgmPlayer.PlayBgmAsync(clip, order, fadeDuration: 1f);
            }

            if (order.AudioType == AudioType.Voice)
            {
                // Voice 再生前に Bgv をフェードアウト
                bgvPlayer.FadeOutVolume(order.ChannelIndex, 0, 0.25f);

                ScenarioContext.Voices.TryGetValue(order.FileName, out var clip);
                await voicePlayer.PlayVoiceAsync(clip, order);
            }

            if (order.AudioType == AudioType.Bgv)
            {
                var clips = order.FileNames
                    .Select(n => ScenarioContext.Bgvs.GetValueOrDefault(n))
                    .ToList();

                bgvPlayer.PrepareBgVoiceClips(order, clips);

                if (!voicePlayer.IsPlaying(order.ChannelIndex))
                {
                    bgvPlayer.Play(order.ChannelIndex);
                    bgvPlayer.FadeInVolume(order.ChannelIndex, 1f, 0.4f);
                }
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
            Debug.Log($"[VOICE END EVENT] ch:{channel} time:{Time.time}");

            bgvPlayer.Play(channel);
            bgvPlayer.FadeInVolume(channel, 1f, 0.4f);
        }
    }
}