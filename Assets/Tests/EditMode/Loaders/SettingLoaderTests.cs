using System;
using System.Xml.Linq;
using Loaders;
using NUnit.Framework;

namespace Tests.EditMode.Loaders
{
    [TestFixture]
    public class SettingLoaderTests
    {
        [Test]
        public void ConvertSceneSetting()
        {
            var xml = "<setting>"
                      + @"  <bgm fileName=""bgmFile"" volume=""1.1"" />"
                      + @"  <voice volume=""1.1"" />"
                      + @"  <se volume=""1.1"" />"
                      + @"  <bgv volume=""1.1"" />"
                      + @"  <window width=""1680"" />"
                      + "</setting>";

            var doc = XDocument.Parse(xml);
            var loader = new SettingLoader();
            var setting = loader.Load(doc);

            Assert.That(setting.WindowWidth, Is.EqualTo(1680));
            Assert.That(setting.BgmOrder.FileName, Is.EqualTo("bgmFile"));

            Assert.That(Math.Abs(1.1 - setting.BgvVolume), Is.LessThan(0.001));
            Assert.That(Math.Abs(1.1 - setting.SeVolume), Is.LessThan(0.001));
            Assert.That(Math.Abs(1.1 - setting.BgvVolume), Is.LessThan(0.001));
        }
    }
}