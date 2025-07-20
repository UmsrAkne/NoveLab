using System;
using System.Linq;
using System.Xml.Linq;
using Core;
using Loaders.XMLs;
using NUnit.Framework;

namespace Tests.EditMode.Loaders.XMLs
{
    public class ImageElementParserTests
    {
        [Test]
        public void ParseImageTag()
        {
            var xml = "<root>"
                      + "<scenario>"
                      + "<image"
                      + @"	a=""imageA"" b=""imageB"" c=""imageC"" d=""imageD"""
                      + @"	x=""1"" y=""2"""
                      + @"	isOverwrite=""true"""
                      + @"	scale=""1.1"""
                      + @"	duration=""2"""
                      + @"	delay=""3"""
                      + @"	targetLayerIndex=""1"""
                      + "/>"
                      + "</scenario>"
                      + "</root>";

            var doc = XDocument.Parse(xml);
            var parser = new ImageElementParser();
            var scenario = new ScenarioEntry();
            parser.PopulateScenario(doc.Element("root")!.Elements("scenario").First(), scenario);

            var order = scenario.ImageOrders.First();

            Assert.That(order.X, Is.EqualTo(1));
            Assert.That(order.Y, Is.EqualTo(2));
            Assert.That(order.IsOverwrite, Is.EqualTo(true));
            Assert.That(Math.Abs(1.1 - order.Scale), Is.LessThan(0.001));
            Assert.That(Math.Abs(2 - order.Duration), Is.LessThan(0.001));
            Assert.That(Math.Abs(3 - order.Delay), Is.LessThan(0.001));
            Assert.That(order.TargetLayerIndex, Is.EqualTo(1));
        }

    }
}