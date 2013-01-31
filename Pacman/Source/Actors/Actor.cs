using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman
{
    public class Actor : Sprite
    {
        public Actor(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
        }
    }
}
