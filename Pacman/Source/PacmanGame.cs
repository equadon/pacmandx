using System;
using Pacman.Screens;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;

namespace Pacman
{
    public class PacmanGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly Logger _logger;
        private readonly PacmanScreenManager _screenManager;

        public PacmanGame()
        {
            _logger = new Logger();
            _logger.NewMessageLogged += new Logger.LogAction(Logger_NewMessageLogged);
            _logger.Info("Logger initiated.");

            _graphics = new GraphicsDeviceManager(this);

            _screenManager = new PacmanScreenManager(this, _logger);
            GameSystems.Add(_screenManager);

            _screenManager.AddScreen(new DebugScreen(_logger));

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.Title = "Pacman using SharpDX";

#if DEBUG
            IsMouseVisible = true;
#endif

            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        private void Logger_NewMessageLogged(Logger logger, LogMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
