using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UI.Animations;

namespace Tests.EditMode.UI.Animations
{
    public class AnimationChainTest
    {
        [Test]
        public async Task AnimationChain_ExecutesAllAnimationsSequentially()
        {
            var animA = new FakeAnimation();
            var animB = new FakeAnimation();
            var chain = new AnimationChain { LoopCount = 1, };

            chain.Add(animA);
            chain.Add(animB);

            var completed = false;
            chain.OnCompleted += () => completed = true;

            chain.Start();

            await UniTask.DelayFrame(1); // Startは非同期に投げてるのでちょっと待つ

            Assert.IsTrue(animA.IsPlaying);
            Assert.IsFalse(animB.IsPlaying);

            animA.Complete(); // A完了

            await UniTask.DelayFrame(1);

            Assert.IsFalse(animA.IsPlaying);
            Assert.IsTrue(animB.IsPlaying);

            animB.Complete(); // B完了

            await UniTask.DelayFrame(1);

            Assert.IsTrue(completed);
        }

        [Test]
        public async Task AnimationChain_GroupedAnimations_RunInParallel()
        {
            var animB = new FakeAnimation { GroupId = "wave", };
            var animC = new FakeAnimation { GroupId = "wave", };
            var chain = new AnimationChain { LoopCount = 1, };

            chain.Add(animB);
            chain.Add(animC);

            chain.Start();

            await UniTask.DelayFrame(1);

            Assert.IsTrue(animB.IsPlaying);
            Assert.IsTrue(animC.IsPlaying);

            // 両方完了しないと OnCompleted 呼ばれない
            var completed = false;
            chain.OnCompleted += () => completed = true;

            animB.Complete();
            await UniTask.DelayFrame(1);
            Assert.IsFalse(completed); // Cがまだ

            animC.Complete();
            await UniTask.DelayFrame(1);
            Assert.IsTrue(completed);
        }
    }
}