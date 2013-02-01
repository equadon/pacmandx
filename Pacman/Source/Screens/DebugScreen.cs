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
            var mouseGrid = Utils.AbsToGrid(ScreenManager.MousePosition);

            if (input.IsMousePressed(MouseButton.Left) &&
                mouseGrid.X >= 0 && mouseGrid.X < Level.TilesWide &&
                mouseGrid.Y >= 0 && mouseGrid.Y < Level.TilesHigh &&
                mouseGrid != Level.Blinky.GridPosition)
            {
                Level.Blinky.TargetTile = mouseGrid;
            }
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
            string text = "";

            text += "Mouse:\n";
            text += "    pos: (" + ScreenManager.MousePosition.X + ", " + ScreenManager.MousePosition.Y + ")\n";

            var gridPos = Utils.AbsToGrid(ScreenManager.MousePosition);
            text += "    grid: [" + gridPos.X + ", " + gridPos.Y + "]\n\n";

            text += "Blinky:\n";
            text += "    grid: [" + Level.Blinky.GridPosition.X + ", " + Level.Blinky.GridPosition.Y + "]\n";
            text += "    direction: " + Level.Blinky.Direction + "\n";
            text += "    target: (" + Level.Blinky.TargetTile.X + ", " + Level.Blinky.TargetTile.Y + ")\n";

            Vector2 pos = new Vector2(DebugBounds.Left + 5, DebugBounds.Top);

            spriteBatch.DrawString(ScreenManager.DebugFont, text, pos, Color.White);

            var targetTileRect = new DrawingRectangle(
                (int)(Level.Blinky.TargetTile.X * PacmanGame.TileWidth), (int)(Level.Blinky.TargetTile.Y * PacmanGame.TileWidth), PacmanGame.TileWidth, PacmanGame.TileWidth);
            spriteBatch.Draw(ScreenManager.BlankTexture, targetTileRect, Color.Green);
        }
    }
}
