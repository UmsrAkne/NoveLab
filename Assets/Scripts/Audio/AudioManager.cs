using System.IO;
using Cysharp.Threading.Tasks;
using Loaders;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioLoader audioLoader;

        [SerializeField]
        private BgmPlayer bgmPlayer;

        [SerializeField]
        private VoicePlayer voicePlayer;

        [SerializeField]
        private BgvPlayer bgvPlayer;

        public async UniTask PlayAsync(AudioOrder order)
        {
            if (order.AudioType == AudioType.Bgm)
            {
                var clip = audioLoader.GetCachedClip(order.FileName);
                Debug.Log("clipName = " + clip.name);
                await bgmPlayer.PlayBgmAsync(clip, fadeDuration: 1f);
            }

            if (order.AudioType == AudioType.Voice)
            {
                var clip = audioLoader.GetCachedClip(order.FileName);
                await voicePlayer.PlayVoiceAsync(clip, order);
            }

            if (order.AudioType == AudioType.Bgv)
            {
                // Bgv 指定の場合の処理
            }

            // 他の AudioType に応じた処理もここに追加予定
        }

        public async UniTaskVoid LoadDebugBgm()
        {
            var path = @"C:\Users\Public\testData\sounds\list2\music.ogg";
            await audioLoader.LoadAudioClipAsync(path);
            var order = new AudioOrder()
            {
                FileName = Path.GetFileName(path),
                AudioType = AudioType.Bgm,
            };

            await PlayAsync(order);
        }

        public async UniTaskVoid LoadDebugVoice()
        {
            var path = @"C:\Users\Public\testData\sounds\list3\test_message1.ogg";
            await audioLoader.LoadAudioClipAsync(path);
            var order = new AudioOrder()
            {
                FileName = Path.GetFileName(path),
                AudioType = AudioType.Voice,
            };

            await PlayAsync(order);
        }

        private void Awake()
        {
            voicePlayer.OnPlaybackCompleted += OnVoicePlaybackCompleted;
        }

        private void OnVoicePlaybackCompleted(int channel)
        {
            // ここに voicePlayer の再生終了時の処理を書く。
        }
    }
}