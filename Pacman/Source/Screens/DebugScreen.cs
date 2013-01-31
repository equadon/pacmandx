using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pacman.ScreenMachine;
using SharpDX.Toolkit.Diagnostics;

namespace Pacman.Screens
{
    public class DebugScreen : GameScreen
    {
        public Logger Logger { get; private set; }

        public DebugScreen(Logger logger)
        {
            Logger = logger;
        }
    }
}
