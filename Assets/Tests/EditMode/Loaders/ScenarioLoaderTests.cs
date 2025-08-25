using System.Linq;
using System.Xml.Linq;
using Loaders;
using NUnit.Framework;

namespace Tests.EditMode.Loaders
{
    public class ScenarioLoaderTests
    {
        [Test]
        public void ParseScenarioXml()
        {
            var xml =
                "<root>"
                + "   <scenario>"
                + @"      <text string=""test1"" />"
                + "   </scenario>"
                + "   <scenario>"
                + @"      <text string=""test2"" />"
                + "   </scenario>"
                + "   <scenario>"
                + "   </scenario>"
                + "</root>";

            var doc = XDocument.Parse(xml);
            var scenarios = new ScenarioLoader().Load(doc);

            Assert.That(scenarios.Count, Is.EqualTo(3));
            CollectionAssert.AreEqual(
                new[] { "test1", "test2", string.Empty, },
                scenarios.Select(s => s.Text));
        }
    }
}