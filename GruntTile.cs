using System;
using System.Collections.Generic;

namespace GADE_POE_HEENA_JANA
{
    public class GruntTile : EnemyTile
    {
        private static readonly Random random = new Random();

        public GruntTile(Position position, Level level)
            : base(position, level, 10, 1)
        {
        }

        public override Tile.TileType GetTileType()
        {
            return Tile.TileType.Grunt;
        }

        public override bool GetMove(out Tile tile)
        {
            int dx = Math.Sign(
                Level.Hero.Position.X - Position.X);

            int dy = Math.Sign(
                Level.Hero.Position.Y - Position.Y);

            List<Tile> possibleMoves = new List<Tile>();

            if (dx != 0)
            {
                Position horizontal = new Position(
                    Position.X + dx, Position.Y);

                if (Level.IsTileEmpty(horizontal))
                {
                    possibleMoves.Add(
                        Level.Grid[horizontal.X, horizontal.Y]);
                }
            }

            if (dy != 0)
            {
                Position vertical = new Position(
                    Position.X, Position.Y + dy);

                if (Level.IsTileEmpty(vertical))
                {
                    possibleMoves.Add(
                        Level.Grid[vertical.X, vertical.Y]);
                }
            }

            if (possibleMoves.Count == 0)
            {
                tile = null;
                return false;
            }

            tile = possibleMoves[random.Next(possibleMoves.Count)];
            return true;
        }

        public override CharacterTile[] GetTargets()
        {
            int dx = Math.Abs(
                Level.Hero.Position.X - Position.X);

            int dy = Math.Abs(
                Level.Hero.Position.Y - Position.Y);

            if (dx + dy == 1 && !Level.Hero.IsDead)
            {
                return new CharacterTile[] { Level.Hero };
            }

            return new CharacterTile[0];
        }

        public override string ToString()
        {
            return IsDead ? "x" : "G";
        }
    }
}
