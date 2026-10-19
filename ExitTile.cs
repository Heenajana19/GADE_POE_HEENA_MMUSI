
namespace GADE_POE_HEENA_JANA
{
    public class ExitTile : Tile
    {
        private bool isLocked;

        public bool IsLocked
        {
            get { return isLocked; }
        }

        public string Display
        {
            get
            {
                if (isLocked)
                {
                    return "L";
                }

                return "E";
            }
        }

        public ExitTile(Position position)
            : base(position)
        {
            isLocked = true;
        }

        public void Unlock()
        {
            isLocked = false;
        }

        public override TileType GetTileType()
        {
            return TileType.Exit;
        }
    }
}
