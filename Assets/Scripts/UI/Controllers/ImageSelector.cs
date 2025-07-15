using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controllers
{
    public class ImageSelector : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        public int SelectedIndex { get; private set; }

        public List<IDisplayImage> DisplayImages { get; set; } = new ();

        public IDisplayImage SelectedItem => DisplayImages[SelectedIndex];

        public void MoveSelection(int direction)
        {
            var newIndex = SelectedIndex + direction;

            // 範囲チェックして切り詰め（またはループさせるならここで）
            newIndex = Mathf.Clamp(newIndex, 0, DisplayImages.Count - 1);

            if (newIndex != SelectedIndex)
            {
                Select(newIndex);
            }

            ScrollToSelected(newIndex);
        }

        private void ScrollToSelected(int index)
        {
            if (index < 0 || index >= DisplayImages.Count)
            {
                return;
            }

            if (!(DisplayImages[index] is MonoBehaviour mb))
            {
                return;
            }

            var selected = mb.GetComponent<RectTransform>();
            var content = scrollRect.content;
            var viewport = scrollRect.viewport;

            // UI更新が済んでないとサイズが正しくないので強制更新
            Canvas.ForceUpdateCanvases();

            // Content内のローカル座標を取得
            var localPos = content.InverseTransformPoint(selected.position);

            var contentWidth = content.rect.width;
            var viewportWidth = viewport.rect.width;

            if (contentWidth <= viewportWidth)
            {
                scrollRect.horizontalNormalizedPosition = 0f;
                return;
            }

            var targetX = Mathf.Clamp01(
                (localPos.x + selected.rect.width * 0.5f - viewportWidth * 0.5f) / (contentWidth - viewportWidth)
            );

            scrollRect.horizontalNormalizedPosition = targetX;
        }

        private void Select(int index)
        {
            if (index < 0 || index >= DisplayImages.Count)
            {
                return;
            }

            // 全てを非選択状態に（表示を戻す）
            for (var i = 0; i < DisplayImages.Count; i++)
            {
                if (i == index)
                {
                    DisplayImages[i].SetAlpha(1.0f);
                }
                else if(Math.Abs(index - i) == 1)
                {
                    DisplayImages[i].SetAlpha(0.45f);
                }
                else
                {
                    DisplayImages[i].SetAlpha(0.2f);
                }
            }

            SelectedIndex = index;
        }
    }
}