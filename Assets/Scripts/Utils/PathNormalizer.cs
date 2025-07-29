using System;
using System.IO;

namespace Utils
{
    public static class PathNormalizer
    {
        /// <summary>
        /// 入力されたパスを正規化します。
        /// </summary>
        /// <param name="inputPath">正規化するファイルのパスを入力します。絶対パス・相対パス共に入力可能ですが、それぞれ結果が異なります。</param>
        /// <param name="defaultExtension">入力されたパスに拡張子がない場合に追記する拡張子を ".png" の形式で入力します。</param>
        /// <returns>正規化されたパスを返します。</returns>
        /// <exception cref="ArgumentException">パスの文字列が空文字やNullの場合にスローされます。</exception>
        public static string NormalizeFilePath(string inputPath, string defaultExtension = "")
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                throw new ArgumentException("パスが空です", nameof(inputPath));
            }

            // 区切り文字を統一（バックスラッシュ → スラッシュ）
            var unifiedPath = inputPath.Replace('\\', '/');

            // 小文字化
            unifiedPath = unifiedPath.ToLowerInvariant();

            // 拡張子がなければ追加
            if (Path.HasExtension(unifiedPath) == false)
            {
                unifiedPath += defaultExtension.ToLowerInvariant();
            }

            return unifiedPath;
        }
    }
}