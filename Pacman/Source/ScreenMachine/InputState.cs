using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.RawInput;

namespace Pacman.ScreenMachine
{
    public enum MouseButton { Left, Middle, Right }

    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public KeyState CurrentKeyState { get; private set; }
        public KeyState LastKeyState { get; private set; }

        public Keys CurrentKey { get; private set; }
        public Keys LastKey { get; private set; }

        //public MouseState CurrentMouseState { get; private set; }
        //public KeyState LastMouseState { get; private set; }

        public MouseButtonFlags CurrentMouseButtonFlags { get; private set; }
        public MouseButtonFlags LastMouseButtonFlags { get; private set; }

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None);
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None);

            Device.KeyboardInput += new System.EventHandler<KeyboardInputEventArgs>(Device_KeyboardInput);
            Device.MouseInput += new System.EventHandler<MouseInputEventArgs>(Device_MouseInput);
        }

        public void Device_KeyboardInput(object sender, KeyboardInputEventArgs args)
        {
            LastKeyState = CurrentKeyState;
            LastKey = CurrentKey;

            CurrentKeyState = args.State;
            CurrentKey = args.Key;
        }

        public void Device_MouseInput(object sender, MouseInputEventArgs args)
        {
            LastMouseButtonFlags = CurrentMouseButtonFlags;
            CurrentMouseButtonFlags = args.ButtonFlags;
        }

        /// <summary>
        /// Reads the latest state user input.
        /// </summary>
        public void Update()
        {
        }

        #region Mouse Button Handling

        /// <summary>
        /// Helper for checking if a mouse button was pressed during this update.
        /// </summary>
        public bool IsMousePressed(MouseButton button)
        {
            var down = MouseDownFlags(button);

            if (down == MouseButtonFlags.None)
                return false;

            if (LastMouseButtonFlags != down &&
                CurrentMouseButtonFlags == down)
            {
                LastMouseButtonFlags = CurrentMouseButtonFlags;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper for checking if a mouse button was down during this update.
        /// </summary>
        public bool IsMouseDown(MouseButton button)
        {
            var down = MouseDownFlags(button);
            if (down == MouseButtonFlags.None)
                return false;

            if (CurrentMouseButtonFlags == down)
            {
                LastMouseButtonFlags = CurrentMouseButtonFlags;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper for checking if a mouse button was released during this update.
        /// </summary>
        public bool IsMouseReleased(MouseButton button)
        {
            var up = MouseUpFlags(button);

            if (up == MouseButtonFlags.None)
                return false;

            if (LastMouseButtonFlags != up &&
                CurrentMouseButtonFlags == up)
            {
                LastMouseButtonFlags = CurrentMouseButtonFlags;
                return true;
            }
            return false;
        }

        public MouseButtonFlags MouseDownFlags(MouseButton button)
        {
            var down = MouseButtonFlags.None;
            switch (button)
            {
                case MouseButton.Left:
                    down = MouseButtonFlags.LeftButtonDown;
                    break;
                case MouseButton.Right:
                    down = MouseButtonFlags.RightButtonDown;
                    break;
                case MouseButton.Middle:
                    down = MouseButtonFlags.MiddleButtonDown;
                    break;
            }
            return down;
        }

        public MouseButtonFlags MouseUpFlags(MouseButton button)
        {
            var up = MouseButtonFlags.None;
            switch (button)
            {
                case MouseButton.Left:
                    up = MouseButtonFlags.LeftButtonUp;
                    break;
                case MouseButton.Right:
                    up = MouseButtonFlags.RightButtonUp;
                    break;
                case MouseButton.Middle:
                    up = MouseButtonFlags.MiddleButtonUp;
                    break;
            }
            return up;
        }

        #endregion

        #region Keyboard Handling

        /// <summary>
        /// Helper for checking if a key was pressed during this update.
        /// </summary>
        public bool IsKeyPressed(Keys key)
        {
            if (LastKeyState == KeyState.KeyUp &&
                (CurrentKeyState == KeyState.KeyDown || CurrentKeyState == KeyState.KeyFirst) &&
                key == CurrentKey)
            {
                LastKeyState = CurrentKeyState;
                LastKey = Keys.None;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper for checking if a key is down during this update.
        /// </summary>
        public bool IsKeyDown(Keys key)
        {
            if (CurrentKeyState == KeyState.KeyDown &&
                CurrentKey == key)
            {
                LastKeyState = KeyState.KeyDown;
                LastKey = CurrentKey;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper for checking if a key was released during this update.
        /// </summary>
        public bool IsKeyReleased(Keys key)
        {
            if ((LastKeyState == KeyState.KeyDown || LastKeyState == KeyState.KeyFirst) &&
                CurrentKeyState == KeyState.KeyUp &&
                key == CurrentKey)
            {
                LastKeyState = CurrentKeyState;
                LastKey = Keys.None;
                CurrentKey = LastKey;
                return true;
            }
            return false;
        }

        #endregion
    }
}
