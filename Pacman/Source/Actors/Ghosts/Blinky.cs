using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Blinky : Ghost
    {
        public Blinky(Texture2D texture, Vector2 position)
            : base(texture, position, new Rectangle(0, 0, 5, 5))
        {
            Color = Color.Red;

            Direction = Direction.Left;
            Velocity = GetVelocity();

            TargetTile = new Vector2(11, 11);

            NextPosition = GridPosition;
            FutureDirection = Direction.Left;
        }
    }
}
