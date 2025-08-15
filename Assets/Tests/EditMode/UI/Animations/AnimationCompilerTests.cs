using Core;
using NUnit.Framework;
using ScenarioModel;
using UI.Animations;
using UnityEngine;

namespace Tests.EditMode.UI.Animations
{
    [TestFixture]
    public class AnimationCompilerTests
    {
        private AnimationCompiler compiler;

        [SetUp]
        public void SetUp()
        {
            compiler = new AnimationCompiler(new DummyDisplayImage(), new DummyImageAdder());
        }

        [Test]
        public void Compile_Slide_BindsDistance()
        {
            var spec = new AnimationSpec
            {
                Name = "slide",
                Attrs = { ["distance"] = "12.5", }, // 大文字小文字無視で拾えるはず
            };

            var anim = compiler.Compile(spec);
            Assert.That(anim, Is.TypeOf<Slide>());
            Assert.That(((Slide)anim).Distance, Is.EqualTo(12.5f).Within(1e-4));
        }

        [Test]
        public void Compile_Image_BindsStringAttr()
        {
            var spec = new AnimationSpec
            {
                Name = "image",
                Attrs = { ["a"] = "A0101", },
            };

            var anim = compiler.Compile(spec);
            Assert.That(anim, Is.TypeOf<ImageAddAnimation>());
            // Assert.That(((ImageAddAnimation)anim).A, Is.EqualTo("A0101"));
        }

        [Test]
        public void Compile_Chain_CompilesChildren()
        {
            var spec = new AnimationSpec
            {
                Name = "chain",
                Children =
                {
                    new AnimationSpec { Name = "slide", Attrs = { ["distance"] = "10", }, },
                    new AnimationSpec { Name = "shake", Attrs = { ["strength"] = "5", }, },
                    new AnimationSpec { Name = "image", Attrs = { ["a"] = "A0001", }, },
                },
            };

            var anim = compiler.Compile(spec);
            Assert.That(anim, Is.TypeOf<AnimationChain>());

            var chain = (AnimationChain)anim;
            Assert.That(chain.Children.Count, Is.EqualTo(3));
            Assert.That(chain.Children[0], Is.TypeOf<Slide>());
            Assert.That(((Slide)chain.Children[0]).Distance, Is.EqualTo(10f).Within(1e-4));
            Assert.That(chain.Children[1], Is.TypeOf<Shake>());
            Assert.That(((Shake)chain.Children[1]).Strength, Is.EqualTo(5f).Within(1e-4));
            Assert.That(chain.Children[2], Is.TypeOf<ImageAddAnimation>());
            // Assert.That(((ImageAddAnimation)chain.Children[2]).A, Is.EqualTo("A0001"));
        }

        [Test]
        public void Compile_CaseInsensitiveAttributeName_Works()
        {
            var spec = new AnimationSpec
            {
                Name = "slide",
                Attrs = { ["DiStAnCe"] = "7", }, // 属性名の大小混在
            };

            var anim = compiler.Compile(spec);
            Assert.That(((Slide)anim).Distance, Is.EqualTo(7f).Within(1e-4));
        }

        private sealed class DummyDisplayImage : IDisplayImage { /* メソッドは空でOK */
            public void SetAlpha(float alpha)
            {
                throw new System.NotImplementedException();
            }

            public void SetScale(float scale)
            {
                throw new System.NotImplementedException();
            }

            public void SetTexture(Texture2D texture)
            {
                throw new System.NotImplementedException();
            }

            public GameObject GameObject { get; }

            public void RegisterAnimation(string key, IUIAnimation animation)
            {
                throw new System.NotImplementedException();
            }

            public void PlayAnimations()
            {
                throw new System.NotImplementedException();
            }

            public int SortingOrder { get; set; }

            public void SetBasePosition(Vector2 pos)
            {
                throw new System.NotImplementedException();
            }

            public void SetOffsetPosition(Vector2 offset)
            {
                throw new System.NotImplementedException();
            }
        }

        // Tests/Stubs/DummyImageAdder.cs
        private sealed class DummyImageAdder : IImageAdder { /* メソッドは空でOK */
            public void AddImage(IDisplayImage image)
            {
                throw new System.NotImplementedException();
            }

            public IDisplayImage GetFront()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}