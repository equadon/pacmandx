﻿using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Blinky : Ghost
    {
        public Blinky(Level level, Texture2D texture, Vector2 position)
            : base(level, texture, position, new DrawingRectangle(3, 2, 48, 51))
        {
            Direction = Direction.Left;

            FutureDirection = Direction.Left;
        }

        /// <summary>
        /// Blinky
        /// Chase Mode: Targets the tile Pacman currently occupies
        /// </summary>
        public override void UpdateTarget()
        {
            switch (Level.GhostMode)
            {
                case GhostMode.Scatter:
                    TargetTile = new Vector2(25, 0);
                    break;
                case GhostMode.Frightened:
                    TargetTile = new Vector2(25, 0);
                    break;
                case GhostMode.Chase:
                    TargetTile = Level.PacMan.GridPosition;
                    break;
            }
        }
    }
}
