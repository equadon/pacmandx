using System.Windows.Forms;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;

using Pacman.Actors;
using Pacman.ScreenMachine;

namespace Pacman.Screens
{
    public class TestLevelScreen : GameScreen
    {
        #region Properties

        public Level Level { get; private set; }

        public new PacmanScreenManager ScreenManager
        {
            get { return (PacmanScreenManager)base.ScreenManager; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return ScreenManager.SpriteBatch; }
        }

        #endregion

        public TestLevelScreen(Logger logger)
        {
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                Level = new Level(ScreenManager);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            var mouseGrid = Utils.AbsToGrid(ScreenManager.MousePosition);

            // Change ghost mode
            if (input.IsKeyPressed(Key.D1))
                Level.GhostMode = GhostMode.Chase;
            if (input.IsKeyPressed(Key.D2))
                Level.GhostMode = GhostMode.Scatter;
            if (input.IsKeyPressed(Key.D3))
                Level.GhostMode = GhostMode.Frightened;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            Level.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Level.Draw(SpriteBatch, gameTime);

            SpriteBatch.End();
        }
    }
}
