using System.Collections.Generic;

namespace Core
{
    public interface IImageOrder
    {
        public List<string> ImageNames { get; set; }

        /// <summary>
        /// この命令が、画像の上書きか、新規レイヤーの追加かを表します。
        /// </summary>
        public bool IsOverwrite { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Scale { get; set; }

        /// <summary>
        /// 画像の描画開始を指定秒数遅らせます。
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        /// 画像の描画に要する時間を秒で指定します。
        /// </summary>
        public float Duration { get; set; }

        public int TargetLayerIndex { get; set; }
    }
}