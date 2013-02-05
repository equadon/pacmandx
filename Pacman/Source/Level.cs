using System;
using Pacman.Actors;
using Pacman.Actors.Ghosts;
using SharpDX;
using SharpDX.Toolkit;
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

        // Points
        public static readonly int DotPoints = 10;
        public static readonly int EnergizerPoints = 50;

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

        private static readonly int[,] DotTiles;

        /// <summary>
        /// Array storing the state for the current level. Use the TileItem
        /// enum for IDs.
        /// </summary>
        private ScoreItem[,] _tileItems;

        private Random _random;

        private GhostMode _ghostMode;

        #endregion

        #region Properties

        public bool HideBlinky { get; set; }
        public bool HidePinky { get; set; }
        public bool HideInky { get; set; }
        public bool HideClyde { get; set; }

        public int DotsLeft { get; private set; }
        public int EnergizersLeft { get; private set; }

        public ScoreItem Fruit { get; private set; }

        public PacmanScreenManager ScreenManager { get; private set; }

        // Starting positions
        public readonly Vector2 PacmanStartingPosition = new Vector2(14*PacmanGame.TileWidth,
                                                                     26*PacmanGame.TileWidth + PacmanGame.TileWidth/2f);

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

        // Points are stored in PacmanScreenManager, but we can use this to add points
        // with help from the event handler.
        public int Points
        {
            get { return ScreenManager.Score; }
            set
            {
                // Fire add points event
                if (AddPoints != null)
                    AddPoints(this, new AddPointsEventArgs(value - Points));
            }
        }

        #endregion

        // Event to use when adding points
        public event PointsAddedHandler AddPoints;

        public Level(PacmanScreenManager screenManager)
        {
            ScreenManager = screenManager;

            ResetLevel();

            _random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            // Next level?
            if (DotsLeft + EnergizersLeft == 0)
                NextLevel();

            PacMan.Update(gameTime);

            if (!HideBlinky)
                Blinky.Update(gameTime);
            if (!HidePinky)
                Pinky.Update(gameTime);
            if (!HideInky)
                Inky.Update(gameTime);
            if (!HideClyde)
                Clyde.Update(gameTime);

            if (Fruit != null)
            {
                Fruit.Update(gameTime);

                if (Fruit.Duration < 0)
                    Fruit = null;
            }
        }

        /// <summary>
        /// Fills the level with dots and energizers
        /// </summary>
        private void Fill()
        {
            var dotSource = new DrawingRectangle(7, 33, 11, 11);
            var energizerSource = new DrawingRectangle(1, 1, 24, 24);

            for (int y = 0; y < TilesHigh; y++)
            {
                for (int x = 0; x < TilesWide; x++)
                {
                    var pos = new Vector2(x * PacmanGame.TileWidth + PacmanGame.TileWidth / 2f, y * PacmanGame.TileWidth + PacmanGame.TileWidth / 2f);

                    if (DotTiles[x, y] == (int)TileItem.Dot)
                    {
                        _tileItems[x, y] = new ScoreItem(TileItem.Dot, 0, this, ScreenManager.DotEnergizerTexture, pos,
                                                         dotSource);
                        DotsLeft++;
                    }
                    else if (DotTiles[x, y] == (int)TileItem.Energizer)
                    {
                        _tileItems[x, y] = new ScoreItem(TileItem.Energizer, 0, this, ScreenManager.DotEnergizerTexture, pos,
                                                         energizerSource);
                        EnergizersLeft++;
                    }
                }
            }
        }

        /// <summary>
        /// Eat the fruit.
        /// </summary>
        public void EatFruit()
        {
            if (Fruit == null)
                return;

            Points += Fruit.Points;
            Fruit = null;
        }

        /// <summary>
        /// Eat the item at position.
        /// </summary>
        /// <param name="position">Pac-Man position</param>
        /// <returns>Returns the type of item that was eaten.</returns>
        public TileItem EatItem(Vector2 position)
        {
            ScoreItem item = _tileItems[(int) position.X, (int) position.Y];
            if (item != null)
            {
                var type = item.ItemType;

                if (type == TileItem.Dot)
                {
                    DotsLeft--;
                    Blinky.HandleElroy(ScreenManager.CurrentLevel, DotsLeft);
                }
                else if (type == TileItem.Energizer)
                {
                    EnergizersLeft--;
                }

                Points += item.Points;
                _tileItems[(int) position.X, (int) position.Y] = null;

                // TODO: Spawn fruits after 70 dots or 70 dots+energizers?
                // 1st spawn after 70 dots have been cleared
                if (Fruit == null && DotsLeft == 170)
                    SpawnFruit(ScreenManager.CurrentLevel);

                // 2nd spawn after 170 dots have been cleared
                if (Fruit == null && DotsLeft == 70)
                    SpawnFruit(ScreenManager.CurrentLevel);

                return type;
            }
            return TileItem.None;
        }

        private void SpawnFruit(int currentLevel)
        {
            DrawingRectangle source = new DrawingRectangle(5, 3, 42, 48);;
            TileItem type;

            if (currentLevel == 1)
            {
                type = TileItem.Cherries;
                source = new DrawingRectangle(5, 3, 42, 48);
            }
            else if (currentLevel == 2)
            {
                type = TileItem.Strawberry;
                source = new DrawingRectangle(114, 2, 45, 50);
            }
            else if (currentLevel >= 3 && currentLevel <= 4)
            {
                type = TileItem.Peach;
                source = new DrawingRectangle(112, 55, 46, 52);
            }
            else if (currentLevel >= 5 && currentLevel <= 6)
            {
                type = TileItem.Apple;
                source = new DrawingRectangle(3, 55, 47, 51);
            }
            else if (currentLevel >= 7 && currentLevel <= 8)
            {
                type = TileItem.Grapes;
                source = new DrawingRectangle(2, 111, 50, 49);
            }
            else if (currentLevel >= 9 && currentLevel <= 10)
            {
                type = TileItem.Galaxian;
                source = new DrawingRectangle(109, 111, 52, 48);
            }
            else if (currentLevel >= 11 && currentLevel <= 12)
            {
                type = TileItem.Bell;
                source = new DrawingRectangle(2, 164, 49, 50);
            }
            else
            {
                type = TileItem.Key;
                source = new DrawingRectangle(120, 164, 30, 51);
            }

            var position = new Vector2(14 * PacmanGame.TileWidth, 20 * PacmanGame.TileWidth + PacmanGame.TileWidth / 2f);

            Fruit = new ScoreItem(type, 10, this, ScreenManager.BonusItemsTileset, position, source);
        }

        #region Change Level Methods

        public void NextLevel()
        {
            ScreenManager.CurrentLevel++;
            ResetLevel();
        }

        private void ResetLevel()
        {
            // Reset tile items
            _tileItems = new ScoreItem[TilesWide,TilesHigh];
            Fill();

            // Reset ghosts and pacman
            ResetActors();

            GhostMode = GhostMode.Scatter;
        }

        private void ResetActors()
        {
            var pacOrigin = new Vector2(48 * Sprite.Scale / 2f, 48 * Sprite.Scale / 2f);
            var ghostOrigin = new Vector2(48 * Sprite.Scale / 2f, 51 * Sprite.Scale / 2f);

            PacMan = new PacMan(this, ScreenManager.PacManTileset, PacmanStartingPosition);

            //Blinky = new Blinky(this, ScreenManager.GhostBlinkyTileset, BlinkyStartingPosition);
            Blinky = new Blinky(this, ScreenManager.GhostBlinkyTileset, Utils.GridToAbs(new Vector2(26, 4), ghostOrigin));

            //Pinky = new Pinky(this, ScreenManager.GhostPinkyTileset, PinkyStartingPosition);
            Pinky = new Pinky(this, ScreenManager.GhostPinkyTileset, Utils.GridToAbs(new Vector2(4, 4), ghostOrigin));

            //Inky = new Inky(this, ScreenManager.GhostInkyTileset, InkyStartingPosition);
            Inky = new Inky(this, ScreenManager.GhostInkyTileset, Utils.GridToAbs(new Vector2(24, 32), ghostOrigin));

            //Clyde = new Clyde(this, ScreenManager.GhostClydeTileset, ClydeStartingPosition);
            Clyde = new Clyde(this, ScreenManager.GhostClydeTileset, Utils.GridToAbs(new Vector2(4, 32), ghostOrigin));
        }

        #endregion


        #region Draw Methods

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawBoard(spriteBatch);

            // Draw items
            for (int y = 0; y < TilesHigh; y++)
                for (int x = 0; x < TilesWide; x++)
                    if (_tileItems[x, y] != null)
                        _tileItems[x, y].Draw(spriteBatch, gameTime);
            
#if DEBUG
            if (!HideBlinky)
                DrawGhostDirection(spriteBatch, Blinky);
            if (!HidePinky)
                DrawGhostDirection(spriteBatch, Pinky);
            if (!HideInky)
                DrawGhostDirection(spriteBatch, Inky);
            if (!HideClyde)
                DrawGhostDirection(spriteBatch, Clyde);
#endif

            PacMan.Draw(spriteBatch, gameTime);

            if (!HideBlinky)
                Blinky.Draw(spriteBatch, gameTime);
            if (!HidePinky)
                Pinky.Draw(spriteBatch, gameTime);
            if (!HideInky)
                Inky.Draw(spriteBatch, gameTime);
            if (!HideClyde)
                Clyde.Draw(spriteBatch, gameTime);

            // Fruit
            if (Fruit != null)
                Fruit.Draw(spriteBatch, gameTime);
        }

        private void DrawBoard(SpriteBatch spriteBatch)
        {
            var backgroundRect = new DrawingRectangle(0, 0, TilesWide * PacmanGame.TileWidth, TilesHigh * PacmanGame.TileWidth);

            spriteBatch.Draw(ScreenManager.LevelBackground, backgroundRect, Color.White);

#if DEBUG
            // Mouse grid position highlight
            var mouseGrid = Utils.AbsToGrid(ScreenManager.MousePosition);

            var rect = new DrawingRectangle((int)mouseGrid.X * PacmanGame.TileWidth, (int)mouseGrid.Y * PacmanGame.TileWidth, PacmanGame.TileWidth,
                                            PacmanGame.TileWidth);
            spriteBatch.Draw(ScreenManager.BlankTexture, rect, new Color(200, 200, 200, 255));

            // Draw grid lines
            for (int x = 0; x < TilesWide; x++)
            {
                for (int y = 0; y < TilesHigh; y++)
                {
                    for (int i = 0; i < 30; i += 6)
                        spriteBatch.Draw(ScreenManager.BlankTexture,
                                            new DrawingRectangle(x*PacmanGame.TileWidth + i, y*PacmanGame.TileWidth, 2, 1),
                                            DebugBorderColor);

                    for (int i = 0; i < 30; i += 6)
                        spriteBatch.Draw(ScreenManager.BlankTexture,
                                            new DrawingRectangle(x*PacmanGame.TileWidth, y*PacmanGame.TileWidth + i, 1, 2),
                                            DebugBorderColor);
                }
            }

#endif

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

        #region Tile Methods

        public static bool IsLegal(Vector2 position)
        {
            try
            {
                return LegalTiles[(int) position.X, (int) position.Y] == (int) TileType.Empty;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public static bool IsLegal(int x, int y)
        {
            return LegalTiles[x, y] == (int) TileType.Empty;
        }

        public static Rectangle TileBounds(Vector2 position)
        {
            int xPos = (int) position.X*PacmanGame.TileWidth;
            int yPos = (int) position.Y*PacmanGame.TileWidth;
            return new Rectangle(xPos, yPos, xPos + PacmanGame.TileWidth, yPos + PacmanGame.TileWidth);
        }

        #endregion

        #region Generate Level

        static Level()
        {
            LegalTiles = new int[,] {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1}, 
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            };

            DotTiles = new int[,] {
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1, 1, 2, 1, 1, 1, 1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1, 1, 1, 2,-1,-1, 1, 1, 1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1, 1, 1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1,-1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1,-1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1, 1, 1, 1,-1,-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,-1,-1, 1, 1, 1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1, 0,-1,-1,-1,-1,-1, 0,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1, 0,-1,-1,-1,-1,-1, 0,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1, 1, 1, 1, 1,-1,-1, 1, 0, 0, 0,-1,-1,-1,-1,-1, 0,-1,-1, 1, 1, 1, 1,-1,-1, 1, 1, 1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1,-1,-1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1,-1,-1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1, 1, 1, 1, 1,-1,-1, 1, 0, 0, 0,-1,-1,-1,-1,-1, 0,-1,-1, 1, 1, 1, 1,-1,-1, 1, 1, 1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1, 0,-1,-1,-1,-1,-1, 0,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1, 0,-1,-1,-1,-1,-1, 0,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1, 1, 1, 1,-1,-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,-1,-1, 1, 1, 1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1,-1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1,-1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1, 1, 1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1,-1,-1,-1, 1,-1,-1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1, 1, 1, 2, 1, 1, 1, 1, 1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1, 1, 1, 1, 2,-1,-1, 1, 1, 1, 1,-1,-1,-1 }, 
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1, 0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }
            };
        }

        #endregion
    }
}
