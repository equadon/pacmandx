using System;
using System.Windows.Forms;
using Pacman.Screens;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using Pacman.ScreenMachine;

namespace Pacman
{
    #region Event Handler and EventArgs class

    public delegate void PointsAddedHandler(object sender, AddPointsEventArgs e);

    public class AddPointsEventArgs
    {
        public int Points;

        public AddPointsEventArgs(int points)
        {
            Points = points;
        }
    }

    #endregion

    public class PacmanScreenManager : ScreenManager
    {
        #region Properties

        // Content
        public SpriteFont DebugFont { get; private set; }
        public SpriteFont GameFont { get; private set; }
        public SpriteFont HudFont { get; private set; }

        public Texture2D LevelBackground { get; private set; }

        public Texture2D BonusItemsTileset { get; private set; }
        public Texture2D PacManTileset { get; private set; }
        public Texture2D GhostBlinkyTileset { get; private set; }
        public Texture2D GhostPinkyTileset { get; private set; }
        public Texture2D GhostInkyTileset { get; private set; }
        public Texture2D GhostClydeTileset { get; private set; }

        public Texture2D DotEnergizerTexture { get; private set; }
        public Texture2D EyesPointsTexture { get; private set; }

        public Texture2D DirectionTexture { get; private set; }

        private GameScreen _currentGameScreen;

        public int Score { get; private set; }
        public int CurrentLevel { get; set; }

        public int Lives { get; private set; }

        public Vector2 MousePosition
        {
            get
            {
                var handle = (Control) Game.Window.NativeWindow;
                var cursorPos = handle.PointToClient(Cursor.Position);

                return new Vector2(cursorPos.X, cursorPos.Y);
            }
        }

        #endregion

        public PacmanScreenManager(Game game)
            : base(game)
        {
            CurrentLevel = 1;
            Score = 0;

            Lives = 3;

            NewGame();
        }

        protected override void LoadContent()
        {
            DebugFont = Content.Load<SpriteFont>(@"Fonts\DebugFont.tkfont");
            GameFont = Content.Load<SpriteFont>(@"Fonts\GameFont.tkfont");
            HudFont = Content.Load<SpriteFont>(@"Fonts\DimitriSwank.tkfont");
            
            BlankTexture = Content.Load<Texture2D>(@"Textures\Blank.png");

            LevelBackground = Content.Load<Texture2D>(@"Textures\Level.png");

            PacManTileset = Content.Load<Texture2D>(@"Textures\PacMan.png");

            GhostBlinkyTileset = Content.Load<Texture2D>(@"Textures\Blinky.png");
            GhostPinkyTileset = Content.Load<Texture2D>(@"Textures\Pinky.png");
            GhostInkyTileset = Content.Load<Texture2D>(@"Textures\Inky.png");
            GhostClydeTileset = Content.Load<Texture2D>(@"Textures\Clyde.png");

            DotEnergizerTexture = Content.Load<Texture2D>(@"Textures\DotEnergizer.png");
            EyesPointsTexture = Content.Load<Texture2D>(@"Textures\EyesPoints.png");

            BonusItemsTileset = Content.Load<Texture2D>(@"Textures\BonusItems.png");

            DirectionTexture = Content.Load<Texture2D>(@"Textures\Direction.png");

            base.LoadContent();

            PacmanGame.Logger.Info("Content loaded.");
        }

        public override void Initialize()
        {
            base.Initialize();

            PacmanGame.Logger.Info("Pacman Screen Manager initialized.");
        }

        public void NewGame()
        {
#if DEBUG
            _currentGameScreen = new DebugScreen();
#else
            _currentGameScreen = new TestLevelScreen();
#endif
            AddScreen(_currentGameScreen);
        }

        /// <summary>
        /// Level wishes to register the add points event.
        /// </summary>
        public void RegisterAddPointsEvent(Level level)
        {
            level.AddPoints += new PointsAddedHandler(AddPoints);
        }

        private void AddPoints(object sender, AddPointsEventArgs e)
        {
            Score += e.Points;
        }

        public void KillPlayer(Level level)
        {
            Lives--;

            if (Lives < 1)
            {
                // Game Over
                AddScreen(new GameOverScreen());
            }
            else
            {
                // Display "Get Ready" screen
                level.ResetLevel();
            }
        }
    }
}
