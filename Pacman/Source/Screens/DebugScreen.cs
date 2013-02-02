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
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                Level = new Level(ScreenManager);
                
                DebugBounds = new Rectangle(Level.TilesWide * PacmanGame.TileWidth,
                                            0,
                                            PacmanGame.ScreenWidth,
                                            PacmanGame.ScreenHeight);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            var mouseGrid = Utils.AbsToGrid(ScreenManager.MousePosition);

            // Control pacman with WASD or arrow keys
            // TODO: How often do we want the polling to occurr?
            if (input.IsKeyDown(Key.W) || input.IsKeyDown(Key.UpArrow))
                Level.PacMan.ChangeDirection(Direction.Up);

            if (input.IsKeyDown(Key.S) || input.IsKeyDown(Key.Down))
                Level.PacMan.ChangeDirection(Direction.Down);

            if (input.IsKeyDown(Key.A) || input.IsKeyDown(Key.Left))
                Level.PacMan.ChangeDirection(Direction.Left);

            if (input.IsKeyDown(Key.D) || input.IsKeyDown(Key.Right))
                Level.PacMan.ChangeDirection(Direction.Right);

            // Set Blinky's target tile with left mouse button
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
            text += "    pos:  (" + Level.Blinky.Position.X + ", " + Level.Blinky.Position.Y + ")\n";
            text += "    grid: [" + Level.Blinky.GridPosition.X + ", " + Level.Blinky.GridPosition.Y + "]\n";
            text += "    direction: " + Level.Blinky.Direction + "\n";
            text += "    velocity: " + Level.Blinky.Velocity + "\n";
            text += "    target: (" + Level.Blinky.TargetTile.X + ", " + Level.Blinky.TargetTile.Y + ")\n";
            text += "    next: (" + Level.Blinky.NextPosition.X + ", " + Level.Blinky.NextPosition.Y + ")\n";

            Vector2 nextAbsPos = Utils.GridToAbs(Level.Blinky.NextPosition, Level.Blinky.Origin);
            text += "          (" + nextAbsPos.X + ", " + nextAbsPos.Y + ")\n";
            text += "    future: " + Level.Blinky.FutureDirection + "\n\n\n";

            text += "Pacman:\n";
            text += "    pos: " + Level.PacMan.Position + "\n";
            text += "   grid: " + Level.PacMan.GridPosition + "\n";
            text += "   direction: " + Level.PacMan.Direction + "\n";
            text += "   velocity: " + Level.PacMan.Velocity + "\n";
            text += "   top left: (" + Level.PacMan.Bounds.Left + "," + Level.PacMan.Bounds.Top + ")\n";
            text += "   bot right: (" + Level.PacMan.Bounds.Right + "," + Level.PacMan.Bounds.Bottom + ")\n";

            Rectangle tileBounds = Level.TileBounds(Level.PacMan.GridPosition);
            text += "\n\nPacman Tile:\n";
            text += "   top left: (" + tileBounds.Left + "," + tileBounds.Top + ")\n";
            text += "   bot right: (" + tileBounds.Right + "," + tileBounds.Bottom + ")\n";

            Vector2 pos = new Vector2(DebugBounds.Left + 5, DebugBounds.Top);

            spriteBatch.DrawString(ScreenManager.DebugFont, text, pos, Color.White);

            // Blinky's target tile
            var targetTileRect = new DrawingRectangle(
                (int)(Level.Blinky.TargetTile.X * PacmanGame.TileWidth), (int)(Level.Blinky.TargetTile.Y * PacmanGame.TileWidth), PacmanGame.TileWidth, PacmanGame.TileWidth);
            spriteBatch.Draw(ScreenManager.BlankTexture, targetTileRect, Color.Red);

            // Pinky's target tile
            targetTileRect = new DrawingRectangle(
                (int)(Level.Pinky.TargetTile.X * PacmanGame.TileWidth), (int)(Level.Pinky.TargetTile.Y * PacmanGame.TileWidth), PacmanGame.TileWidth, PacmanGame.TileWidth);
            spriteBatch.Draw(ScreenManager.BlankTexture, targetTileRect, Color.Pink);
        }
    }
}
