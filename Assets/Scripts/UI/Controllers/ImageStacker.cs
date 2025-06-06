using System.Collections.Generic;
using Core;
using UI.Animations;
using UnityEngine;

namespace UI.Controllers
{
    public class ImageStacker : MonoBehaviour
    {
        [SerializeField]
        private int maxCount = 3;

        private readonly List<IDisplayImage> activeImages = new();

        [SerializeField]
        private Transform imageParent;

        public void AddImage(IDisplayImage newImage)
        {
            var imageGameObject = newImage.GameObject;

            // 親に追加（UIなら Canvasの子に）
            imageGameObject.transform.SetParent(imageParent, false);

            // 一番手前にする
            imageGameObject.transform.SetAsLastSibling();

            activeImages.Add(newImage);

            var fadeIn = new FadeIn(newImage) { Duration = 0.2f, };
            var slide = new Slide(newImage) { Duration = 0.3f, Angle = 180f, Distance = 30f, };

            newImage.RegisterAnimation(nameof(fadeIn), fadeIn);
            newImage.RegisterAnimation(nameof(Slide), slide);
            newImage.PlayAnimations();

            // 超えたら古いやつ削除
            if (activeImages.Count > maxCount)
            {
                var oldest = activeImages[0];
                Destroy(oldest.GameObject);
                activeImages.RemoveAt(0);
            }
        }
    }
}