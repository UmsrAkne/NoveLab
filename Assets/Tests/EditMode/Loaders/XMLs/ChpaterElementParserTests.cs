using System.Linq;
using System.Xml.Linq;
using Loaders.XMLs;
using NUnit.Framework;
using ScenarioModel;

namespace Tests.EditMode.Loaders.XMLs
{
    public class ChapterElementParserTests
    {
        [Test]
        public void ParseChapterElement()
        {
            var xml = "<root>"
                      + "<scenario>"
                      + @"  <chapter name=""chapter1"" />"
                      + "</scenario>"

                      + "<scenario>"
                      + "</scenario>"
                      + "</root>";

            var doc = XDocument.Parse(xml);
            var parser = new ChapterElementParser();
            var scenario1 = new ScenarioEntry();
            var scenario2 = new ScenarioEntry();

            var elements = doc.Element("root")!.Elements("scenario").ToList();

            parser.PopulateScenario(elements[0], scenario1);
            parser.PopulateScenario(elements[1], scenario2);

            Assert.AreEqual(scenario1.Chapter.Name, "chapter1");
            Assert.IsNull(scenario2.Chapter);
        }

    }
}