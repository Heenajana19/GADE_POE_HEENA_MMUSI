using System.Collections.Generic;

namespace GADE_POE_HEENA_JANA
{
    public class WarlockTile : EnemyTile
    {
        public WarlockTile(Position position, Level level)
            : base(position, level, 10, 5)
        {
        }

        public override Tile.TileType GetTileType()
        {
            return Tile.TileType.Warlock;
        }

        public override bool GetMove(out Tile tile)
        {
            tile = null;
            return false;
        }

        public override CharacterTile[] GetTargets()
        {
            List<CharacterTile> targets =
                new List<CharacterTile>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int x = Position.X + dx;
                    int y = Position.Y + dy;

                    if (x < 0 || x >= Level.Width ||
                        y < 0 || y >= Level.Height)
                    {
                        continue;
                    }

                    CharacterTile character =
                        Level.Grid[x, y] as CharacterTile;

                    if (character != null &&
                        !character.IsDead)
                    {
                        targets.Add(character);
                    }
                }
            }

            return targets.ToArray();
        }

        public override string ToString()
        {
            return IsDead ? "x" : "W";
        }
    }
}
