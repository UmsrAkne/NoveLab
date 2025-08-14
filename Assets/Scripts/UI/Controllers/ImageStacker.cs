using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace UI.Controllers
{
    public class ImageStacker : MonoBehaviour, IImageAdder
    {
        [SerializeField]
        private int maxCount = 3;

        [SerializeField]
        private int baseSortingOrder;

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

            var localMax = activeImages
                .Select(img => img?.SortingOrder ?? baseSortingOrder)
                .DefaultIfEmpty(baseSortingOrder)
                .Max();

            newImage.SortingOrder = localMax + 1;

            activeImages.Add(newImage);

            // 超えたら古いやつ削除
            if (activeImages.Count > maxCount)
            {
                var oldest = activeImages[0];
                Destroy(oldest.GameObject);
                activeImages.RemoveAt(0);
            }
        }

        public IDisplayImage GetFront()
        {
            return activeImages.LastOrDefault();
        }
    }
}