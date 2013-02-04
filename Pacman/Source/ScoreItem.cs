using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman
{
    /// <summary>
    /// An object that Pac-Man can eat to increase his score.
    /// </summary>
    public class ScoreItem : Sprite
    {
        public TileItem ItemType { get; private set; }
        public int Points { get; private set; }

        public ScoreItem(TileItem itemType, int points, Level level, Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(level, texture, position, sourceRect)
        {
            ItemType = itemType;
            Points = points;
        }
    }
}
