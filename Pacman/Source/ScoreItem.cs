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
                    FlashSourceRect = new DrawingRectangle(59, 3, 42, 49);
                    break;
                case TileItem.Strawberry:
                    Points = 300;
                    FlashSourceRect = new DrawingRectangle(168, 2, 45, 50);
                    break;
                case TileItem.Peach:
                    Points = 500;
                    FlashSourceRect = new DrawingRectangle(166, 55, 47, 52);
                    break;
                case TileItem.Apple:
                    Points = 700;
                    FlashSourceRect = new DrawingRectangle(57, 55, 47, 51);
                    break;
                case TileItem.Grapes:
                    Points = 1000;
                    FlashSourceRect = new DrawingRectangle(56, 111, 50, 49);
                    break;
                case TileItem.Galaxian:
                    Points = 2000;
                    FlashSourceRect = new DrawingRectangle(163, 111, 52, 49);
                    break;
                case TileItem.Bell:
                    Points = 3000;
                    FlashSourceRect = new DrawingRectangle(56, 164, 49, 50);
                    break;
                case TileItem.Key:
                    Points = 5000;
                    FlashSourceRect = new DrawingRectangle(175, 166, 29, 50);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Duration > 0d)
                Duration -= gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
