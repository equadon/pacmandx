using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Inky : Ghost
    {
        public Inky(Level level, Texture2D texture, Vector2 position)
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
            switch (Level.GhostMode)
            {
                case GhostMode.Scatter:
                    TargetTile = new Vector2(27, 35);
                    break;
                case GhostMode.Frightened:
                    TargetTile = new Vector2(27, 35);
                    break;
                case GhostMode.Chase:
                    var pacmanOffset = GetNextPosition(Level.PacMan.GridPosition, Level.PacMan.Direction, 2);

                    // Original game had a bug which caused the upward pos to be both 4 above and 4 to the left of pacman
                    if (Level.PacMan.Direction == Direction.Up)
                        pacmanOffset.X -= 2;

                    Vector2 length = pacmanOffset - Level.Blinky.GridPosition;

                    TargetTile = pacmanOffset + length;
                    break;
            }
        }
    }
}
