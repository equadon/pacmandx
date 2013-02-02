using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Blinky : Ghost
    {
        public Blinky(Texture2D texture, Vector2 position)
            : base(texture, position, new Rectangle(0, 0, 30, 30))
        {
            TargetTile = new Vector2(15, 11);
            GridPosition = new Vector2(25, 8);
            Direction = Direction.Left;
            NextPosition = GetNextPosition(GridPosition, Direction);

            SpeedModifier = 4f;

            CalculateFutureDirection();
        }
    }
}
