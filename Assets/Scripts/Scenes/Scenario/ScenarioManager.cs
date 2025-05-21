using Audio;
using Cysharp.Threading.Tasks;
using Loaders;
using TMPro;
using UI.TypeWriter;
using UnityEngine;

namespace Scenes.Scenario
{
    public class ScenarioManager : MonoBehaviour
    {
        private TypewriterEngine typewriterEngine;

        [SerializeField]
        private AudioLoader audioLoader;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private BgmPlayer bgmPlayer;

        [SerializeField]
        private TextMeshProUGUI textMeshPro;

        private void Start()
        {
            var path = @"C:\Users\Public\testData\sounds\list2\music.ogg";
            Debug.Log(path);
            LoadSound(path).Forget();

            typewriterEngine.SetText("Dummy Text. Dummy Text. Dummy Text. Dummy Text. Dummy Text. Dummy Text. ");
        }

        private void Awake()
        {
            typewriterEngine = new TypewriterEngine(new TextDisplayTarget(textMeshPro));
        }

        private void Update()
        {
            typewriterEngine.Update(Time.deltaTime);
        }

        private async UniTaskVoid LoadSound(string path)
        {
            await audioLoader.LoadAudioClipAsync(path);
            var clip = audioLoader.GetCachedClip(path);
            await bgmPlayer.PlayBgmAsync(clip, 5f);
        }
    }
}