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

        // Starting positions
        public static readonly Vector2 PacmanStartingPosition = new Vector2(420, 780 + PacmanGame.TileWidth / 2f);
        public static readonly Vector2 BlinkyStartingPosition = new Vector2(420, 420 + PacmanGame.TileWidth / 2f);

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

        public PacmanScreenManager ScreenManager { get; private set; }

        public PacMan PacMan { get; private set; }
        public Blinky Blinky { get; private set; }

        public Level(PacmanScreenManager screenManager)
        {
            _tiles = new int[TilesWide, TilesHigh];

            ScreenManager = screenManager;

            Blinky = new Blinky(ScreenManager.BlankTexture, BlinkyStartingPosition);

            PacMan = new PacMan(ScreenManager.PacManTileset, PacmanStartingPosition);

            _random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            PacMan.Update(gameTime);
            Blinky.Update(gameTime);
        }

        #region Draw Methods

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawBoard(spriteBatch);
            
#if DEBUG
            float rotation = 0f;
            switch (Blinky.FutureDirection)
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
            //spriteBatch.Draw(ScreenManager.DirectionTexture, Utils.GridToAbs(Blinky.NextPosition, Blinky.Origin), new DrawingRectangle(0, 0, 30, 30), Color.Red, rotation, new Vector2(15, 15), 1f, SpriteEffects.None, 0f);
#endif

            PacMan.Draw(spriteBatch, gameTime);
            Blinky.Draw(spriteBatch, gameTime);
        }

        private void DrawBoard(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < TilesHigh; y++)
            {
                for (int x = 0; x < TilesWide; x++)
                {
                    Color color = (LegalTiles[x, y] == (int) TileType.Empty) ? DebugEmptyTileColor : DebugUnknownTileColor;
#if DEBUG
                    var mouseGrid = Utils.AbsToGrid(ScreenManager.MousePosition);
                    if (mouseGrid.X == x && mouseGrid.Y == y)
                        color = new Color(200, 200, 200, 255);
#endif
                    var rect = new DrawingRectangle(x*PacmanGame.TileWidth, y*PacmanGame.TileWidth, PacmanGame.TileWidth,
                                                    PacmanGame.TileWidth);
                    spriteBatch.Draw(ScreenManager.BlankTexture, rect, color);

                    for (int i = 0; i < 30; i += 6)
                        spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(x * PacmanGame.TileWidth + i, y * PacmanGame.TileWidth, 2, 1), DebugBorderColor);

                    for (int i = 0; i < 30; i += 6)
                        spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(x * PacmanGame.TileWidth, y * PacmanGame.TileWidth + i, 1, 2), DebugBorderColor);
                }
            }

            spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(TilesWide * PacmanGame.TileWidth, 0, 1, TilesHigh * PacmanGame.TileWidth), DebugBorderColor);
        }

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
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,1,1,0,0,0,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,0,0,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,0,0,0,0,1,1,0,0,0,0,1,1,1,1,1,0,1,1,0,0,0,0,1,1,0,0,0,0,1,1,1},
                {1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1},
                {1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1},
                {1,1,1,1,0,0,0,0,0,1,1,0,0,0,0,1,1,1,1,1,0,1,1,0,0,0,0,1,1,0,0,0,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1},
                {1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,0,0,0,1,1,0,1,1,1},
                {1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1},
                {1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,1,1,0,0,0,0,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };
        }

        #endregion
    }
}
