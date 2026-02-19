using System;
using System.Collections.Generic;
using Core;
using ScenarioModel;
using UI.Controllers;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI.Images
{
    public class ImageSetFactory : IImageSetFactory, ITextureProvider
    {
        private readonly GameObject imageSetPrefab;
        private readonly IReadOnlyDictionary<string, Texture2D> images;
        private readonly TextureMerger textureMerger;
        private static int orderNumber;

        public ImageSetFactory(
            GameObject imageSetPrefab, IReadOnlyDictionary<string, Texture2D> images, TextureMerger textureMerger)
        {
            this.imageSetPrefab = imageSetPrefab
                ? imageSetPrefab
                : throw new ArgumentNullException(nameof(imageSetPrefab));

            this.images = images ?? throw new ArgumentNullException(nameof(images));
            this.textureMerger = textureMerger;
        }

        public void CreateAndAdd(IImageContainer imageStacker, ImageOrder order)
        {
            var stacker = imageStacker as ImageStacker;
            if (stacker == null)
            {
                throw new ArgumentNullException(nameof(stacker));
            }

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            // 画像名からTextureを解決（不足分は null）
            var tex1 = Resolve(order.ImageNames, 0);
            var tex2 = Resolve(order.ImageNames, 1);
            var tex3 = Resolve(order.ImageNames, 2);

            var mergedTexture = textureMerger.Merge(tex1, tex2, tex3);

            // 生成（シーン上のオブジェクトでもInstantiate可能）
            var go = UnityEngine.Object.Instantiate(imageSetPrefab, stacker.transform);
            var imageSet = go.GetComponent<ImageSet>();
            imageSet.TextureProvider = this;
            imageSet.BaseImageName = order.A;
            imageSet.SetBasePosition(new Vector2(order.X, order.Y));

            if (!imageSet)
            {
                throw new InvalidOperationException("ImageSet prefab に ImageSet コンポーネントが見つかりません。");
            }

            imageSet.SetTexture(mergedTexture);

            // 位置・スケール反映（RectTransform 前提）
            if (go.transform is RectTransform rt)
            {
                rt.anchoredPosition = new Vector2(order.X, order.Y);
                rt.localScale = Vector3.one * order.Scale;
            }
            else
            {
                go.transform.localPosition = new Vector3(order.X, order.Y, 0f);
                go.transform.localScale = Vector3.one * order.Scale;
            }

            if (go.TryGetComponent<SortingGroup>(out var sortingGroup))
            {
                sortingGroup.sortingOrder = orderNumber++;
                orderNumber++;
            }

            // 追加（上書きロジックがあるならここで分岐：ReplaceImage 等）
            // 例:
            // if (order.IsOverwrite) stacker.ReplaceImage(order.TargetLayerIndex, imageSet);
            // else                   stacker.AddImage(imageSet);
            stacker.AddImage(imageSet);
        }

        public List<Texture2D> GetTextures(ImageOrder order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var list = new List<Texture2D>();

            foreach (var orderImageName in order.ImageNames)
            {
                images.TryGetValue(orderImageName, out var tex);
                list.Add(tex);
            }

            return list;
        }

        private Texture2D Resolve(IReadOnlyList<string> names, int index)
        {
            if (names == null || index >= names.Count)
            {
                return null;
            }

            var key = names[index];
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (images.TryGetValue(key.ToLower(), out var tex))
            {
                return tex;
            }

            Debug.LogWarning($"Image '{key}' not found in dictionary.");
            return null;
        }

        public Texture2D GetTexture(ImageOrder imageOrder)
        {
            // 画像名からTextureを解決（不足分は null）
            var tex1 = Resolve(imageOrder.ImageNames, 0);
            var tex2 = Resolve(imageOrder.ImageNames, 1);
            var tex3 = Resolve(imageOrder.ImageNames, 2);

            return textureMerger.Merge(tex1, tex2, tex3);
        }
    }
}