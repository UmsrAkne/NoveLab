using ScenarioModel;
using System.Collections;
using UnityEngine;

namespace Audio
{
    public class BgmPlayerV2 : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private Coroutine fadeCoroutine;

        /// <summary>
        /// BGMを再生します（コルーチン版）
        /// </summary>
        public void PlayBgm(AudioClip newClip, AudioOrder order, float fadeDuration = 1f)
        {
            // 実行中のフェードがあれば即座に止める（即切り替えの担保）
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            var targetVolume = order != null ? Mathf.Clamp01(order.Volume) : 1f;

            // 新しいフェード処理を開始
            fadeCoroutine = StartCoroutine(CrossFadeCoroutine(newClip, fadeDuration, targetVolume));
        }

        private IEnumerator CrossFadeCoroutine(AudioClip newClip, float duration, float targetMaxVolume)
        {
            // 1. フェードアウト
            if (audioSource.isPlaying)
            {
                yield return StartCoroutine(FadeVolume(0f, duration));
                audioSource.Stop();
            }

            // 2. クリップの差し替えと再生開始
            audioSource.clip = newClip;
            audioSource.loop = true;
            audioSource.volume = 0f;
            audioSource.Play();

            // 3. フェードイン
            yield return StartCoroutine(FadeVolume(targetMaxVolume, duration));

            fadeCoroutine = null;
        }

        private IEnumerator FadeVolume(float targetVolume, float duration)
        {
            var startVolume = audioSource.volume;
            var time = 0f;

            // duration が 0 の場合は即座に目標値へ
            if (duration <= 0)
            {
                audioSource.volume = targetVolume;
                yield break;
            }

            while (time < duration)
            {
                time += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                yield return null; // 1フレーム待機
            }

            audioSource.volume = targetVolume;
        }

        /// <summary>
        /// 必要に応じてBGMを完全に止める場合に使用
        /// </summary>
        public void StopBgm(float fadeDuration = 1f)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeAndStop(fadeDuration));
        }

        private IEnumerator FadeAndStop(float duration)
        {
            yield return StartCoroutine(FadeVolume(0f, duration));
            audioSource.Stop();
            fadeCoroutine = null;
        }
    }
}