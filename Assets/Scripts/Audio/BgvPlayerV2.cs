using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class BgvPlayerV2 : MonoBehaviour
    {
        [SerializeField] private SimpleAudioPlayer linkedPlayer;
        [SerializeField] private int targetChannelIndex;

        private AudioSource bgvSource;
        private List<AudioClip> playlist = new ();
        private int currentClipIndex = -1;

        private float maxVolume = 1.0f;
        private Coroutine fadeCoroutine;
        private bool isRunning;

        private void Awake()
        {
            bgvSource = GetComponent<AudioSource>();
            bgvSource.loop = false; // ループは自前で制御（プレイリスト再生のため）
            bgvSource.playOnAwake = false;
        }

        private void Start()
        {
            // SimpleAudioPlayer のイベントに紐付け
            if (linkedPlayer != null)
            {
                linkedPlayer.RegisterEvents(targetChannelIndex, OnParentStarted, OnParentFinished);
            }
        }

        /// <summary>
        /// BGVの再生を開始します
        /// </summary>
        public void Play(AudioClip[] clips, float vol = 1.0f, float pan = 0f)
        {
            if (clips == null || clips.Length == 0)
            {
                return;
            }

            playlist = clips.ToList();
            maxVolume = vol;
            bgvSource.panStereo = pan;
            isRunning = true;

            ShufflePlaylist();
            PlayNext();

            // 親が再生中なら音量0、そうでなければフェードイン
            if (linkedPlayer.IsPlaying(targetChannelIndex))
            {
                bgvSource.volume = 0f;
            }
            else
            {
                StartFade(0f, maxVolume);
            }
        }

        public void Stop()
        {
            isRunning = false;
            bgvSource.Stop();
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            // 再生終了チェック（プレイリストの自動遷移）
            if (!bgvSource.isPlaying)
            {
                PlayNext();
            }
        }

        private void PlayNext()
        {
            currentClipIndex++;
            if (currentClipIndex >= playlist.Count)
            {
                ShufflePlaylist();
                currentClipIndex = 0;
            }

            bgvSource.clip = playlist[currentClipIndex];
            bgvSource.Play();
        }

        private void ShufflePlaylist()
        {
            playlist = playlist.OrderBy(_ => Guid.NewGuid()).ToList();
        }

        // --- イベントハンドラ ---

        private void OnParentStarted(int index)
        {
            if (!isRunning)
            {
                return;
            }

            // 親の再生開始に伴い、最短時間（0.1s）でフェードアウト。
            // 直接 volume = 0 を代入せず StartFade を経由させる。
            // これによってフェード用コルーチンの停止制御や内部状態の整合性を担保している。
            StartFade(bgvSource.volume, 0f, 0.1f);
        }

        private void OnParentFinished(int index)
        {
            if (!isRunning)
            {
                return;
            }

            // 親が終わったらフェードイン
            StartFade(bgvSource.volume, maxVolume);
        }

        // --- フェード制御 ---

        private void StartFade(float from, float to, float fadeDuration = 1.0f)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeRoutine(from, to, fadeDuration));
        }

        private IEnumerator FadeRoutine(float from, float to, float fadeDuration = 1.0f)
        {
            float elapsed = 0;
            bgvSource.volume = from;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                bgvSource.volume = Mathf.Lerp(from, to, elapsed / fadeDuration);
                yield return null;
            }

            bgvSource.volume = to;
            fadeCoroutine = null;
        }
    }
}