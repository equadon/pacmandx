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

            SpriteBatch.Begin();
            SpriteBatch.Draw(ScreenManager.BlankTexture, screenRect, new Color(0, 0, 0, 150));
            SpriteBatch.DrawString(ScreenManager.GameFont, "Game Over", new Vector2(200, 200), Color.Red);
            SpriteBatch.End();
        }
    }
}
