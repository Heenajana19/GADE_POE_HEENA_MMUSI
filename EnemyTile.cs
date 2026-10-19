using System;

namespace GADE_POE_HEENA_JANA
{
    public abstract class EnemyTile : CharacterTile
    {
        protected Level Level;

        protected EnemyTile(
            Position position,
            Level level,
            int health,
            int attackPower)
            : base(position, health, attackPower)
        {
            Level = level;
        }

        public abstract bool GetMove(out Tile tile);

        public abstract CharacterTile[] GetTargets();
    }
}
