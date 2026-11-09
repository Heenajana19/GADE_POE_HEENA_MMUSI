using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class HeroTile : CharacterTile
    {
        public HeroTile(Position position)
            : base(position, 40, 5)
        {
        }

        public override char Display
        {
            get { return IsDead ? 'x' : '▼'; }
        }
    }
}
