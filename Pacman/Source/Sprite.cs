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

        public Rectangle SourceRect { get; protected set; }

        public Rectangle Bounds
        {
            get
            {
                int x = (int)Position.X;
                int y = (int)Position.Y;
                int originX = (int) Origin.X;
                int originY = (int)Origin.Y;
                return new Rectangle(x - originX, y - originY, x + originX, y + originY);
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            protected set { _position = new Vector2((float)Math.Round(value.X, 0), (float)Math.Round(value.Y, 0)); }
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
            get { return new Vector2(SourceRect.Width / 2f, SourceRect.Height / 2f); }
        }

        #endregion

        public Sprite(Texture2D texture, Vector2 position, Rectangle sourceRect)
        {
            _texture = texture;
            _position = position;

            SourceRect = sourceRect;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, Position, SourceRect, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
