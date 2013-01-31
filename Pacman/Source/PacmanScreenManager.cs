using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;

using Pacman.ScreenMachine;

namespace Pacman
{
    public class PacmanScreenManager : ScreenManager
    {
        public Logger Logger { get; private set; }

        // Content
        public SpriteFont DebugFont { get; private set; }

        public Texture2D BonusItemsTileset { get; private set; }
        public Texture2D PacManTileset { get; private set; }

        public PacmanScreenManager(Game game, Logger logger)
            : base(game)
        {
            Logger = logger;
        }

        protected override void LoadContent()
        {
            DebugFont = Content.Load<SpriteFont>(@"Fonts\DebugFont.tkfont");
            
            BlankTexture = Content.Load<Texture2D>(@"Textures\Blank.png");
            
            BonusItemsTileset = Content.Load<Texture2D>(@"Textures\BonusItemsTileset.png");
            PacManTileset = Content.Load<Texture2D>(@"Textures\PacManTilesheet.png");

            base.LoadContent();

            Logger.Info("Content loaded.");
        }

        public override void Initialize()
        {
            base.Initialize();

            Logger.Info("Pacman Screen Manager initialized.");
        }
    }
}
