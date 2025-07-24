using System;
using System.Linq;
using System.Xml.Linq;
using Loaders.XMLs;
using NUnit.Framework;
using ScenarioModel;

namespace Tests.EditMode.Loaders.XMLs
{
    public class BgmElementParserTests
    {
        [Test]
        public void ParseSeElement()
        {
            var xml = "<root>"
                      + "<scenario>"
                      + "<bgm"
                      + @"  fileName=""bgmFile"""
                      + @"  volume=""1.1"" delay=""2"""
                      + "/>"
                      + "</scenario>"
                      + "</root>";

            var doc = XDocument.Parse(xml);
            var parser = new BgmElementParser();
            var scenario = new ScenarioEntry();
            parser.PopulateScenario(doc.Element("root")!.Elements("scenario").First(), scenario);

            var order = scenario.BgmOrder;
            Assert.AreEqual(order.AudioType, AudioType.Bgm);

            Assert.That(order.FileName, Is.EqualTo("bgmFile"));
            Assert.That(Math.Abs(2 - order.Delay), Is.LessThan(0.001));
            Assert.That(Math.Abs(1.1 - order.Volume), Is.LessThan(0.001));
        }

    }
}