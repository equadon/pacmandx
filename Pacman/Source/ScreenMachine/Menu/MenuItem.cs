using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Pacman.ScreenMachine.Menu
{
    public class MenuItem
    {
        private Vector2 _position;

        #region Properties

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #endregion

        #region Events

        /// <summary>Event raised when the menu item is selected.</summary>
        public event EventHandler Selected;

        /// <summary>Method for raising the Selected event.</summary>
        protected internal virtual void OnSelectItem()
        {
            if (Selected != null)
                Selected(this, null);
        }

        #endregion

        public MenuItem()
        {
        }

        /// <summary>Update menu item.</summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
        }
                
        /// <summary>
        /// Draws the menu item. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
        }
    }
}
