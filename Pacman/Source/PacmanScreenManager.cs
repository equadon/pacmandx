using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;

using Pacman.ScreenMachine;

namespace Pacman
{
    public class PacmanScreenManager : ScreenManager
    {
        // Content
        public SpriteFont DebugFont { get; private set; }

        public Texture2D SimpleLevelTiles { get; private set; }

        public Texture2D BonusItemsTileset { get; private set; }
        public Texture2D PacManTileset { get; private set; }
        public Texture2D GhostBlinkyTileset { get; private set; }
        public Texture2D GhostPinkyTileset { get; private set; }
        public Texture2D GhostInkyTileset { get; private set; }
        public Texture2D GhostClydeTileset { get; private set; }

        public Texture2D DirectionTexture { get; private set; }

        public int CurrentLevel { get; set; }

        public Vector2 MousePosition
        {
            get
            {
                var handle = (System.Windows.Forms.Control)Game.Window.NativeWindow;
                var cursorPos = handle.PointToClient(System.Windows.Forms.Cursor.Position);

                return new Vector2(cursorPos.X, cursorPos.Y);
            }
        }

        public PacmanScreenManager(Game game)
            : base(game)
        {
            CurrentLevel = 1;
        }

        protected override void LoadContent()
        {
            DebugFont = Content.Load<SpriteFont>(@"Fonts\DebugFont.tkfont");
            
            BlankTexture = Content.Load<Texture2D>(@"Textures\Blank.png");

            SimpleLevelTiles = Content.Load<Texture2D>(@"Textures\LevelTilesSimple.png");

            PacManTileset = Content.Load<Texture2D>(@"Textures\PacMan.png");

            GhostBlinkyTileset = Content.Load<Texture2D>(@"Textures\Blinky.png");
            GhostPinkyTileset = Content.Load<Texture2D>(@"Textures\Pinky.png");
            GhostInkyTileset = Content.Load<Texture2D>(@"Textures\Inky.png");
            GhostClydeTileset = Content.Load<Texture2D>(@"Textures\Clyde.png");

            DirectionTexture = Content.Load<Texture2D>(@"Textures\Direction.png");

            base.LoadContent();

            PacmanGame.Logger.Info("Content loaded.");
        }

        public override void Initialize()
        {
            base.Initialize();

            PacmanGame.Logger.Info("Pacman Screen Manager initialized.");
        }
    }
}
