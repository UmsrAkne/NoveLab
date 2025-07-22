using System;
using System.Linq;
using System.Xml.Linq;
using Loaders.XMLs;
using NUnit.Framework;
using ScenarioModel;

namespace Tests.EditMode.Loaders.XMLs
{
    public class SeElementParserTests
    {
        [Test]
        public void ParseSeElement()
        {
            var xml = "<root>"
                      + "<scenario>"
                      + "<se"
                      + @"  fileName=""soundFile"""
                      + @"  volume=""1.1"" delay=""2"" pan=""-1"" repeatCount=""3"" channel=""4"""
                      + "/>"
                      + "<se"
                      + @"  fileName=""soundFile"""
                      + @"  volume=""1.2"" delay=""4"" pan=""1"" repeatCount=""5"" channel=""3"""
                      + "/>"
                      + "</scenario>"
                      + "</root>";

            var doc = XDocument.Parse(xml);
            var parser = new SeElementParser();
            var scenario = new ScenarioEntry();
            parser.PopulateScenario(doc.Element("root")!.Elements("scenario").First(), scenario);

            Assert.AreEqual(scenario.SeOrders.Count, 2);

            var order = scenario.SeOrders.First();
            Assert.AreEqual(order.AudioType, AudioType.Se);

            Assert.That(order.FileName, Is.EqualTo("soundFile"));
            Assert.That(Math.Abs(2 - order.Delay), Is.LessThan(0.001));
            Assert.That(Math.Abs(1 + order.Pan), Is.LessThan(0.001));
            Assert.That(Math.Abs(3 - order.RepeatCount), Is.LessThan(0.001));
            Assert.AreEqual(order.ChannelIndex, 4);
        }

    }
}