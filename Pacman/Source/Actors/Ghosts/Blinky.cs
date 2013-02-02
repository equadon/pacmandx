using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Blinky : Ghost
    {
        public Blinky(Texture2D texture, Vector2 position)
            : base(texture, position, new DrawingRectangle(3, 2, 48, 51))
        {
            Direction = Direction.Left;
            Velocity = GetVelocity();

            TargetTile = new Vector2(11, 11);

            NextPosition = GridPosition;
            FutureDirection = Direction.Left;
        }
    }
}
