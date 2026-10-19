using System;
using System.Collections.Generic;

namespace GADE_POE_HEENA_JANA
{
    public class TyrantTile : EnemyTile
    {
        public TyrantTile(Position position, Level level)
            : base(position, level, 15, 5)
        {
        }

        public override Tile.TileType GetTileType()
        {
            return Tile.TileType.Tyrant;
        }

        public override bool GetMove(out Tile tile)
        {
            int dx = Math.Sign(
                Level.Hero.Position.X - Position.X);

            int dy = Math.Sign(
                Level.Hero.Position.Y - Position.Y);

            // Try moving horizontally towards the hero.
            if (dx != 0)
            {
                int x = Position.X + dx;
                int y = Position.Y;

                if (Level.IsTileEmpty(new Position(x, y)))
                {
                    tile = Level.Grid[x, y];
                    return true;
                }
            }

            // Try moving vertically towards the hero.
            if (dy != 0)
            {
                int x = Position.X;
                int y = Position.Y + dy;

                if (Level.IsTileEmpty(new Position(x, y)))
                {
                    tile = Level.Grid[x, y];
                    return true;
                }
            }

            tile = null;
            return false;
        }

        public override CharacterTile[] GetTargets()
        {
            List<CharacterTile> targets =
                new List<CharacterTile>();

            for (int x = 0; x < Level.Width; x++)
            {
                if (x == Position.X)
                {
                    continue;
                }

                CharacterTile character =
                    Level.Grid[x, Position.Y] as CharacterTile;

                if (character != null &&
                    !character.IsDead)
                {
                    targets.Add(character);
                }
            }

            for (int y = 0; y < Level.Height; y++)
            {
                if (y == Position.Y)
                {
                    continue;
                }

                CharacterTile character =
                    Level.Grid[Position.X, y] as CharacterTile;

                if (character != null &&
                    !character.IsDead)
                {
                    targets.Add(character);
                }
            }

            return targets.ToArray();
        }

        public override string ToString()
        {
            return IsDead ? "x" : "T";
        }
    }
}
