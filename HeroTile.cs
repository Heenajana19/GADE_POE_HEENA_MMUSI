namespace GADE_POE_HEENA_JANA
{
    public class HeroTile : CharacterTile
    {
        public HeroTile(Position position) : base(position, 40, 5)
        {
        }

        public override TileType GetTileType()
        {
            return TileType.Hero;
        }

        public override string ToString()
        {
            return IsDead ? "x" : "▼";
        }
    }
}
