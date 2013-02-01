using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
