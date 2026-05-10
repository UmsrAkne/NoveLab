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
using UnityEngine.SceneManagement;
using Utils;
using AudioType = ScenarioModel.AudioType;

namespace Scenes.Scenario
{
    public class ScenarioManager : MonoBehaviour
    {
        private const float DefaultWidth = 1280f;

        private readonly Dictionary<(int, string), IUIAnimation> runningAnimations = new();
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
        private AudioLoader audioLoader;

        [SerializeField]
        private LogDumper logDumper;

        [SerializeField]
        private RectTransform leftFrame;

        [SerializeField]
        private RectTransform rightFrame;

        [SerializeField]
        private SimpleAudioPlayer simpleAudioPlayer;

        [SerializeField]
        private List<BgvPlayerV2> bgvPlayerV2List;

        [SerializeField]
        private BgmPlayerV2 bgmPlayerV2;

        [SerializeField]
        private SePlayer sePlayer;

        private void Start()
        {
            scenarioContext = LoadingManager.GlobalScenarioContext;
            SetVisibleWidth(scenarioContext.SceneSetting.WindowWidth);
            imageSetFactory = new ImageSetFactory(imageSetPrefab, scenarioContext.Images, textureMerger);
            animationCompiler =
                new AnimationCompiler(imageStackers.First(), imageSetFactory);

            var bgmOrder = scenarioContext.SceneSetting.BgmOrder;
            var bgmClip = scenarioContext.BGMs.GetValueOrDefault(bgmOrder.FileName);
            bgmPlayerV2.PlayBgm(bgmClip, bgmOrder);

            logDumper.Log($"Loaded from: {scenarioContext.ScenarioDirectoryPath}");

            AdvanceScenario();
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

            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
            {
                ReloadScenarioAsync();
            }
        }

        private void AdvanceScenario()
        {
            var scenario = GetScenario();
            WriteText();
            PlayAnimation(scenario);
            PlayAudio(scenario);
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
                var bgmOrder = scenarioEntry.BgmOrder;
                var bgmClip = scenarioContext.BGMs.GetValueOrDefault(bgmOrder.FileName);
                bgmPlayerV2.PlayBgm(bgmClip, bgmOrder);
            }

            foreach (var audioOrder in audioOrders)
            {
                if (audioOrder.AudioType == AudioType.Se)
                {
                    var clip = scenarioContext.Ses.GetValueOrDefault(audioOrder.FileName);
                    sePlayer.PlaySe(clip,audioOrder);
                }
            }

            foreach (var audioOrder in audioOrders)
            {
                // audioManager.PlayAsync(audioOrder).Forget();
                if (audioOrder.AudioType == AudioType.Voice)
                {
                    var clip = scenarioContext.Voices.GetValueOrDefault(audioOrder.FileName);

                    // Voice の再生開始イベントが Play() 呼び出し直後に飛ぶ。
                    // それをキャッチして BgvPlayer が再生を止めるまでの僅かな間、２つのプレイヤーの音声が重なってしまうことある。
                    // これを防止するため、Voice の再生に僅かな遅延を入れている。
                    simpleAudioPlayer.Play(audioOrder.ChannelIndex, clip, audioOrder.Volume, audioOrder.Pan, 200);
                }
            }

            foreach (var audioOrder in audioOrders)
            {
                if (audioOrder.AudioType == AudioType.Bgv)
                {
                    var clips = audioOrder.FileNames
                        .Select(n => scenarioContext.Bgvs.GetValueOrDefault(n))
                        .ToArray();

                    bgvPlayerV2List[audioOrder.ChannelIndex].Play(clips, audioOrder.Volume, audioOrder.Pan);
                }
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
            // キーを作成 (レイヤー番号とアニメーションの型名)
            var key = (anim.TargetLayerIndex, anim.GetType().Name);

            // すでに同じレイヤーで同じ型のアニメーションが動いていれば止める
            if (runningAnimations.TryGetValue(key, out var oldAnim))
            {
                oldAnim.Stop();
                runningAnimations.Remove(key);
            }

            anim.OnCompleted += () =>
            {
                // 自分がまだ辞書に残っているなら削除
                if (runningAnimations.TryGetValue(key, out var current) && current == anim)
                {
                    runningAnimations.Remove(key);
                }
            };

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

            runningAnimations[key] = anim;
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

        private void ReloadScenarioAsync()
        {
            // await sceneFader.FadeOut(1f);

            LoadingManager.GlobalScenarioContext.IsLoaded = false;
            SceneManager.LoadScene("LoadingScene");
        }
    }
}