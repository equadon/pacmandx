﻿using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman
{
    public class Sprite
    {
        #region Fields

        private readonly Texture2D _texture;

        private Vector2 _position;

        #endregion

        #region Properties

        public Rectangle SourceRect { get; protected set; }

        public Vector2 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Return the Sprite's grid position. The setter will update Position
        /// and place it in the middle of the specified tile.
        /// </summary>
        public Vector2 GridPosition
        {
            get { return Position; }
            set
            {
                _position = new Vector2(
                    value.X * PacmanGame.TileWidth + Origin.X,
                    value.Y * PacmanGame.TileWidth + Origin.Y);
            }
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

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, Position, SourceRect, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
