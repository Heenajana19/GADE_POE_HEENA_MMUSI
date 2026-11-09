using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class WallTile : Tile
    {
        public WallTile(Position position) : base(position)
        {
        }

        public override char Display
        {
            get { return '█'; }
        }
    }
}
