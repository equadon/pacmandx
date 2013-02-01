using System;
//using System.Windows.Forms;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Multimedia;

namespace Pacman.ScreenMachine
{
    public enum MouseButton { Left, Middle, Right }

    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices.
    /// </summary>
    public class InputState
    {
        private DirectInput _directInput;

        private Keyboard _keyboard;
        private Mouse _mouse;

        #region Properties

        public KeyboardState LastKeyboardState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        public MouseState MouseState { get; private set; }

        #endregion

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            _directInput = new DirectInput();

            // Initialize keyboard
            _keyboard = new Keyboard(_directInput);
            _keyboard.Properties.BufferSize = 256;

            // Set the cooperative level of the keyboard to not share with other programs.
            //_keyboard.SetCooperativeLevel(windowsHandle, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);

            _keyboard.Acquire();

            // Initialize mouse
            _mouse = new Mouse(_directInput);
            _mouse.Properties.AxisMode = DeviceAxisMode.Relative;

            // Set the cooperative level of the mouse to share with other programs.
            //_mouse.SetCooperativeLevel(windowsHandle, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);

            _mouse.Acquire();

            LastKeyboardState = new KeyboardState();
            KeyboardState = new KeyboardState();
        }

        /// <summary>
        /// Reads the latest state user input.
        /// </summary>
        public void Update()
        {
            ReadKeyboard();

            ReadMouse();
        }

        private void ReadKeyboard()
        {
            LastKeyboardState = KeyboardState;
            KeyboardState = _keyboard.GetCurrentState();
        }

        private void ReadMouse()
        {
            var resultCode = ResultCode.Ok;

            MouseState = new MouseState();

            // Read mouse device
            MouseState = _mouse.GetCurrentState();
        }

        public void Shutdown()
        {
            if (_keyboard != null)
            {
                _keyboard.Unacquire();
                _keyboard.Dispose();
                _keyboard = null;
            }

            if (_mouse != null)
            {
                _mouse.Unacquire();
                _mouse.Dispose();
                _mouse = null;
            }

            if (_directInput != null)
            {
                _directInput.Dispose();
                _directInput = null;
            }
        }

        #region Mouse Button Handling

        /// <summary>
        /// Helper for checking if a mouse button was pressed during this update.
        /// </summary>
        public bool IsMousePressed(MouseButton button)
        {
            return false;
        }

        /// <summary>
        /// Helper for checking if a mouse button was down during this update.
        /// </summary>
        public bool IsMouseDown(MouseButton button)
        {
            return false;
        }

        /// <summary>
        /// Helper for checking if a mouse button was released during this update.
        /// </summary>
        public bool IsMouseReleased(MouseButton button)
        {
            return false;
        }

        #endregion

        #region Keyboard Handling

        /// <summary>
        /// Helper for checking if a key was pressed during this update.
        /// </summary>
        public bool IsKeyPressed(Key key)
        {
            return (!LastKeyboardState.IsPressed(key) &&
                    KeyboardState.IsPressed(key));
        }

        /// <summary>
        /// Helper for checking if a key is down during this update.
        /// </summary>
        public bool IsKeyDown(Key key)
        {
            return KeyboardState.IsPressed(key);
        }

        /// <summary>
        /// Helper for checking if a key was released during this update.
        /// </summary>
        public bool IsKeyReleased(Key key)
        {
            return (LastKeyboardState.IsPressed(key) &&
                    !KeyboardState.IsPressed(key));
        }

        #endregion
    }
}
