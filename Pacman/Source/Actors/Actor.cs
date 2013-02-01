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

        /// <summary>
        /// Move the actor in the chosen direction. Override this for
        /// customized behavior.
        /// </summary>
        /// <param name="direction">The direction to move. This will be null for
        /// ghosts since their movement will be decided by their pathfinding.</param>
        public virtual void Move(Direction? direction = null)
        {
        }
    }
}
