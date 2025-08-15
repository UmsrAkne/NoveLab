using System.Collections.Generic;

namespace ScenarioModel
{
    public class ImageOrder : IOrder
    {
        private string a;

        public List<string> ImageNames { get; set; } =
            new (4) { string.Empty, string.Empty, string.Empty, string.Empty, };

        public string A { get => ImageNames[0]; set => ImageNames[0] = value; }

        public string B { get => ImageNames[1]; set => ImageNames[1] = value; }

        public string C { get => ImageNames[2]; set => ImageNames[2] = value; }

        public string D { get => ImageNames[3]; set => ImageNames[3] = value; }

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

        public List<string> ResourceFileNames => ImageNames;
    }
}