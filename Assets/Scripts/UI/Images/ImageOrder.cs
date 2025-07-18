using System.Collections.Generic;

namespace UI.Images
{
    public class ImageOrder
    {
        public List<string> ImageNames { get; set; } = new (4);

        /// <summary>
        /// この命令が、画像の上書きか、新規レイヤーの追加かを表します。
        /// </summary>
        public bool IsOverwrite { get; set; }

        public float X { get; set; } = 0;

        public float Y { get; set; } = 0;

        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// 画像の描画開始を指定秒数遅らせます。
        /// </summary>
        public float Delay { get; set; } = 0f;

        /// <summary>
        /// 画像の描画に要する時間を秒で指定します。
        /// </summary>
        public float Duration { get; set; } = 1.0f;

        public int TargetLayerIndex { get; set; } = 0;
    }
}