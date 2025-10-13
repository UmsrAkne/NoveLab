using UnityEngine;

namespace Utils
{
    public static class AspectUtil
    {
        public static void SetAspect(Camera cam)
        {
            if (cam == null)
            {
                return;
            }

            var targetAspect = 1680f / 720f;
            var windowAspect = (float)Screen.width / Screen.height;

            var scale = windowAspect / targetAspect;

            if (scale < 1.0f)
            {
                // 横幅が狭すぎる → 上下に黒帯
                cam.rect = new Rect(0, (1 - scale) / 2f, 1, scale);
            }
            else
            {
                // 横幅が広すぎる → 左右に黒帯
                var scaleWidth = 1.0f / scale;
                cam.rect = new Rect((1 - scaleWidth) / 2f, 0, scaleWidth, 1);
            }
        }
    }
}