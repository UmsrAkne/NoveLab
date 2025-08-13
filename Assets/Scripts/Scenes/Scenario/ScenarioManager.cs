using System.Collections.Generic;
using System.Linq;
using Audio;
using Core;
using Cysharp.Threading.Tasks;
using Loaders;
using ScenarioModel;
using Scenes.Loading;
using TMPro;
using UI.Animations;
using UI.Controllers;
using UI.Images;
using UI.TypeWriter;
using UnityEngine;
using Utils;

namespace Scenes.Scenario
{
    public class ScenarioManager : MonoBehaviour
    {
        public static GlobalScenarioContext GlobalScenarioContext = new ();

        private TypewriterEngine typewriterEngine;
        private readonly List<ScenarioEntry> scenarioEntries = new ();
        private int scenarioIndex;
        private List<Texture2D> textures = new ();
        private GlobalScenarioContext scenarioContext;
        private IImageSetFactory imageSetFactory;
        private AnimationCompiler animationCompiler;
        private ScenarioEntry lastExecution;

        [SerializeField]
        private GameObject imageSetPrefab;

        [SerializeField]
        private List<ImageStacker> imageStackers = new ();

        [SerializeField]
        private TextMeshProUGUI textMeshPro;

        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private AudioLoader audioLoader;

        [SerializeField]
        private LogDumper logDumper;

        private void Start()
        {
            audioManager.LoadDebugBgm().Forget();
            audioManager.LoadDebugVoice().Forget();

            scenarioContext = LoadingManager.GlobalScenarioContext;
            imageSetFactory = new ImageSetFactory(imageSetPrefab, scenarioContext.Images);
            animationCompiler =
                new AnimationCompiler(imageStackers.First().GetFront(), imageStackers.First(), imageSetFactory);

            logDumper.Log($"Loaded from: {scenarioContext.ScenarioDirectoryPath}");
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
                PlayAnimation();
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

        private void PlayAnimation()
        {
            if (scenarioIndex >= scenarioEntries.Count)
            {
                return;
            }

            var scenario = scenarioEntries[scenarioIndex];

            if (scenario == lastExecution)
            {
                return;
            }

            lastExecution = scenario;

            var animationSpec = scenarioContext.Scenarios[scenarioIndex].Animations.FirstOrDefault();
            var a = animationCompiler.Compile(animationSpec);
            if (a != null)
            {
                RegisterSmart(a);
            }
        }

        private void RegisterSmart(IUIAnimation anim)
        {
            if (anim is ImageAddAnimation)
            {
                // ImageAddAnimation の場合だけ特殊な処理
                anim.Start();
            }
            else
            {
                // その他の通常アニメーション
                imageStackers.FirstOrDefault()?.GetFront()?.RegisterAnimation(anim.GetType().Name, anim);
            }
        }
    }
}