using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using Pacman.Actors;
using Pacman.ScreenMachine;

namespace Pacman.Screens
{
    public class DebugScreen : GameScreen
    {
        #region Properties

        public Ghost DrawGhost { get; private set; }

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

        public DebugScreen()
        {
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                Level = new Level(ScreenManager);

                ScreenManager.RegisterAddPointsEvent(Level);
                
                DebugBounds = new Rectangle(Level.TilesWide * PacmanGame.TileWidth,
                                            0,
                                            PacmanGame.ScreenWidth,
                                            PacmanGame.ScreenHeight);
            }
        }

        public override void HandleInput(GameTime gameTime, Input input)
        {
            var mousePos = ScreenManager.MousePosition;
            var mouseGrid = Utils.AbsToGrid(mousePos);

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

            if (input.IsKeyPressed(Key.H))
                Level.SpawnFruit(ScreenManager.CurrentLevel);

            if (input.IsKeyPressed(Key.NumberPad1))
                Level.HideBlinky = !Level.HideBlinky;
            if (input.IsKeyPressed(Key.NumberPad2))
                Level.HidePinky = !Level.HidePinky;
            if (input.IsKeyPressed(Key.NumberPad3))
                Level.HideInky = !Level.HideInky;
            if (input.IsKeyPressed(Key.NumberPad4))
                Level.HideClyde = !Level.HideClyde;

            if (input.IsMousePressed(MouseButton.Left))
            {
                if (mousePos.X >= Level.Blinky.Bounds.Left && mousePos.X <= Level.Blinky.Bounds.Right &&
                    mousePos.Y >= Level.Blinky.Bounds.Top && mousePos.Y <= Level.Blinky.Bounds.Bottom)
                    DrawGhost = Level.Blinky;
                else if (mousePos.X >= Level.Pinky.Bounds.Left && mousePos.X <= Level.Pinky.Bounds.Right &&
                         mousePos.Y >= Level.Pinky.Bounds.Top && mousePos.Y <= Level.Pinky.Bounds.Bottom)
                    DrawGhost = Level.Pinky;
                else if (mousePos.X >= Level.Inky.Bounds.Left && mousePos.X <= Level.Inky.Bounds.Right &&
                         mousePos.Y >= Level.Inky.Bounds.Top && mousePos.Y <= Level.Inky.Bounds.Bottom)
                    DrawGhost = Level.Inky;
                else if (mousePos.X >= Level.Clyde.Bounds.Left && mousePos.X <= Level.Clyde.Bounds.Right &&
                         mousePos.Y >= Level.Clyde.Bounds.Top && mousePos.Y <= Level.Clyde.Bounds.Bottom)
                    DrawGhost = Level.Clyde;
                else
                    DrawGhost = null;
            }

            // Change ghost mode
            if (input.IsKeyPressed(Key.D1))
                Level.GhostMode = GhostMode.Chase;
            if (input.IsKeyPressed(Key.D2))
                Level.GhostMode = GhostMode.Scatter;
            if (input.IsKeyPressed(Key.D3))
                Level.GhostMode = GhostMode.Frightened;

            // Change level
            if (input.IsKeyPressed(Key.PageUp))
                Level.NextLevel();
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
            //DrawGhostDebugInfo(SpriteBatch, gameTime);
            
            // Draw level debug info
            DrawLevelDebugInfo(SpriteBatch, gameTime);

            // Ghost targets
            DrawGhostTargets(SpriteBatch);

            SpriteBatch.End();
        }

        private void DrawLevelDebugInfo(SpriteBatch spriteBatch, GameTime gameTime)
        {
            string text = "";
            var pos = new Vector2(DebugBounds.Left + 5, DebugBounds.Top);

            var gridPos = Utils.AbsToGrid(ScreenManager.MousePosition);
            text += "Mouse:\n";
            text += "    pos: (" + ScreenManager.MousePosition.X + ", " + ScreenManager.MousePosition.Y + ")\n";
            text += "    grid: [" + gridPos.X + ", " + gridPos.Y + "]\n\n";

            text += "Score: " + ScreenManager.Score + "\n\n";

            text += "Frightened Time Left: " + Level.GhostModeDuration.ToString("N2") + "s\n\n";

            text += "Level: " + ScreenManager.CurrentLevel + "\n";
            text += "Dots left: " + Level.DotsLeft + "\n";
            text += "Energizers left: " + Level.EnergizersLeft + "\n\n";

            text += "Ghost Mode: " + Level.GhostMode + "\n\n";

            text += "Speeds:\n";
            text += "  Pac-Man: " + Level.PacMan.SpeedModifier * 100 + "%\n";
            text += "   Blinky: " + Level.Blinky.SpeedModifier * 100 + "%\n";
            text += "    Pinky: " + Level.Pinky.SpeedModifier * 100 + "%\n";
            text += "     Inky: " + Level.Inky.SpeedModifier * 100 + "%\n";
            text += "    Clyde: " + Level.Clyde.SpeedModifier * 100 + "%\n\n";

            spriteBatch.DrawString(ScreenManager.DebugFont, text, pos, Color.White);

            var textSize = ScreenManager.DebugFont.MeasureString(text);

            textSize += DrawActorInfo(spriteBatch, Level.PacMan, new Vector2(pos.X, pos.Y + textSize.Y));

            if (DrawGhost != null)
            {
                var ghostPos = new Vector2(pos.X, pos.Y + textSize.Y);
                DrawActorInfo(spriteBatch, DrawGhost, ghostPos);
            }
        }

        private void DrawGhostDebugInfo(SpriteBatch spriteBatch, GameTime gameTime)
        {
            string text = "";

            text += "Ghost Mode: " + Level.GhostMode + "\n\n";

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

            text += "Inky:\n";
            text += "    pos:  (" + Level.Inky.Position.X + ", " + Level.Inky.Position.Y + ")\n";
            text += "    grid: [" + Level.Inky.GridPosition.X + ", " + Level.Inky.GridPosition.Y + "]\n";
            text += "    direction: " + Level.Inky.Direction + "\n";
            text += "    velocity: " + Level.Inky.Velocity + "\n";
            text += "    target: (" + Level.Inky.TargetTile.X + ", " + Level.Inky.TargetTile.Y + ")\n";
            text += "    next: (" + Level.Inky.NextPosition.X + ", " + Level.Inky.NextPosition.Y + ")\n";

            nextAbsPos = Utils.GridToAbs(Level.Blinky.NextPosition, Level.Blinky.Origin);
            text += "          (" + nextAbsPos.X + ", " + nextAbsPos.Y + ")\n";
            text += "    future: " + Level.Blinky.FutureDirection + "\n\n\n";

            Vector2 pos = new Vector2(DebugBounds.Left + 5, DebugBounds.Top);

            spriteBatch.DrawString(ScreenManager.DebugFont, text, pos, Color.White);
        }

        /// <summary>
        /// Draw debug info for an actor.
        /// </summary>
        /// <returns>Returns the MeasureString vector</returns>
        private Vector2 DrawActorInfo(SpriteBatch spriteBatch, Actor actor, Vector2 position)
        {
            string text = "\n";

            var ghost = actor as Ghost;

            text += actor.GetType().ToString() + ":\n";
            text += "    pos:  " + actor.Position.ToString("N2") + "\n";
            text += "    grid: " + actor.GridPosition + "\n";
            text += "    direction: " + actor.Direction + "\n";

            if (ghost != null)
                text += "    future dir: " + ghost.FutureDirection + "\n";

            text += "    velocity: " + actor.Velocity.ToString("N2") + "\n";

            if (ghost != null)
                text += "    target: " + ghost.TargetTile + "\n";

            text += "    next: " + actor.NextPosition + "\n";

            spriteBatch.DrawString(ScreenManager.DebugFont, text, position, Color.White);

            return ScreenManager.DebugFont.MeasureString(text);
        }

        private void DrawGhostTargets(SpriteBatch spriteBatch)
        {
            // No need to draw targets in frightened mode
            if (Level.GhostMode == GhostMode.Frightened)
                return;

            DrawingRectangle targetTileRect;

            // Blinky's target tile
            if (!Level.HideBlinky)
            {
                targetTileRect = new DrawingRectangle(
                    (int) (Level.Blinky.TargetTile.X*PacmanGame.TileWidth),
                    (int) (Level.Blinky.TargetTile.Y*PacmanGame.TileWidth), PacmanGame.TileWidth, PacmanGame.TileWidth);
                spriteBatch.Draw(ScreenManager.BlankTexture, targetTileRect, Color.Red);
            }

            // Pinky's target tile
            if (!Level.HidePinky)
            {
                targetTileRect = new DrawingRectangle(
                    (int)(Level.Pinky.TargetTile.X * PacmanGame.TileWidth), (int)(Level.Pinky.TargetTile.Y * PacmanGame.TileWidth), PacmanGame.TileWidth, PacmanGame.TileWidth);
                spriteBatch.Draw(ScreenManager.BlankTexture, targetTileRect, Color.Pink);
            }

            // Inky's target tile
            if (!Level.HideInky)
            {
                targetTileRect = new DrawingRectangle(
                    (int)(Level.Inky.TargetTile.X * PacmanGame.TileWidth), (int)(Level.Inky.TargetTile.Y * PacmanGame.TileWidth), PacmanGame.TileWidth, PacmanGame.TileWidth);
                spriteBatch.Draw(ScreenManager.BlankTexture, targetTileRect, Color.Blue);
            }

            // Clyde's target tile
            if (!Level.HideClyde)
            {
                targetTileRect = new DrawingRectangle(
                    (int)(Level.Clyde.TargetTile.X * PacmanGame.TileWidth), (int)(Level.Clyde.TargetTile.Y * PacmanGame.TileWidth), PacmanGame.TileWidth, PacmanGame.TileWidth);
                spriteBatch.Draw(ScreenManager.BlankTexture, targetTileRect, Color.Orange);
            }
        }
    }
}
