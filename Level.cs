using System;
using System.Collections.Generic;

namespace GADE_POE_HEENA_JANA
{
    public class Level
    {
        public Tile[,] Grid { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public HeroTile Hero { get; private set; }

        public List<EnemyTile> Enemies { get; private set; }
        public List<GruntTile> Grunts { get; private set; }
        public List<HealthPickupTile> HealthPickups { get; private set; }

        private Random random = new Random();

        public Level(int width, int height)
        {
            Width = width;
            Height = height;

            Grid = new Tile[width, height];

            Enemies = new List<EnemyTile>();
            Grunts = new List<GruntTile>();
            HealthPickups = new List<HealthPickupTile>();

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            // Create the walls and empty tiles.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x == 0 || x == Width - 1 ||
                        y == 0 || y == Height - 1)
                    {
                        Grid[x, y] =
                            new WallTile(new Position(x, y));
                    }
                    else
                    {
                        Grid[x, y] =
                            new EmptyTile(new Position(x, y));
                    }
                }
            }

            // Spawn the hero.
            Position heroPos = GetRandomEmptyPosition();

            Hero = new HeroTile(heroPos);
            Grid[heroPos.X, heroPos.Y] = Hero;

            // Spawn 2-3 randomly selected enemy types.
            int enemyCount = random.Next(2, 4);

            for (int i = 0; i < enemyCount; i++)
            {
                Position enemyPos = GetRandomEmptyPosition();

                EnemyTile enemy = (EnemyTile)CreateTile(
                    Tile.TileType.Enemy, enemyPos);

                Enemies.Add(enemy);

                GruntTile grunt = enemy as GruntTile;

                if (grunt != null)
                {
                    Grunts.Add(grunt);
                }
            }

            // Spawn 1-2 health pickups.
            int pickupCount = random.Next(1, 3);

            for (int i = 0; i < pickupCount; i++)
            {
                Position pickupPos = GetRandomEmptyPosition();

                HealthPickupTile pickup =
                    (HealthPickupTile)CreateTile(
                        Tile.TileType.HealthPickup, pickupPos);

                HealthPickups.Add(pickup);
            }
        }

        private EnemyTile CreateEnemyTile(Position position)
        {
            int roll = random.Next(100);

            if (roll < 50)
            {
                return new GruntTile(position, this);
            }
            else if (roll < 80)
            {
                return new WarlockTile(position, this);
            }
            else
            {
                return new TyrantTile(position, this);
            }
        }

        private Tile CreateTile(
            Tile.TileType type, Position position)
        {
            Tile createdTile;

            switch (type)
            {
                case Tile.TileType.Hero:
                    createdTile = new HeroTile(position);
                    break;

                case Tile.TileType.Wall:
                    createdTile = new WallTile(position);
                    break;

                case Tile.TileType.Empty:
                    createdTile = new EmptyTile(position);
                    break;

                case Tile.TileType.HealthPickup:
                    createdTile = new HealthPickupTile(position);
                    break;

                case Tile.TileType.Grunt:
                    createdTile = new GruntTile(position, this);
                    break;

                case Tile.TileType.Warlock:
                    createdTile = new WarlockTile(position, this);
                    break;

                case Tile.TileType.Tyrant:
                    createdTile = new TyrantTile(position, this);
                    break;

                case Tile.TileType.Enemy:
                    createdTile = CreateEnemyTile(position);
                    break;

                default:
                    createdTile = new EmptyTile(position);
                    break;
            }

            Grid[position.X, position.Y] = createdTile;

            return createdTile;
        }

        public Position GetRandomEmptyPosition()
        {
            int x;
            int y;

            do
            {
                x = random.Next(1, Width - 1);
                y = random.Next(1, Height - 1);
            }
            while (!(Grid[x, y] is EmptyTile));

            return new Position(x, y);
        }

        public bool IsTileEmpty(Position pos)
        {
            return Grid[pos.X, pos.Y] is EmptyTile;
        }

        public void MoveTile(Tile tile, Position newPos)
        {
            Grid[tile.Position.X, tile.Position.Y] =
                new EmptyTile(tile.Position);

            tile.Position = newPos;

            Grid[newPos.X, newPos.Y] = tile;
        }
    }
}
