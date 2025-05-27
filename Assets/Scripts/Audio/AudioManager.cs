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

        public async UniTask PlayAsync(AudioOrder order)
        {

            if (order.AudioType == AudioType.Bgm)
            {
                var clip = await audioLoader.LoadAudioClipAsync(order.FileName);
                await bgmPlayer.PlayBgmAsync(clip, fadeDuration: 1f);
            }

            // 他の AudioType に応じた処理もここに追加予定
        }
    }
}