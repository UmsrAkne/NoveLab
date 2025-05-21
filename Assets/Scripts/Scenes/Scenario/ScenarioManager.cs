using Audio;
using Cysharp.Threading.Tasks;
using Loaders;
using UnityEngine;

namespace Scenes.Scenario
{
    public class ScenarioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioLoader audioLoader;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private BgmPlayer bgmPlayer;

        private void Start()
        {
            var path = @"C:\Users\Public\testData\sounds\list2\music.ogg";
            Debug.Log(path);
            LoadSound(path).Forget();
        }

        private async UniTaskVoid LoadSound(string path)
        {
            await audioLoader.LoadAudioClipAsync(path);
            var clip = audioLoader.GetCachedClip(path);
            await bgmPlayer.PlayBgmAsync(clip, 5f);
        }
    }
}