using System;
using System.Linq;

namespace GADE_POE_HEENA_JANA
{
    public class GameEngine
    {
        public Level CurrentLevel { get; private set; }

        public GameEngine(int width, int height)
        {
            CurrentLevel = new Level(width, height);
        }

        public void MoveHero(Direction direction)
        {
            if (CurrentLevel.Hero.IsDead)
            {
                return;
            }

            Position targetPos = GetTargetPosition(
                CurrentLevel.Hero.Position, direction);

            Tile targetTile =
                CurrentLevel.Grid[targetPos.X, targetPos.Y];

            if (targetTile is EmptyTile)
            {
                CurrentLevel.MoveTile(
                    CurrentLevel.Hero, targetPos);
            }
            else if (targetTile is HealthPickupTile pickup)
            {
                CurrentLevel.Hero.Heal(pickup.HealAmount);
                CurrentLevel.HealthPickups.Remove(pickup);

                CurrentLevel.MoveTile(
                    CurrentLevel.Hero, targetPos);
            }
            else if (targetTile is EnemyTile enemy)
            {
                CurrentLevel.Hero.Attack(enemy);

                if (enemy.IsDead)
                {
                    RemoveEnemy(enemy);
                }
            }

            ProcessEnemyTurns();
        }

        private void ProcessEnemyTurns()
        {
            foreach (EnemyTile enemy in
                CurrentLevel.Enemies.ToArray())
            {
                if (CurrentLevel.Hero.IsDead)
                {
                    break;
                }

                if (enemy.IsDead)
                {
                    RemoveEnemy(enemy);
                    continue;
                }

                // Attack all characters selected by this enemy.
                CharacterTile[] targets = enemy.GetTargets();

                if (targets.Length > 0)
                {
                    foreach (CharacterTile target in targets)
                    {
                        if (target != null && !target.IsDead)
                        {
                            enemy.Attack(target);
                        }
                    }
                }
                else
                {
                    // Move if this enemy type has a valid move.
                    Tile nextTile;

                    if (enemy.GetMove(out nextTile) &&
                        nextTile != null &&
                        nextTile is EmptyTile)
                    {
                        CurrentLevel.MoveTile(
                            enemy, nextTile.Position);
                    }
                }

                // Remove enemies defeated during the turn.
                foreach (EnemyTile other in
                    CurrentLevel.Enemies.ToArray())
                {
                    if (other.IsDead)
                    {
                        RemoveEnemy(other);
                    }
                }
            }
        }

        private void RemoveEnemy(EnemyTile enemy)
        {
            CurrentLevel.Enemies.Remove(enemy);

            GruntTile grunt = enemy as GruntTile;

            if (grunt != null)
            {
                CurrentLevel.Grunts.Remove(grunt);
            }

            CurrentLevel.Grid[
                enemy.Position.X,
                enemy.Position.Y] =
                new EmptyTile(enemy.Position);
        }

        private Position GetTargetPosition(
            Position current, Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return new Position(current.X, current.Y - 1);

                case Direction.Down:
                    return new Position(current.X, current.Y + 1);

                case Direction.Left:
                    return new Position(current.X - 1, current.Y);

                case Direction.Right:
                    return new Position(current.X + 1, current.Y);

                default:
                    return current;
            }
        }
    }
}
