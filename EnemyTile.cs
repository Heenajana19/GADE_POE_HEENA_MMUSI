using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class EmptyTile : Tile
    {
        public EmptyTile(Position position) : base(position)
        {
        }

        public override char Display
        {
            get { return '.'; }
        }
    }
}
