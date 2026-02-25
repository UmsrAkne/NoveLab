using Cysharp.Threading.Tasks;
using UnityEngine;
using ScenarioModel;

namespace Audio
{
    public class BgmPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private UniTask currentFadeTask;

        public async UniTask PlayBgmAsync(AudioClip newClip, AudioOrder order, float fadeDuration = 1f)
        {
            if (currentFadeTask.Status == UniTaskStatus.Pending)
            {
                await currentFadeTask.SuppressCancellationThrow();
            }

            var targetVolume = order != null ? Mathf.Clamp01(order.Volume) : 1f;

            // 新しいタスクを代入
            currentFadeTask = CrossFadeAsync(newClip, fadeDuration, targetVolume);
            await currentFadeTask;
            currentFadeTask = default;
        }

        private async UniTask CrossFadeAsync(AudioClip newClip, float duration, float targetVolume)
        {
            const float minVolume = 0f;

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

            // フェードイン（目標音量は order.Volume ）
            await FadeVolumeAsync(audioSource, targetVolume, duration);
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