using System;
using Pacman.Screens;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;

namespace Pacman
{
    public class PacmanGame : Game
    {
        public static readonly int TileWidth = 27;
#if DEBUG
        public static readonly int ScreenWidth = Level.TilesWide * TileWidth + 300;
#else
        public static readonly int ScreenWidth = Level.TilesWide * TileWidth;
#endif
        public static readonly int ScreenHeight = Level.TilesHigh * TileWidth;

        public static readonly Logger Logger = new Logger();

        private readonly GraphicsDeviceManager _graphics;
        private readonly PacmanScreenManager _screenManager;

        public PacmanGame()
        {
            Logger.NewMessageLogged += new Logger.LogAction(Logger_NewMessageLogged);
            Logger.Info("Logger initiated.");

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;

            _screenManager = new PacmanScreenManager(this);
            GameSystems.Add(_screenManager);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
#if DEBUG
            Window.Title = "Pacman using SharpDX (DEBUG)";
            IsMouseVisible = true;
#else
            Window.Title = "Pacman using SharpDX";
#endif

            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        private void Logger_NewMessageLogged(Logger logger, LogMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
