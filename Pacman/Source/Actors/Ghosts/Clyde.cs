using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Clyde : Ghost
    {
        public Clyde(Level level, Texture2D texture, Vector2 position)
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
                    TargetTile = new Vector2(0, 35);
                    break;
                case GhostMode.Chase:
                    float distance = Vector2.Distance(Level.PacMan.GridPosition, GridPosition);

                    TargetTile = (distance > 8f) ? Level.PacMan.GridPosition : new Vector2(0, 35);
                    break;
            }
        }
    }
}
