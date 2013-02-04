using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.ScreenMachine.Menu
{
    public abstract class MenuScreen : GameScreen
    {
        #region Fields & Properties

        private List<MenuItem> _menuItems = new List<MenuItem>();

        protected IList<MenuItem> MenuItems
        {
            get { return _menuItems; }
        }

        public int SelectedEntry { get; private set; }

        public string MenuTitle { get; private set; }

        #endregion

        protected MenuScreen(string menuTitle)
        {
            MenuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, Input input)
        {
            // Move to the previous menu entry?
            if (input.IsKeyPressed(Key.UpArrow))
            {
                SelectedEntry--;

                if (SelectedEntry < 0)
                    SelectedEntry = MenuItems.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsKeyPressed(Key.Down))
            {
                SelectedEntry++;

                if (SelectedEntry >= MenuItems.Count)
                    SelectedEntry = 0;
            }

            if (input.IsKeyPressed(Key.Return))
            {
                OnSelectEntry(SelectedEntry);
            }
            else if (input.IsKeyPressed(Key.Escape))
            {
                OnCancel();
            }
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int itemIndex)
        {
            MenuItems[itemIndex].OnSelectItem();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuItem event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuItemLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItem menuItem = MenuItems[i];

                // each entry is to be centered horizontally
                //position.X = ScreenManager.Game.GraphicsDevice.Viewport.Width / 2 - MenuTextItem.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuItem.Position = position;

                // move down for the next entry the size of this entry
                //position.Y += menuTextItem.GetHeight(this);
                position.Y += 100;
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuTextItem object.
            for (int i = 0; i < MenuItems.Count; i++)
            {
                bool isSelected = IsActive && (i == SelectedEntry);

                MenuItems[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuItemLocations();

            //GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.MenuFont;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItem menuItem = MenuItems[i];

                bool isSelected = IsActive && (i == SelectedEntry);

                menuItem.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            //Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2f, 80);
            Vector2 titlePosition = new Vector2(800 / 2f, 80);
            Vector2 titleOrigin = font.MeasureString(MenuTitle) / 2;
            Color3 titleColor = new Color3(50, 50, 50) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, MenuTitle, titlePosition, (Color)titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
