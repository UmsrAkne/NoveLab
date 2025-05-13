using System.Collections.Generic;
using Loaders;
using NUnit.Framework;

namespace Tests.EditMode.Loaders
{
    public class ImageLoaderTests
    {
        [Test]
        [TestCase(0, new [] {"a", "b", "c", }, "先頭から開始")]
        [TestCase(1, new [] {"b", "c", "a", }, "中心から開始")]
        [TestCase(2, new [] {"c", "b", "a", }, "末尾から開始")]
        [TestCase(3, new [] {"a", "b", "c", }, "範囲外のインデックス指定")]
        public void ReOrderByProximity(int initialIndex, string[] expected, string description)
        {
            var list = new List<string>() { "a", "b", "c", };
            var actual = ImageLoader.ReorderByProximity(list, initialIndex);
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}