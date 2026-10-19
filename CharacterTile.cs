
namespace GADE_POE_HEENA_JANA
{
    public abstract class CharacterTile : Tile
    {
        protected int hitPoints;
        protected int maxHitPoints;
        protected int attackPower;

        protected Tile[] vision;

        public CharacterTile(
            Position position,
            int hitPoints,
            int attackPower)
            : base(position)
        {
            this.hitPoints = hitPoints;
            this.maxHitPoints = hitPoints;
            this.attackPower = attackPower;

            vision = new Tile[4];
        }

        public void UpdateVision(Level level)
        {
            int x = Position.X;
            int y = Position.Y;

            // Up
            vision[0] = y > 0
                ? level.Tiles[x, y - 1]
                : null;

            // Right
            vision[1] = x < level.Width - 1
                ? level.Tiles[x + 1, y]
                : null;

            // Down
            vision[2] = y < level.Height - 1
                ? level.Tiles[x, y + 1]
                : null;

            // Left
            vision[3] = x > 0
                ? level.Tiles[x - 1, y]
                : null;
        }

        public override TileType GetTileType()
        {
            return TileType.Empty;
        }
    }
}
