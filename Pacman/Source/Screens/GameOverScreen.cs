using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pacman.ScreenMachine;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Screens
{
    public class GameOverScreen : GameScreen
    {
        public new PacmanScreenManager ScreenManager
        {
            get { return (PacmanScreenManager)base.ScreenManager; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return ScreenManager.SpriteBatch; }
        }

        public GameOverScreen()
        {
            IsPopup = true;
        }
        
        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
        }

        public override void Draw(GameTime gameTime)
        {
            var screenRect = new DrawingRectangle(0, 0, Level.TilesWide * PacmanGame.TileWidth, Level.TilesHigh * PacmanGame.TileWidth);

            const string gameOverText = "Game Over";
            Vector2 size = ScreenManager.GameFont.MeasureString(gameOverText);
            var pos = new Vector2(PacmanGame.ScreenWidth / 2f - size.X / 2f, PacmanGame.ScreenHeight / 2f - size.Y);

            SpriteBatch.Begin();
            SpriteBatch.Draw(ScreenManager.BlankTexture, screenRect, new Color(0, 0, 0, 150));
            SpriteBatch.DrawString(ScreenManager.GameFont, gameOverText, pos, Color.White);
            SpriteBatch.End();
        }
    }
}
