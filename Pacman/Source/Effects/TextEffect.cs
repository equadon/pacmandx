using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Effects
{
    public class TextEffect
    {
        private double _duration;

        #region Properties

        public SpriteFont Font { get; private set; }
        public string Text { get; private set; }

        public Level Level { get; private set; }

        public Vector2 Position { get; private set; }
        public float Scale { get; private set; }

        public Color Color { get; private set; }

        public Vector2 Origin
        {
            get { return Font.MeasureString(Text)/2; }
        }

        #endregion

        public TextEffect(Level level, SpriteFont font, Vector2 position, string text)
        {
            Level = level;
            Font = font;
            Position = position;
            Text = text;
            Scale = 1f;
            Color = Color.Green;
            _duration = 0.8d;
        }

        public void Update(GameTime gameTime)
        {
            Position = new Vector2(Position.X, Position.Y - 1);
            Scale += 0.01f;
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_duration <= 0)
                Level.Effects.Remove(this);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(Font, Text, Position, Color, 0f, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
