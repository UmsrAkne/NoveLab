using System;
using System.Collections.Generic;
using Core;
using ScenarioModel;

namespace UI.Animations
{
// ImageStackerにIUIAnimationを実装するのではなく、
// ImageStackerを内部で利用するIUIAnimationを作成します。

// 新しく作成するクラス
    public class ImageAddAnimation : IUIAnimation
    {
        private readonly IImageContainer imageStacker;
        private readonly IDisplayImage imageToAdd;
        private IUIAnimation fadeInAnimation;
        private IImageSetFactory imageSetFactory;

        public ImageAddAnimation(IDisplayImage image, IImageContainer stacker = null, IImageSetFactory imageSetFactory = null)
        {
            imageStacker = stacker;
            imageToAdd = image;
            this.imageSetFactory = imageSetFactory;
        }

        public bool IsPlaying { get; private set; }

        public string GroupId { get; set; }

        public event Action OnCompleted;

        public string A { get; set; } = string.Empty;

        public void Start()
        {
            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;

            imageSetFactory.CreateAndAdd(imageStacker, new ImageOrder { ImageNames = new List<string> { A, }, });

            // 追加された画像に登録されたアニメーションを取得
            // この部分の実装はImageStackerのAddImageメソッド内で
            // fadeInをpublicにするか、IDisplayImageにアニメーション取得メソッドを追加する必要があるかもしれません。
            // 例：fadeInAnimation = imageToAdd.GetAnimation<FadeIn>();

            fadeInAnimation = new FadeIn(imageStacker.GetFront())
            {
                Duration = 1.5f,
            };

            fadeInAnimation.OnCompleted += OnInternalAnimationCompleted;
            fadeInAnimation.Start();
        }

        private void OnInternalAnimationCompleted()
        {
            // 内部アニメーションが完了したら、このクラスの完了イベントを発火
            fadeInAnimation.OnCompleted -= OnInternalAnimationCompleted;
            IsPlaying = false;
            OnCompleted?.Invoke();
        }

        public void Stop()
        {
            // 停止ロジック
            if (fadeInAnimation is { IsPlaying: true, })
            {
                fadeInAnimation.Stop();
            }

            IsPlaying = false;
        }
    }
}