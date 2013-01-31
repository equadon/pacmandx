using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.ScreenMachine.Menu
{
    public class MenuTextItem : MenuItem
    {
        private float _selectionFade;

        public string Text { get; private set; }

        public MenuTextItem(string text)
        {
            Text = text;
        }

        public override void Update(MenuScreen screen, bool isSelected, SharpDX.Toolkit.GameTime gameTime)
        {
            // Gradually fade between selected/deselected appearance
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
            else
                _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
        }

        public override void Draw(MenuScreen screen, bool isSelected, SharpDX.Toolkit.GameTime gameTime)
        {
            Color color = isSelected ? Color.Yellow : Color.Black;

            // Pulsate thet size of the selected entry
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * _selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            SpriteFont font = screen.ScreenManager.MenuFont;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2f);

            spriteBatch.DrawString(font, Text, Position, color, 0f,
                origin, scale, SpriteEffects.None, 0f);
        }
    }
}
