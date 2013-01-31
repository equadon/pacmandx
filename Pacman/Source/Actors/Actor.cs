using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class Actor : Sprite
    {
        protected Vector2 Velocity { get; set; }

        public Actor(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
        }
    }
}
