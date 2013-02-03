using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman
{
    public class Sprite
    {
        #region Fields

        private readonly Texture2D _texture;

        protected Vector2 _position;

        #endregion

        #region Properties

        public Color Color { get; protected set; }

        public Rectangle SourceRect { get; protected set; }

        public Level Level { get; private set; }

        public RectangleF Bounds
        {
            get
            {
                float x = Position.X;
                float y = Position.Y;
                float originX = Origin.X;
                float originY = Origin.Y;
                return new RectangleF(x - originX, y - originY, x + originX, y + originY);
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            protected set { _position = value; }
        }

        /// <summary>
        /// Return the Sprite's grid position. The setter will update Position
        /// and place it in the middle of the specified tile.
        /// </summary>
        public Vector2 GridPosition
        {
            get { return Utils.AbsToGrid(_position); }
            set { _position = Utils.GridToAbs(value, Origin); }
        }

        public Vector2 Origin
        {
            get { return new Vector2(SourceRect.Width * Scale / 2f, SourceRect.Height * Scale / 2f); }
        }

        public static float Scale
        {
            get { return PacmanGame.TileWidth/30f; }
        }

        #endregion

        public Sprite(Level level, Texture2D texture, Vector2 position, Rectangle sourceRect)
        {
            _texture = texture;
            _position = position;

            Level = level;
            SourceRect = sourceRect;

            Color = Color.White;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, Position, SourceRect, Color, 0f, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
