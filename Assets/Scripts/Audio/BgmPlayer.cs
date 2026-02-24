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
            if (currentFadeTask.Status == UniTaskStatus.Pending)
            {
                await currentFadeTask.SuppressCancellationThrow();
            }

            // 新しいタスクを代入
            currentFadeTask = CrossFadeAsync(newClip, fadeDuration);
            await currentFadeTask;
            currentFadeTask = default;
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