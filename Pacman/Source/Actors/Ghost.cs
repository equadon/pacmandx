using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class Ghost : Actor
    {
        public Ghost(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
        }
    }
}
