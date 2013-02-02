using System;
using SharpDX;
using SharpDX.Toolkit;
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
        public const float BaseSpeedModifer = 1.0f;

        #region Properties

        public float SpeedModifier { get; set; }

        public Direction Direction { get; protected set; }

        protected Vector2 Velocity
        {
            get
            {
                switch (Direction)
                {
                    case Direction.Up:
                        return new Vector2(0, -1);
                    case Direction.Down:
                        return new Vector2(0, 1);
                    case Direction.Left:
                        return new Vector2(-1, 0);
                    default:
                        return new Vector2(1, 0);
                }
            }
        }

        #endregion

        public Actor(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
            SpeedModifier = BaseSpeedModifer;
        }

        public override void Update(GameTime gameTime)
        {
            _position += Velocity * SpeedModifier;
        }

        public virtual void Move(Direction? direction = null)
        {
            if (direction != Direction)
                return;

            const float moveDistance = 1.4f;

            switch (direction)
            {
                case Direction.Up:
                    _position.Y -= moveDistance;
                    break;
                case Direction.Down:
                    _position.Y += moveDistance;
                    break;
                case Direction.Left:
                    _position.X -= moveDistance;
                    break;
                case Direction.Right:
                    _position.X += moveDistance;
                    break;
            }
        }
    }
}
