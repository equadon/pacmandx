using System.Windows.Forms;
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

        public Rectangle DebugBounds { get; private set; }

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
                
                DebugBounds = new Rectangle(Level.TilesWide * PacmanGame.TileWidth,
                                            0,
                                            PacmanGame.ScreenWidth,
                                            PacmanGame.ScreenHeight);
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

            // Draw debug info on the right side of the screen
            DrawDebugInfo(SpriteBatch, gameTime);

            SpriteBatch.End();
        }

        private void DrawDebugInfo(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var mousePos = new Vector2(0, 0);

            var a = (Control)ScreenManager.Game.Window.NativeWindow;

            string text = "";

            text += "Mouse:\n";
            text += "    pos: (" + mousePos.X + ", " + mousePos.Y + ")";

            Vector2 pos = new Vector2(DebugBounds.Left + 5, DebugBounds.Top);

            spriteBatch.DrawString(ScreenManager.DebugFont, text, pos, Color.White);
        }
    }
}
