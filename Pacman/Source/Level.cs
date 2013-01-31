namespace Pacman
{
    public enum TileType
    {
        Empty,
        Dot,
        Energizer,
        Cherries, // level 1
        Strawberry, // level 2
        Peach, // level 3-4
        Apple, // level 5-6
        Grapes, // level 7-8
        Galaxian, // level 9-10
        Bell, // level 11-12
        Key, // level 13+
    }

    public class Level
    {
        public static readonly int TilesWide = 28;
        public static readonly int TilesHigh = 36;

        /// <summary>
        /// Array listing legal tiles that will be used by the pathfinding.
        /// 1 means tile is legal to move onto, 0 means illegal.
        /// </summary>
        private static readonly int[,] LegalTiles = new int[TilesWide, TilesHigh];

        /// <summary>
        /// Array storing the state for the current level. Use the TileType
        /// enum for IDs.
        /// </summary>
        private readonly int[,] _tiles = new int[TilesWide, TilesHigh];
    }
}
