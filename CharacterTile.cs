
using System;

namespace GADE_POE_HEENA_JANA
{
    public abstract class CharacterTile : Tile
    {
        protected int hitPoints;
        protected int maxHitPoints;
        protected int attackPower;

        protected Tile[] vision;

        public CharacterTile(Position position, int hitPoints, int attackPower)
            : base(position)
        {
            this.hitPoints = hitPoints;
            this.maxHitPoints = hitPoints;
            this.attackPower = attackPower;

            vision = new Tile[4];
        }

        public void UpdateVision(Level level)
        {
            // The character's vision will be updated here.
        }
    }
}
