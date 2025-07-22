using System;
using System.Linq;
using System.Xml.Linq;
using Loaders.XMLs;
using NUnit.Framework;
using ScenarioModel;

namespace Tests.EditMode.Loaders.XMLs
{
    public class BgvElementParserTests
    {
        [Test]
        public void ParseBgvElement()
        {
            var xml = "<root>"
                      + "<scenario>"
                      + "<bgv"
                      + @"  names=""file1"""
                      + @"  volume=""1.1"" pan=""-1"" channel=""4"""
                      + "/>"
                      + "<backgroundVoice"
                      + @"  names=""file2, file3,"""
                      + @"  volume=""1.2"" pan=""1"" channel=""3"""
                      + "/>"
                      + "</scenario>"
                      + "</root>";

            var doc = XDocument.Parse(xml);
            var parser = new BgvElementParser();
            var scenario = new ScenarioEntry();
            parser.PopulateScenario(doc.Element("root")!.Elements("scenario").First(), scenario);

            Assert.AreEqual(scenario.BgvOrders.Count, 2);

            var order = scenario.BgvOrders.First();
            Assert.AreEqual(order.AudioType, AudioType.Bgv);

            CollectionAssert.AreEqual(new [] { "file1", }, scenario.BgvOrders[0].FileNames);
            CollectionAssert.AreEqual(new [] { "file2", "file3", }, scenario.BgvOrders[1].FileNames);

            Assert.That(Math.Abs(1 + order.Pan), Is.LessThan(0.001));
            Assert.That(Math.Abs(1.1 - order.Volume), Is.LessThan(0.001));
            Assert.AreEqual(order.ChannelIndex, 4);
        }

    }
}