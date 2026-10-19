namespace GADE_POE_HEENA_JANA
{
    public abstract class Tile
    {
        public Position Position { get; set; }

        public enum TileType
        {
            Hero,
            Wall,
            Empty,
            Exit,
            Grunt,
            HealthPickup,
            Enemy,
            Warlock,
            Tyrant
        }

        protected Tile(Position position)
        {
            Position = position;
        }

        public abstract TileType GetTileType();
    }
}
