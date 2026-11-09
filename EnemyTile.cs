using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public abstract class EnemyTile : CharacterTile
    {
        protected Level level;

        protected EnemyTile(
            Position position,
            Level level,
            int hitPoints,
            int attackPower)
            : base(position, hitPoints, attackPower)
        {
            this.level = level;
        }

        public abstract bool GetMove(out Tile tile);

        public abstract CharacterTile[] GetTargets();
    }
}
