using System.Collections.Generic;
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
        private readonly List<ScenarioEntry> scenarioEntries = new ();
        private int scenarioIndex;

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
                if (scenarioIndex >= scenarioEntries.Count)
                {
                    return;
                }

                if (typewriterEngine.IsFinished)
                {
                    typewriterEngine.SetText(scenarioEntries[scenarioIndex].Text);
                    scenarioIndex++;
                }
                else
                {
                    typewriterEngine.ShowFullText();
                }
            }
        }

        private async UniTaskVoid LoadSound(string path)
        {
            await audioLoader.LoadAudioClipAsync(path);
            var clip = audioLoader.GetCachedClip(path);
            await bgmPlayer.PlayBgmAsync(clip, 5f);
        }
    }
}