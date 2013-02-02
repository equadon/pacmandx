﻿using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class PacMan : Actor
    {
        public PacMan(Texture2D texture, Vector2 position)
            : base(texture, position, new DrawingRectangle(3, 3, 48, 48))
        {
            SpeedModifier = 1f;
            Direction = Direction.Left;
        }

        /// <summary>
        /// We reached the center of the tile, make sure we don't run into a wall.
        /// </summary>
        public override void OnTileCenter(Rectangle currentTileBounds)
        {
            base.OnTileCenter(currentTileBounds);

            // Stop if the next position is an illegal tile.
            if (!Level.IsLegal(GetNextPosition(GridPosition, Direction)))
            {
                Position -= Velocity;
                Velocity = Vector2.Zero;
            }
        }

        /// <summary>
        /// Request direction change.
        /// 
        /// TODO: Note that pacman can take shortcuts if he requests direction a few pixels before ghosts can.
        /// </summary>
        /// <param name="direction">New direction</param>
        /// <returns>True if the direction change was performed.</returns>
        public bool ChangeDirection(Direction direction)
        {
            if (direction == Direction)
                return false;

            if (Level.IsLegal(GetNextPosition(GridPosition, direction)))
            {
                Direction = direction;
                Velocity = GetVelocity();

                return true;
            }

            return false;
        }
    }
}
