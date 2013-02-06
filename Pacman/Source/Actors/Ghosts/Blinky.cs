using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Blinky : Ghost
    {
        public bool IsElroy { get; private set; }

        public Blinky(Level level, Texture2D texture, Vector2 position)
            : base(level, texture, position, new DrawingRectangle(3, 2, 48, 51))
        {
            Direction = Direction.Left;

            FutureDirection = Direction.Left;

            IsElroy = false;
        }

        /// <summary>
        /// Blinky
        /// Chase Mode: Targets the tile Pacman currently occupies
        /// Scatter Mode: If Blinky is Elroy he'll use his Chase Mode in Scatter Mode.
        /// </summary>
        public override void UpdateTarget()
        {
            if (IsDead)
            {
                TargetTile = new Vector2(13, 14);
                return;
            }

            switch (Level.GhostMode)
            {
                case GhostMode.Scatter:
                    TargetTile = new Vector2(25, 0);
                    break;
                case GhostMode.Chase:
                    TargetTile = Level.PacMan.GridPosition;
                    break;
            }
        }

        /// <summary>
        /// Update SpeedModifier.
        /// </summary>
        /// <param name="currentLevel">Current level we're on.</param>
        /// <param name="dotsLeft">Dots left remaining on this level.</param>
        public void HandleElroy(int currentLevel, int dotsLeft)
        {
            if (dotsLeft <= 0)
                return;

            float speed = 0;
            int dotsLeftRange = 0;

            if (currentLevel == 1)
            {
                speed = 0.8f;
                dotsLeftRange = 20;
            }
            else if (currentLevel == 2)
            {
                speed = 0.9f;
                dotsLeftRange = 30;
            }
            else if (currentLevel >= 3 && currentLevel <= 4)
            {
                speed = 0.9f;
                dotsLeftRange = 40;
            }
            else if (currentLevel == 5)
            {
                speed = 1f;
                dotsLeftRange = 40;
            }
            else if (currentLevel >= 6 && currentLevel <= 8)
            {
                speed = 1f;
                dotsLeftRange = 50;
            }
            else if (currentLevel >= 9 && currentLevel <= 11)
            {
                speed = 1f;
                dotsLeftRange = 60;
            }
            else if (currentLevel >= 12 && currentLevel <= 14)
            {
                speed = 1f;
                dotsLeftRange = 80;
            }
            else if (currentLevel >= 15 && currentLevel <= 18)
            {
                speed = 1f;
                dotsLeftRange = 100;
            }
            else if (currentLevel >= 19)
            {
                speed = 1f;
                dotsLeftRange = 120;
            }

            // Check which elroy state Blinky is
            if (dotsLeft <= dotsLeftRange / 2)
            {
                SpeedModifier = speed + 0.05f;
                IsElroy = true;
            }
            else if (dotsLeft <= dotsLeftRange)
            {
                SpeedModifier = speed;
                IsElroy = true;
            }
        }
    }
}
