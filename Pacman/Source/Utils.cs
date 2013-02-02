using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Pacman
{
    public static class Utils
    {
        /// <summary>
        /// Performs a linear interoplation between float values.
        /// </summary>
        /// <param name="start">Start value.</param>
        /// <param name="end">End value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weigth of <paramref name="end"/>.</param>
        /// <returns>The linear interpolation of the two values.</returns>
        public static float Lerp(float start, float end, float amount)
        {
            return (start + amount * (end - start));
        }

        /// <summary>
        /// Converts an absolution position to grid position.
        /// </summary>
        public static Vector2 AbsToGrid(Vector2 absPosition)
        {
            return new Vector2(
                Convert.ToInt32(Math.Floor(absPosition.X / PacmanGame.TileWidth)),
                Convert.ToInt32(Math.Floor(absPosition.Y / PacmanGame.TileWidth)));
        }

        /// <summary>
        /// Converts a grid position to absolute position.
        /// </summary>
        public static Vector2 GridToAbs(Vector2 gridPosition, Vector2 origin)
        {
            return new Vector2(
                gridPosition.X * PacmanGame.TileWidth + origin.X,
                gridPosition.Y * PacmanGame.TileWidth + origin.Y);
        }
    }
}
