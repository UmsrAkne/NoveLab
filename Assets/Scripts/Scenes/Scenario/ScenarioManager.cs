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
        private const float DefaultWidth = 1280f;

        private TypewriterEngine typewriterEngine;
        private int scenarioIndex;
        private GlobalScenarioContext scenarioContext;
        private IImageSetFactory imageSetFactory;
        private AnimationCompiler animationCompiler;
        private ScenarioEntry lastExecution;

        [SerializeField]
        private GameObject imageSetPrefab;

        [SerializeField]
        private List<ImageStacker> imageStackers = new ();

        [SerializeField]
        private TextureMerger textureMerger;

        [SerializeField]
        private TextMeshProUGUI textMeshPro;

        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private AudioLoader audioLoader;

        [SerializeField]
        private LogDumper logDumper;

        [SerializeField]
        private RectTransform leftFrame;

        [SerializeField]
        private RectTransform rightFrame;

        private void Start()
        {
            scenarioContext = LoadingManager.GlobalScenarioContext;
            SetVisibleWidth(scenarioContext.SceneSetting.WindowWidth);
            imageSetFactory = new ImageSetFactory(imageSetPrefab, scenarioContext.Images, textureMerger);
            animationCompiler =
                new AnimationCompiler(imageStackers.First(), imageSetFactory);

            audioManager.ScenarioContext = scenarioContext;
            audioManager.LogDumper = logDumper;

            audioManager.PlayAsync(scenarioContext.SceneSetting.BgmOrder).Forget();

            logDumper.Log($"Loaded from: {scenarioContext.ScenarioDirectoryPath}");
        }

        private void Awake()
        {
            AspectUtil.SetAspect(Camera.main);
            typewriterEngine = new TypewriterEngine(new TextDisplayTarget(textMeshPro));
        }

        private void Update()
        {
            typewriterEngine.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                var scenario = GetScenario();
                WriteText();

                if (!typewriterEngine.IsFinished)
                {
                    PlayAnimation(scenario);
                    PlayAudio(scenario);
                }
            }
        }

        private ScenarioEntry GetScenario()
        {
            if (scenarioIndex >= scenarioContext.Scenarios.Count)
            {
                return null;
            }

            return scenarioContext.Scenarios[scenarioIndex];
        }

        private void PlayAudio(ScenarioEntry scenarioEntry)
        {
            var audioOrders = scenarioEntry.VoiceOrders
                .Concat(scenarioEntry.SeOrders)
                .Concat(scenarioEntry.BgvOrders).ToList();

            if (scenarioEntry.BgmOrder != null)
            {
                audioOrders.Add(scenarioEntry.BgmOrder);
            }

            foreach (var audioOrder in audioOrders)
            {
                audioManager.PlayAsync(audioOrder).Forget();
            }
        }

        private void WriteText()
        {
            if (scenarioIndex >= scenarioContext.Scenarios.Count)
            {
                return;
            }

            if (typewriterEngine.IsFinished)
            {
                typewriterEngine.SetText(scenarioContext.Scenarios[scenarioIndex]);
                scenarioIndex++;
            }
            else
            {
                typewriterEngine.ShowFullText();
            }
        }

        private void PlayAnimation(ScenarioEntry scenarioEntry)
        {
            if (scenarioEntry == lastExecution)
            {
                return;
            }

            lastExecution = scenarioEntry;

            var animations = scenarioEntry.Animations
                .Select(spec => animationCompiler.Compile(spec)).ToList();

            foreach (var uiAnimation in animations)
            {
                RegisterSmart(uiAnimation);
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
                var stacker = imageStackers[anim.TargetLayerIndex];
                stacker.GetFront()?.RegisterAnimation(anim.GetType().Name, anim);
                anim.Start();
            }
        }

        private void SetVisibleWidth(float targetWidth)
        {
            targetWidth = Mathf.Clamp(targetWidth, 1280f, 1680f);

            var delta = targetWidth - DefaultWidth;
            var offset = delta / 2f;

            // 左は左へ
            leftFrame.anchoredPosition = new Vector2(-offset, leftFrame.anchoredPosition.y);

            // 右は右へ
            rightFrame.anchoredPosition = new Vector2(offset, rightFrame.anchoredPosition.y);
        }
    }
}