﻿using System;
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
        private static readonly int[,] _legalTiles;

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
            for (int y = 0; y < TilesHigh; y++)
            {
                for (int x = 0; x < TilesWide; x++)
                {
                    Color color = (_legalTiles[x, y] == (int) TileType.Empty) ? DebugEmptyTileColor : DebugUnknownTileColor;
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

            spriteBatch.Draw(ScreenManager.BlankTexture, new DrawingRectangle(TilesWide * PacmanGame.TileWidth, 0, 1, TilesHigh * PacmanGame.TileWidth), DebugBorderColor);
        }

        #endregion

        #region Generate Level

        static Level()
        {
            _legalTiles = new int[,] {
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
