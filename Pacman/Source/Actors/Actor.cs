using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Actor : Sprite
    {
        public const float BaseActorVelocity = 1.0f;

        #region Properties

        protected Vector2 Velocity { get; set; }

        public Direction Direction { get; protected set; }

        #endregion

        public Actor(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
        }
    }
}
