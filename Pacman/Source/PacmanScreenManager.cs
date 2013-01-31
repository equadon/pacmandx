using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Toolkit;

using Pacman.ScreenMachine;
using SharpDX.Toolkit.Diagnostics;

namespace Pacman
{
    public class PacmanScreenManager : ScreenManager
    {
        public Logger Logger { get; private set; }

        public PacmanScreenManager(Game game, Logger logger)
            : base(game)
        {
            Logger = logger;
        }

        public override void Initialize()
        {
            base.Initialize();

            Logger.Info("Pacman Screen Manager initialized.");
        }
    }
}
