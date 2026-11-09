using System;
using System.Collections.Generic;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class TyrantTile : EnemyTile
    {
        public TyrantTile(Position position, Level level)
            : base(position, level, 15, 5)
        {
        }

        public override char Display
        {
            get { return IsDead ? 'x' : '§'; }
        }

        public override bool GetMove(out Tile tile)
        {
            tile = null;

            if (IsDead)
            {
                return false;
            }

            int dx = Math.Sign(level.Hero.X - X);
            int dy = Math.Sign(level.Hero.Y - Y);

            if (dx != 0)
            {
                Tile horizontal = level.GetTile(X + dx, Y);

                if (horizontal is EmptyTile)
                {
                    tile = horizontal;
                    return true;
                }
            }

            if (dy != 0)
            {
                Tile vertical = level.GetTile(X, Y + dy);

                if (vertical is EmptyTile)
                {
                    tile = vertical;
                    return true;
                }
            }

            return false;
        }

        public override CharacterTile[] GetTargets()
        {
            List<CharacterTile> targets = new List<CharacterTile>();

            if (IsDead)
            {
                return targets.ToArray();
            }

            for (int x = 0; x < level.Width; x++)
            {
                AddTarget(level.GetTile(x, Y), targets);
            }

            for (int y = 0; y < level.Height; y++)
            {
                AddTarget(level.GetTile(X, y), targets);
            }

            return targets.ToArray();
        }

        private void AddTarget(
            Tile tile,
            List<CharacterTile> targets)
        {
            CharacterTile target = tile as CharacterTile;

            if (target != null &&
                target != this &&
                !target.IsDead &&
                !targets.Contains(target))
            {
                targets.Add(target);
            }
        }
    }
}
