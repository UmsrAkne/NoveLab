using System;
using System.Linq;
using System.Xml.Linq;
using Loaders.XMLs;
using NUnit.Framework;
using ScenarioModel;

namespace Tests.EditMode.Loaders.XMLs
{
    [TestFixture]
    public class AnimeElementParserTests
    {
        private AnimeElementParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new AnimeElementParser();
        }

        [Test]
        public void Parse_SingleSlideAnime_Works()
        {
            var xml = XElement.Parse(@"
                <scenario>
                    <anime name='slide' distance='10' />
                </scenario>");

            var scenario = new ScenarioEntry();
            parser.PopulateScenario(xml, scenario);

            Assert.That(scenario.Animations.Count, Is.EqualTo(1));
            var spec = scenario.Animations.First();
            Assert.That(spec.Name, Is.EqualTo("slide"));
            Assert.That(spec.Attrs["distance"], Is.EqualTo("10"));
            Assert.That(spec.Children, Is.Empty);
        }

        [Test]
        public void Parse_ChainWithChildren_Works()
        {
            var xml = XElement.Parse(@"
                <scenario>
                    <anime name='chain'>
                        <anime name='slide' distance='10' />
                        <anime name='shake' strength='5' />
                    </anime>
                </scenario>");

            var scenario = new ScenarioEntry();
            parser.PopulateScenario(xml, scenario);

            var chain = scenario.Animations.Single();
            Assert.That(chain.Name, Is.EqualTo("chain"));
            Assert.That(chain.Children.Count, Is.EqualTo(2));
            Assert.That(chain.Children[0].Name, Is.EqualTo("slide"));
            Assert.That(chain.Children[1].Attrs["strength"], Is.EqualTo("5"));
        }

        [Test]
        public void Parse_ChainInsideChain_Throws()
        {
            var xml = XElement.Parse(@"
                <scenario>
                    <anime name='chain'>
                        <anime name='chain'>
                            <anime name='slide' distance='10' />
                        </anime>
                    </anime>
                </scenario>");

            var scenario = new ScenarioEntry();
            Assert.Throws<InvalidOperationException>(() =>
                parser.PopulateScenario(xml, scenario)
            );
        }

        [Test]
        public void Parse_AnimeWithoutName_Throws()
        {
            var xml = XElement.Parse(@"
                <scenario>
                    <anime distance='10' />
                </scenario>");

            var scenario = new ScenarioEntry();
            Assert.Throws<InvalidOperationException>(() =>
                parser.PopulateScenario(xml, scenario)
            );
        }
    }
}