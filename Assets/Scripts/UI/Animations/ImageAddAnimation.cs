using System;
using Core;

namespace UI.Animations
{
// ImageStackerにIUIAnimationを実装するのではなく、
// ImageStackerを内部で利用するIUIAnimationを作成します。

// 新しく作成するクラス
    public class ImageAddAnimation : IUIAnimation
    {
        private readonly IImageAdder imageStacker;
        private readonly IDisplayImage imageToAdd;
        private IUIAnimation fadeInAnimation;

        public ImageAddAnimation(IDisplayImage image, IImageAdder stacker = null)
        {
            imageStacker = stacker;
            imageToAdd = image;
        }

        public bool IsPlaying { get; private set; }

        public string GroupId { get; set; }

        public event Action OnCompleted;

        public void Start()
        {
            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;

            // ImageStackerのAddImageメソッドを呼び出す
            imageStacker.AddImage(imageToAdd);

            // 追加された画像に登録されたアニメーションを取得
            // この部分の実装はImageStackerのAddImageメソッド内で
            // fadeInをpublicにするか、IDisplayImageにアニメーション取得メソッドを追加する必要があるかもしれません。
            // 例：fadeInAnimation = imageToAdd.GetAnimation<FadeIn>();

            // 取得したアニメーションの完了イベントを購読
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