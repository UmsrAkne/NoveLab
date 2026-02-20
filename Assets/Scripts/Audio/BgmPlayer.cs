using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Audio
{
    public class BgmPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private UniTask currentFadeTask;

        public async UniTask PlayBgmAsync(AudioClip newClip, float fadeDuration = 1f)
        {
            // フェード中なら完了まで待つ（同時再生を防止）
            await currentFadeTask.SuppressCancellationThrow();

            currentFadeTask = CrossFadeAsync(newClip, fadeDuration);
            await currentFadeTask;
        }

        private async UniTask CrossFadeAsync(AudioClip newClip, float duration)
        {
            const float minVolume = 0f;
            var maxVolume = 1f;

            if (audioSource.isPlaying)
            {
                // フェードアウト
                await FadeVolumeAsync(audioSource, minVolume, duration);
                audioSource.Stop();
            }

            // 新しいBGM再生
            audioSource.loop = true;
            audioSource.clip = newClip;
            audioSource.volume = 0f;
            audioSource.Play();

            // フェードイン
            await FadeVolumeAsync(audioSource, maxVolume, duration);
        }

        private async UniTask FadeVolumeAsync(AudioSource source, float targetVolume, float duration)
        {
            var startVolume = source.volume;
            var time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                await UniTask.Yield(); // 毎フレーム待つ
            }

            source.volume = targetVolume;
        }
    }
}