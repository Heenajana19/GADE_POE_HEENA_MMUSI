using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public abstract class PickupTile : Tile
    {
        protected PickupTile(Position position) : base(position)
        {
        }

        public abstract void ApplyEffect(CharacterTile character);
    }
}
