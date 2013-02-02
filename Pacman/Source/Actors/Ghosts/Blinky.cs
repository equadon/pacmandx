using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors.Ghosts
{
    public class Blinky : Ghost
    {
        public Blinky(Texture2D texture, Vector2 position)
            : base(texture, position, new Rectangle(0, 0, 30, 30))
        {
            TargetTile = new Vector2(9, 9);
            GridPosition = new Vector2(13, 14);
            Direction = Direction.Right;
            NextPosition = GetNextPosition(GridPosition, Direction);

            SpeedModifier = 2f;

            CalculateFutureDirection();
        }
    }
}
