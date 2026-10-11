using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Framework.Testing;
using osuTK;
using osuTK.Input;

namespace FlickTest.Game.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneMultiBall : ManualInputManagerTestScene
    {
        private MainScreen screen;

        public TestSceneMultiBall()
        {
            Add(new ScreenStack(screen = new MainScreen()) { RelativeSizeAxes = Axes.Both });
        }

        private int ballCount => screen.Balls.Count();

        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("clear balls", () => screen.Balls.ToList().ForEach(b => b.Expire()));
            AddUntilStep("no balls", () => ballCount == 0);
        }

        [Test]
        public void TestBackgroundClickSpawnsBall()
        {
            int before = 0;
            AddStep("count balls", () => before = ballCount);
            AddStep("move mouse to background", () => InputManager.MoveMouseTo(screen.ToScreenSpace(new Vector2(20))));
            AddStep("left click", () => InputManager.Click(MouseButton.Left));
            AddAssert("a new ball spawned", () => ballCount == before + 1);
        }

        [Test]
        public void TestRightClickDeletesBall()
        {
            int before = 0;
            AddStep("move to background", () => InputManager.MoveMouseTo(screen.ToScreenSpace(new Vector2(200))));
            AddStep("spawn a ball", () => InputManager.Click(MouseButton.Left));
            AddStep("count balls", () => before = ballCount);
            AddStep("right click a ball", () =>
            {
                InputManager.MoveMouseTo(screen.Balls.First());
                InputManager.Click(MouseButton.Right);
            });
            AddAssert("ball was removed", () => ballCount == before - 1);
        }
    }
}
