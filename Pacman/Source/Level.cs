using System;
using Pacman.Actors;
using Pacman.Actors.Ghosts;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;

namespace Pacman
{
    #region Enums

    public enum TileItem
    {
        None,
        Dot,
        Energizer,
        Cherries, // level 1
        Strawberry, // level 2
        Peach, // level 3-4
        Apple, // level 5-6
        Grapes, // level 7-8
        Galaxian, // level 9-10
        Bell, // level 11-12
        Key, // level 13+
    }

    public enum TileType
    {
        Empty,
        Unknown,
        WallStraight,
        DoubleWallStraight // etc
    }

    #endregion

    public class Level
    {
        #region Fields

        public static readonly int TilesWide = 28;
        public static readonly int TilesHigh = 36;

        // Colors
        public static readonly Color DebugEmptyTileColor = new Color(79, 79, 79, 255);
        public static readonly Color DebugBorderColor = new Color(150, 150, 150, 255);
        public static readonly Color DebugUnknownTileColor = Color.Black;

        /// <summary>
        /// Array listing legal and illegal tiles that will be used by the pathfinding.
        /// 
        /// Tiles are accessed in a x, y style, not row, col
        /// 
        /// 0: legal
        /// 1: level tile #1
        /// 2: level tile #2
        /// 3: level tile #3
        /// 4: level tile #4
        /// level tiles are the four different type of level tiles we can have to build the map
        /// </summary>
        private static readonly int[,] LegalTiles;

        /// <summary>
        /// Array storing the state for the current level. Use the TileType
        /// enum for IDs.
        /// </summary>
        private readonly int[,] _tiles;

        #endregion

        private Random _random;

        private GhostMode _ghostMode;

        public PacmanScreenManager ScreenManager { get; private set; }

        // Starting positions
        public readonly Vector2 PacmanStartingPosition = new Vector2(14 * PacmanGame.TileWidth, 26 * PacmanGame.TileWidth + PacmanGame.TileWidth / 2f);

        public GhostMode GhostMode
        {
            get { return _ghostMode; }
            set
            {
                if (value != _ghostMode)
                {
                    _ghostMode = value;
                    
                    // Inform all ghosts of the chanage
                    Blinky.ForceNewDirection = true;
                    Pinky.ForceNewDirection = true;
                    Inky.ForceNewDirection = true;
                    Clyde.ForceNewDirection = true;
                }
            }
        }

        public PacMan PacMan { get; private set; }

        public Blinky Blinky { get; private set; }
        public Pinky Pinky { get; private set; }
        public Inky Inky { get; private set; }
        public Clyde Clyde { get; private set; }

        public Level(PacmanScreenManager screenManager)
        {
            _tiles = new int[TilesWide, TilesHigh];

            ScreenManager = screenManager;

            var pacOrigin = new Vector2(48 * Sprite.Scale / 2f, 48 * Sprite.Scale / 2f);
            var ghostOrigin = new Vector2(48 * Sprite.Scale / 2f, 51 * Sprite.Scale / 2f);

            _ghostMode = GhostMode.Scatter;

            PacMan = new PacMan(this, ScreenManager.PacManTileset, PacmanStartingPosition);

            //Blinky = new Blinky(this, ScreenManager.GhostBlinkyTileset, BlinkyStartingPosition);
            Blinky = new Blinky(this, ScreenManager.GhostBlinkyTileset, Utils.GridToAbs(new Vector2(26, 4), ghostOrigin));

            //Pinky = new Pinky(this, ScreenManager.GhostPinkyTileset, PinkyStartingPosition);
            Pinky = new Pinky(this, ScreenManager.GhostPinkyTileset, Utils.GridToAbs(new Vector2(4, 4), ghostOrigin));

            //Inky = new Inky(this, ScreenManager.GhostInkyTileset, InkyStartingPosition);
            Inky = new Inky(this, ScreenManager.GhostInkyTileset, Utils.GridToAbs(new Vector2(24, 32), ghostOrigin));

            //Clyde = new Clyde(this, ScreenManager.GhostClydeTileset, ClydeStartingPosition);
            Clyde = new Clyde(this, ScreenManager.GhostClydeTileset, Utils.GridToAbs(new Vector2(4, 32), ghostOrigin));

            _random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            PacMan.Update(gameTime);

            Blinky.Update(gameTime);
            Pinky.Update(gameTime);
            Inky.Update(gameTime);
            Clyde.Update(gameTime);
        }

        #region Draw Methods

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawBoard(spriteBatch);
            
#if DEBUG
            DrawGhostDirection(spriteBatch, Blinky);
            DrawGhostDirection(spriteBatch, Pinky);
            DrawGhostDirection(spriteBatch, Inky);
            DrawGhostDirection(spriteBatch, Clyde);
#endif

            PacMan.Draw(spriteBatch, gameTime);

            Blinky.Draw(spriteBatch, gameTime);
            Pinky.Draw(spriteBatch, gameTime);
            Inky.Draw(spriteBatch, gameTime);
            Clyde.Draw(spriteBatch, gameTime);
        }

        private void DrawBoard(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < TilesHigh; y++)
            {
                for (int x = 0; x < TilesWide; x++)
                {
                    int tile = LegalTiles[x, y];

                    var destination = new DrawingRectangle(x*PacmanGame.TileWidth, y*PacmanGame.TileWidth,
                                                           PacmanGame.TileWidth, PacmanGame.TileWidth);

                    float rotation = 0f;
                    DrawingRectangle source = new DrawingRectangle(0, 0, 30, 30);
                    Vector2 origin = new Vector2(PacmanGame.TileWidth / 2f, PacmanGame.TileWidth / 2f);
                    Vector2 pos = new Vector2(
                                x * PacmanGame.TileWidth + origin.X,
                                y * PacmanGame.TileWidth + origin.Y);

                    switch (tile)
                    {
                        case 0:
                            spriteBatch.Draw(ScreenManager.BlankTexture, destination, Color.White);
                            break;
                        case 1:
                            spriteBatch.Draw(ScreenManager.BlankTexture, destination, Color.Black);
                            break;
                        case 2:
                            rotation = 0f;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 3:
                            rotation = MathUtil.Pi;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 4:
                            rotation = MathUtil.Pi + MathUtil.Pi/2f;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 5:
                            rotation = MathUtil.Pi / 2f;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 6:
                            rotation = MathUtil.Pi/2;
                            source.X = 30;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 7:
                            rotation = 0f;
                            source.X = 30;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 8:
                            rotation = -MathUtil.Pi / 2f;
                            source.X = 30;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 9:
                            rotation = -MathUtil.Pi;
                            source.X = 30;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 10:
                            rotation = MathUtil.Pi;
                            source.X = 60;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 11:
                            rotation = MathUtil.Pi / 2f;
                            source.X = 60;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 12:
                            rotation = 0f;
                            source.X = 60;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        case 13:
                            rotation = -MathUtil.Pi / 2f;
                            source.X = 60;
                            spriteBatch.Draw(ScreenManager.SimpleLevelTiles, pos, source, Color.White, rotation, origin,
                                             1f, SpriteEffects.None, 0f);
                            break;
                        default:
                            spriteBatch.Draw(ScreenManager.BlankTexture, destination, Color.Blue);
                            break;
                    }
#if DEBUG
                    Color color = (LegalTiles[x, y] == (int) TileType.Empty) ? DebugEmptyTileColor : DebugUnknownTileColor;

                    var mouseGrid = Utils.AbsToGrid(ScreenManager.MousePosition);
                    if (mouseGrid.X == x && mouseGrid.Y == y)
                        color = new Color(200, 200, 200, 255);

                    var rect = new DrawingRectangle(x*PacmanGame.TileWidth, y*PacmanGame.TileWidth, PacmanGame.TileWidth,
                                                    PacmanGame.TileWidth);
                    spriteBatch.Draw(ScreenManager.BlankTexture, rect, color);

                    for (int i = 0; i < 30; i += 6)
                        spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(x * PacmanGame.TileWidth + i, y * PacmanGame.TileWidth, 2, 1), DebugBorderColor);

                    for (int i = 0; i < 30; i += 6)
                        spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(x * PacmanGame.TileWidth, y * PacmanGame.TileWidth + i, 1, 2), DebugBorderColor);
#endif
                }
            }

            spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(TilesWide * PacmanGame.TileWidth, 0, 1, TilesHigh * PacmanGame.TileWidth), DebugBorderColor);
        }

#if DEBUG
        private void DrawGhostDirection(SpriteBatch spriteBatch, Ghost ghost)
        {
            // We don't use future direction in frightened mode
            if (GhostMode == GhostMode.Frightened)
                return;

            float rotation = 0f;
            switch (ghost.FutureDirection)
            {
                case Direction.Up:
                    rotation = MathUtil.DegreesToRadians(0);
                    break;
                case Direction.Right:
                    rotation = MathUtil.DegreesToRadians(90);
                    break;
                case Direction.Down:
                    rotation = MathUtil.DegreesToRadians(180);
                    break;
                case Direction.Left:
                    rotation = MathUtil.DegreesToRadians(270);
                    break;
            }

            Color color;
            if (ghost is Blinky)
                color = Color.Red;
            else if (ghost is Pinky)
                color = Color.Pink;
            else if (ghost is Inky)
                color = Color.Blue;
            else
                color = Color.Orange;

            spriteBatch.Draw(ScreenManager.DirectionTexture, Utils.GridToAbs(ghost.NextPosition, ghost.Origin), new DrawingRectangle(0, 0, 30, 30), color, rotation, new Vector2(15, 15), 1f, SpriteEffects.None, 0f);
        }
#endif
        #endregion

        public static bool IsLegal(Vector2 position)
        {
            try
            {
                return LegalTiles[(int)position.X, (int)position.Y] == (int)TileType.Empty;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public static bool IsLegal(int x, int y)
        {
            return LegalTiles[x, y] == (int)TileType.Empty;
        }

        public static Rectangle TileBounds(Vector2 position)
        {
            int xPos = (int)position.X * PacmanGame.TileWidth;
            int yPos = (int)position.Y * PacmanGame.TileWidth;
            return new Rectangle(xPos, yPos, xPos + PacmanGame.TileWidth, yPos + PacmanGame.TileWidth);
        }

        #region Generate Level

        static Level()
        {
            LegalTiles = new int[,] {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 7, 3, 8, 0, 7, 8, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 7, 8, 0, 1, 1, 0, 7, 8, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 4, 5, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 4, 5, 0, 0, 0, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 4, 5, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 4,11, 3, 3, 8, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 6, 2, 9, 0, 6, 9, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 6, 2, 2, 2, 9, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 7, 3, 8, 0, 7, 3, 3, 3, 3, 3, 3, 8, 0, 7, 3, 3, 3, 8, 0, 7, 8, 0, 7, 3, 3,10, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 6, 2, 2,13,12, 2, 2, 9, 0, 6, 2, 2, 2, 9, 0, 4, 5, 0, 6, 2, 2,13, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 7, 8, 0, 4, 5, 0, 1, 1, 1, 1, 1, 0, 7, 8, 0, 4, 5, 0, 7, 8, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 6, 2, 9, 0, 4, 5, 0, 6, 9, 0, 1, 1, 1, 1, 1, 0, 4, 5, 0, 6, 9, 0, 4, 5, 0, 6, 9, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 4, 5, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 3, 3, 3, 8, 0, 4,11, 3, 3, 8, 0, 1, 1, 1, 1, 1, 0, 4,11, 3, 3, 8, 0, 4,11, 3, 3, 8, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 2, 2, 2, 9, 0, 4,12, 2, 2, 9, 0, 1, 1, 1, 1, 1, 0, 4,12, 2, 2, 9, 0, 4,12, 2, 2, 9, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 4, 5, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 7, 3, 8, 0, 4, 5, 0, 7, 8, 0, 1, 1, 1, 1, 1, 0, 4, 5, 0, 7, 8, 0, 4, 5, 0, 7, 8, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 6, 9, 0, 4, 5, 0, 1, 1, 1, 1, 1, 0, 6, 9, 0, 4, 5, 0, 6, 9, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 0, 0, 0, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 7, 3, 3,10,11, 3, 3, 8, 0, 7, 3, 3, 3, 8, 0, 4, 5, 0, 7, 3, 3,10, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 6, 2, 9, 0, 6, 2, 2, 2, 2, 2, 2, 9, 0, 6, 2, 2, 2, 9, 0, 6, 9, 0, 6, 2, 2,13, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 7, 3, 8, 0, 7, 8, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 7, 3, 3, 3, 8, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 4, 5, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 4,12, 2, 2, 9, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 4, 1, 5, 0, 4, 5, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 4, 5, 0, 0, 0, 0, 4, 5, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 6, 2, 9, 0, 6, 9, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 6, 9, 0, 1, 1, 0, 6, 9, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            };

            // TODO: use this version once we have the full map drawn out so we can load one large image once rather
            //       than building it up every frame.
            //LegalTiles = new int[,] {
            //    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
            //    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            //};
        }

        #endregion
    }
}
