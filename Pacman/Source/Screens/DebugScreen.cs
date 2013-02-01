using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;

using Pacman.Actors;
using Pacman.ScreenMachine;

namespace Pacman.Screens
{
    public class DebugScreen : GameScreen
    {
        #region Properties

        public Logger Logger { get; private set; }

        public Level Level { get; private set; }

        public new PacmanScreenManager ScreenManager
        {
            get { return (PacmanScreenManager) base.ScreenManager; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return ScreenManager.SpriteBatch; }
        }

        #endregion

        public DebugScreen(Logger logger)
        {
            Logger = logger;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                Level = new Level(Logger, ScreenManager);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
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
