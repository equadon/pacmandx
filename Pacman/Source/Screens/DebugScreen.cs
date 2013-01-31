using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;

using Pacman.ScreenMachine;

namespace Pacman.Screens
{
    public class DebugScreen : GameScreen
    {
        #region Properties

        public Logger Logger { get; private set; }

        public Sprite Fruit { get; private set; }

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
                Fruit = new Sprite(ScreenManager.BonusItemsTileset, new Vector2(PacmanGame.ScreenWidth/2f, PacmanGame.ScreenHeight/2f), new Rectangle(8, 5, 76, 83));
                var a = Fruit.Origin;
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            Fruit.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            
            SpriteBatch.DrawString(ScreenManager.DebugFont, "Debugging!", Vector2.Zero, Color.White);

            Fruit.Draw(SpriteBatch, gameTime);

            SpriteBatch.End();
        }
    }
}
