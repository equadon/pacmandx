using System;
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
        Unknown,
        Empty,
        WallStraight,
        DoubleWallStraight // etc
    }

    #endregion

    public class Level
    {
        #region Fields

        public static readonly int TilesWide = 28;
        public static readonly int TilesHigh = 36;

        public static readonly Color DebugEmptyTileColor = new Color(79, 79, 79, 255);
        public static readonly Color DebugBorderColor = new Color(100, 100, 100, 255);
        public static readonly Color DebugUnknownTileColor = Color.Black;

        /// <summary>
        /// Array listing legal and illegal tiles that will be used by the pathfinding.
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

        public PacmanScreenManager ScreenManager { get; private set; }

        public Level(Logger logger, PacmanScreenManager screenManager)
        {
            _tiles = new int[TilesWide, TilesHigh];

            ScreenManager = screenManager;
        }

        public void Update(GameTime gameTime)
        {
        }

        #region Draw Methods

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawBoard(spriteBatch);
        }

        private void DrawBoard(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < TilesWide; x++)
            {
                for (int y = 0; y < TilesHigh; y++)
                {
                    Color color = (LegalTiles[x, y] == (int) TileType.Empty) ? DebugEmptyTileColor : DebugUnknownTileColor;
                    spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(x * PacmanGame.TileWidth, y * PacmanGame.TileWidth, PacmanGame.TileWidth, PacmanGame.TileWidth), color);

#if DEBUG
                    // Draw dotted cell borders in debug mode
                    int newX = x * PacmanGame.TileWidth;
                    for (int i = 0; i < PacmanGame.TileWidth / 5; i++)
                    {
                        spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(newX, y * PacmanGame.TileWidth, 2, 1), DebugBorderColor);
                        newX += 5;
                    }

                    int newY = y * PacmanGame.TileWidth;
                    for (int i = 0; i < PacmanGame.TileWidth / 5; i++)
                    {
                        spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(x * PacmanGame.TileWidth, newY, 1, 2), DebugBorderColor);
                        newY += 5;
                    }
#endif
                }
            }
        }

        #endregion

        #region Generate Level

        static Level()
        {
            LegalTiles = new int[TilesWide,TilesHigh];

            // Build level
            GenerateLevel();
        }

        /// <summary>
        /// Generate level and assign different TileTypes to the appropriate tiles.
        /// </summary>
        private static void GenerateLevel()
        {
            // Rows
            LegalizeRow(4, 1, 12);
            LegalizeRow(4, 15, 26);

            LegalizeRow(8, 1, 26);

            LegalizeRow(11, 1, 6);
            LegalizeRow(11, 9, 12);
            LegalizeRow(11, 15, 18);
            LegalizeRow(11, 21, 26);
        }

        private static void LegalizeRow(int row, int startX, int endX)
        {
            for (int x = startX; x <= endX; x++)
                LegalTiles[x, row] = (int) TileType.Empty;
        }

        private static void LegalizeCol(int col, int startY, int endY)
        {
            for (int y = startY; y <= endY; y++)
                LegalTiles[col, y] = (int)TileType.Empty;
        }

        #endregion
    }
}
