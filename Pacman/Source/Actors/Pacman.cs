using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class PacMan : Actor
    {
        public PacMan(Texture2D texture, Vector2 position)
            : base(texture, position, new Rectangle(67, 1, 97, 32))
        {
        }
    }
}
