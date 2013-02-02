﻿using System;
using Pacman.Screens;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;

namespace Pacman
{
    public class PacmanGame : Game
    {
        public static readonly int TileWidth = 30;
        public static readonly int ScreenWidth = Level.TilesWide * TileWidth + 300;
        public static readonly int ScreenHeight = 900;

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

            _screenManager.AddScreen(new DebugScreen(Logger));

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
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        private void Logger_NewMessageLogged(Logger logger, LogMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
