using SharpDX;
using SharpDX.Toolkit;
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
        public double Duration { get; private set; }

        public ScoreItem(TileItem itemType, double duration, Level level, Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(level, texture, position, sourceRect)
        {
            ItemType = itemType;
            Duration = duration;

            // Set points
            switch (itemType)
            {
                case TileItem.Dot:
                    Points = 10;
                    break;
                case TileItem.Energizer:
                    Points = 50;
                    break;
                case TileItem.Cherries:
                    Points = 100;
                    break;
                case TileItem.Strawberry:
                    Points = 300;
                    break;
                case TileItem.Peach:
                    Points = 500;
                    break;
                case TileItem.Apple:
                    Points = 700;
                    break;
                case TileItem.Grapes:
                    Points = 1000;
                    break;
                case TileItem.Galaxian:
                    Points = 2000;
                    break;
                case TileItem.Bell:
                    Points = 3000;
                    break;
                case TileItem.Key:
                    Points = 5000;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Duration > 0d)
                Duration -= gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
