using System;
using System.Collections.Generic;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class WarlockTile : EnemyTile
    {
        public WarlockTile(Position position, Level level)
            : base(position, level, 10, 5)
        {
        }

        public override char Display
        {
            get { return IsDead ? 'x' : 'ᐂ'; }
        }

        public override bool GetMove(out Tile tile)
        {
            tile = null;
            return false;
        }

        public override CharacterTile[] GetTargets()
        {
            List<CharacterTile> targets = new List<CharacterTile>();

            if (IsDead)
            {
                return targets.ToArray();
            }

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    CharacterTile target =
                        level.GetTile(X + dx, Y + dy) as CharacterTile;

                    if (target != null && !target.IsDead)
                    {
                        targets.Add(target);
                    }
                }
            }

            return targets.ToArray();
        }
    }
}
