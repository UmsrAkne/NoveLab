using System.Collections.Generic;
using Audio;
using Cysharp.Threading.Tasks;
using Loaders;
using TMPro;
using UI.Controllers;
using UI.Images;
using UI.TypeWriter;
using UnityEngine;

namespace Scenes.Scenario
{
    public class ScenarioManager : MonoBehaviour
    {
        private TypewriterEngine typewriterEngine;
        private readonly List<ScenarioEntry> scenarioEntries = new ();
        private int scenarioIndex;
        private List<Texture2D> textures = new ();

        [SerializeField]
        private GameObject imageSetPrefab;

        [SerializeField]
        private ImageStacker imageStacker;

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
            LoadDebugImages().Forget();

            scenarioEntries.Add(new ScenarioEntry() { Text = "Dummy1 Dummy1 Dummy1 Dummy1 Dummy1 Dummy1", });
            scenarioEntries.Add(new ScenarioEntry() { Text = "Dummy2 Dummy2 Dummy2 Dummy2 Dummy2 Dummy2", });
            scenarioEntries.Add(new ScenarioEntry() { Text = "Dummy3 Dummy3 Dummy3 Dummy3 Dummy3 Dummy3", });
        }

        private void Awake()
        {
            typewriterEngine = new TypewriterEngine(new TextDisplayTarget(textMeshPro));
        }

        private void Update()
        {
            typewriterEngine.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                WriteText();
            }
        }

        private void WriteText()
        {
            if (scenarioIndex >= scenarioEntries.Count)
            {
                return;
            }

            if (typewriterEngine.IsFinished)
            {
                typewriterEngine.SetText(scenarioEntries[scenarioIndex]);
                scenarioIndex++;
            }
            else
            {
                typewriterEngine.ShowFullText();
            }
        }

        private async UniTaskVoid LoadSound(string path)
        {
            await audioLoader.LoadAudioClipAsync(path);
            var clip = audioLoader.GetCachedClip(path);
            await bgmPlayer.PlayBgmAsync(clip, 5f);
        }

        private async UniTaskVoid LoadDebugImages()
        {
            Debug.Log("This runs ONLY in the editor during Play mode!");

            var i1 = await ImageLoader.LoadTexture(@"C:\Users\Public\testData\images\A0101.png");
            textures.Add(i1);

            var i2 = await ImageLoader.LoadTexture(@"C:\Users\Public\testData\images\B0101.png");
            textures.Add(i2);

            var i3 = await ImageLoader.LoadTexture(@"C:\Users\Public\testData\images\C0101.png");
            textures.Add(i3);

            var imageSetGameObject = Instantiate(imageSetPrefab, imageStacker.transform);
            var imageSet = imageSetGameObject.GetComponent<ImageSet>();
            imageSet.SetTextures(textures[0], textures[1], textures[2]);
            imageStacker.AddImage(imageSet);
        }
    }
}