using System;
using UnityEngine;

namespace Audio
{
    public class SimpleAudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// 同時発音数の上限値。
        /// 一般的なサウンドの重なりや、主要な演出が競合しない必要十分なリソース量として "4" を定義。
        /// </summary>
        private const int ChannelCount = 4;

        private AudioChannel[] channels;

        private void Awake()
        {
            channels = new AudioChannel[ChannelCount];
            for (var i = 0; i < ChannelCount; i++)
            {
                // チャンネルごとに独立した AudioSource を生成
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialize = false; // 2Dパン制御のため

                channels[i] = new AudioChannel(i, source);
            }
        }

        private void Update()
        {
            // 毎フレーム全チャンネルの状態を監視（非同期を使わないポーリング）
            for (var i = 0; i < ChannelCount; i++)
            {
                channels[i].UpdateCheck();
            }
        }

        /// <summary>
        /// 音声を再生します。Play()を呼んだ直後から IsPlaying は true を返します。
        /// </summary>
        public void Play(int index, AudioClip clip, float vol = 1f, float pan = 0f)
        {
            if (IsValidIndex(index))
            {
                channels[index].Play(clip, vol, pan);
            }
        }

        /// <summary>
        /// 音声を停止します。停止時にも終了イベントが送出されます。
        /// </summary>
        public void Stop(int index)
        {
            if (IsValidIndex(index))
            {
                channels[index].Stop();
            }
        }

        /// <summary>
        /// 現在再生中かどうかを返します（Play直後から true になります）
        /// </summary>
        public bool IsPlaying(int index)
        {
            return IsValidIndex(index) && channels[index].IsPlayingInternal;
        }

        public void SetVolume(int index, float vol)
        {
            if (IsValidIndex(index))
            {
                channels[index].Source.volume = vol;
            }
        }

        public void SetPan(int index, float pan)
        {
            if (IsValidIndex(index))
            {
                channels[index].Source.panStereo = pan;
            }
        }

        /// <summary>
        /// イベントを登録します。index 0~3 を指定してください。
        /// </summary>
        public void RegisterEvents(int index, Action<int> onStart, Action<int> onFinish)
        {
            if (!IsValidIndex(index))
            {
                return;
            }

            channels[index].OnStarted = onStart;
            channels[index].OnFinished = onFinish;
        }

        private bool IsValidIndex(int i)
        {
            return i is >= 0 and < ChannelCount;
        }
    }
}