using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class Ghost : Actor
    {
        public Vector2 TargetTile { get; protected set; }

        public Ghost(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
        }


    }
}
