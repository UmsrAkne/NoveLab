using NUnit.Framework;
using UI.TypeWriter;

namespace Tests.EditMode.UI.TypeWriter
{
    public class TypewriterEngineTests
    {
        private class DummyTextTarget : ITextDisplayTarget
        {
            public string Text { get; set; } = "";
        }

        [Test]
        public void SetText_InitializesStateCorrectly()
        {
            var dummy = new DummyTextTarget();
            var engine = new TypewriterEngine(dummy);

            engine.SetText("Hello");

            Assert.AreEqual("", dummy.Text);
            Assert.IsFalse(engine.IsFinished);
        }

        [Test]
        public void Update_AddsCharactersOverTime()
        {
            var dummy = new DummyTextTarget();
            var engine = new TypewriterEngine(dummy);

            engine.SetText("Hi");
            engine.Update(0.05f); // 1文字分
            Assert.AreEqual("H", dummy.Text);

            engine.Update(0.05f); // 次の文字
            Assert.AreEqual("Hi", dummy.Text);
            Assert.IsTrue(engine.IsFinished);
        }

        [Test]
        public void Update_CanHandleMultipleCharactersInOneCall()
        {
            var dummy = new DummyTextTarget();
            var engine = new TypewriterEngine(dummy);

            engine.SetText("Hey");
            engine.Update(0.15f); // 3文字分の時間（0.05f * 3）

            Assert.AreEqual("Hey", dummy.Text);
            Assert.IsTrue(engine.IsFinished);
        }
    }
}