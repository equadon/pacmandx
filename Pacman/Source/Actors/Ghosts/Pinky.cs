using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Pinky : Ghost
    {
        public Pinky(Level level, Texture2D texture, Vector2 position)
            : base(level, texture, position, new DrawingRectangle(3, 2, 48, 51))
        {
            Direction = Direction.Left;
            Velocity = GetVelocity();

            NextPosition = GridPosition;
            FutureDirection = Direction.Left;
        }

        /// <summary>
        /// Blinky
        /// Chase Mode: Targets the tile Pacman currently occupies
        /// </summary>
        public override void UpdateTarget()
        {
            switch (CurrentMode)
            {
                case GhostMode.Scatter:
                    TargetTile = new Vector2(2, 0);
                    break;
                case GhostMode.Frightened:
                    TargetTile = Vector2.Zero;
                    break;
                case GhostMode.Chase:
                    Vector2 target = GetNextPosition(Level.PacMan.GridPosition, Level.PacMan.Direction, 4);

                    // Original game had a bug which caused the upward pos to be both 4 above and 4 to the left of pacman
                    if (Level.PacMan.Direction == Direction.Up)
                        target.X -= 4;

                    TargetTile = target;
                    break;
            }
        }
    }
}
