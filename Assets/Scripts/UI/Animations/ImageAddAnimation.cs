using System;
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
        private IUIAnimation fadeInAnimation;
        private IImageSetFactory imageSetFactory;

        public ImageAddAnimation(IImageContainer stacker = null, IImageSetFactory imageSetFactory = null)
        {
            imageStacker = stacker;
            this.imageSetFactory = imageSetFactory;
        }

        public bool IsPlaying { get; private set; }

        public string GroupId { get; set; }

        public event Action OnCompleted;

        public ImageOrder ImageOrder { get; set; }

        public void Start()
        {
            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;

            imageSetFactory.CreateAndAdd(imageStacker, ImageOrder);

            // 追加された画像に登録されたアニメーションを取得
            // この部分の実装はImageStackerのAddImageメソッド内で
            // fadeInをpublicにするか、IDisplayImageにアニメーション取得メソッドを追加する必要があるかもしれません。

            fadeInAnimation = new FadeIn(imageStacker.GetFront())
            {
                Duration = ImageOrder.Duration,
                Delay = ImageOrder.Delay,
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