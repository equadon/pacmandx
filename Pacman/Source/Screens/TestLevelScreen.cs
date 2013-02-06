using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Toolkit;
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

        public TestLevelScreen()
        {
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                Level = new Level(ScreenManager);

                ScreenManager.RegisterAddPointsEvent(Level);
            }
        }

        public override void HandleInput(GameTime gameTime, Input input)
        {
            var mouseGrid = Utils.AbsToGrid(ScreenManager.MousePosition);

            // Change ghost mode
            if (input.IsKeyPressed(Key.D1))
                Level.GhostMode = GhostMode.Chase;
            if (input.IsKeyPressed(Key.D2))
                Level.GhostMode = GhostMode.Scatter;
            if (input.IsKeyPressed(Key.D3))
                Level.GhostMode = GhostMode.Frightened;

            // Control pacman with WASD or arrow keys
            // TODO: How often do we want the polling to occurr?
            if (input.IsKeyDown(Key.W) || input.IsKeyDown(Key.UpArrow))
                Level.PacMan.Move(Direction.Up);

            if (input.IsKeyDown(Key.S) || input.IsKeyDown(Key.Down))
                Level.PacMan.Move(Direction.Down);

            if (input.IsKeyDown(Key.A) || input.IsKeyDown(Key.Left))
                Level.PacMan.Move(Direction.Left);

            if (input.IsKeyDown(Key.D) || input.IsKeyDown(Key.Right))
                Level.PacMan.Move(Direction.Right);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!IsActive)
                return;

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
