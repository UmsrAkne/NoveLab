using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace UI.Controllers
{
    public class ImageStacker : MonoBehaviour, IImageContainer
    {
        [SerializeField]
        private int maxCount = 3;

        [SerializeField]
        private int baseSortingOrder;

        private readonly List<IDisplayImage> activeImages = new();

        [SerializeField]
        private Transform imageParent;

        [SerializeField]
        private Transform effectLayer;

        private Sprite whiteSprite;

        private SpriteRenderer effectRenderer;

        private void Awake()
        {
            var go = new GameObject("WhiteEffect");
            go.transform.SetParent(effectLayer, false);

            effectRenderer = go.AddComponent<SpriteRenderer>();
            effectRenderer.sprite = CreateWhiteSprite();
            effectRenderer.color = new Color(1, 1, 1, 0); // デフォルトは透明
      }

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

            effectLayer.SetAsFirstSibling();
        }

        public IDisplayImage GetFront()
        {
            return activeImages.LastOrDefault();
        }

        private Sprite CreateWhiteSprite()
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();

            return Sprite.Create(
                tex,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f),
                1f);
        }
    }
}