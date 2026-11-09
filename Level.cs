using System;
using System.Collections.Generic;
using System.Linq;

namespace GADE_POE_HEENA_JANA
{
    public class Level
    {
        public Tile[,] Grid { get; private set; }

        public Tile[,] Tiles
        {
            get { return Grid; }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public HeroTile Hero { get; private set; }
        public ExitTile Exit { get; private set; }

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

            Position heroPos = GetRandomEmptyPosition();

            Hero = new HeroTile(heroPos);
            Grid[heroPos.X, heroPos.Y] = Hero;

            // Create a locked exit.
            Position exitPos = GetRandomEmptyPosition();

            Exit = new ExitTile(exitPos);
            Grid[exitPos.X, exitPos.Y] = Exit;

            // Spawn 2-3 enemies.
            int enemyCount = random.Next(2, 4);

            for (int i = 0; i < enemyCount; i++)
            {
                Position enemyPos = GetRandomEmptyPosition();

                EnemyTile enemy = CreateEnemyTile(enemyPos);

                Grid[enemyPos.X, enemyPos.Y] = enemy;
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
                    new HealthPickupTile(pickupPos);

                Grid[pickupPos.X, pickupPos.Y] = pickup;
                HealthPickups.Add(pickup);
            }

            UpdateExit();
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

        public void UpdateExit()
        {
            bool allEnemiesDefeated =
                Enemies.All(enemy => enemy.IsDead);

            if (allEnemiesDefeated && Exit != null)
            {
                Exit.Unlock();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class Level
    {
        [NonSerialized]
        private Tile[,] tiles;

        private int width;
        private int height;

        private HeroTile hero;
        private ExitTile exit;
        private EnemyTile[] enemies;
        private PickupTile[] pickups;

        private static readonly Random random = new Random();

        public Tile[,] Tiles
        {
            get { return tiles; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public HeroTile Hero
        {
            get { return hero; }
        }

        public ExitTile Exit
        {
            get { return exit; }
        }

        public EnemyTile[] Enemies
        {
            get { return enemies; }
        }

        public PickupTile[] Pickups
        {
            get { return pickups; }
        }

        public Level(
            int width,
            int height,
            int numberOfEnemies,
            int numberOfPickups,
            HeroTile existingHero = null)
        {
            if (width < 3 || height < 3)
            {
                throw new ArgumentException("The level is too small.");
            }

            int interiorSpaces = (width - 2) * (height - 2);

            if (numberOfEnemies < 0 ||
                numberOfPickups < 0 ||
                numberOfEnemies + numberOfPickups + 2 > interiorSpaces)
            {
                throw new ArgumentException(
                    "There is not enough space for the level contents.");
            }

            this.width = width;
            this.height = height;

            tiles = new Tile[width, height];
            enemies = new EnemyTile[numberOfEnemies];
            pickups = new PickupTile[numberOfPickups];

            InitialiseTiles();

            Position heroPosition = GetRandomEmptyPosition();

            if (existingHero == null)
            {
                hero = (HeroTile)CreateTile(
                    Tile.TileType.Hero, heroPosition);
            }
            else
            {
                hero = existingHero;
                hero.Position = heroPosition;
                tiles[hero.X, hero.Y] = hero;
            }

            exit = (ExitTile)CreateTile(
                Tile.TileType.Exit, GetRandomEmptyPosition());

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = (EnemyTile)CreateTile(
                    Tile.TileType.Enemy, GetRandomEmptyPosition());
            }

            for (int i = 0; i < pickups.Length; i++)
            {
                pickups[i] = (PickupTile)CreateTile(
                    Tile.TileType.Pickup, GetRandomEmptyPosition());
            }

            UpdateVision();
            UpdateExit();
        }

        private Tile CreateTile(
            Tile.TileType tileType,
            Position position)
        {
            Tile tile;

            switch (tileType)
            {
                case Tile.TileType.Wall:
                    tile = new WallTile(position);
                    break;

                case Tile.TileType.Hero:
                    tile = new HeroTile(position);
                    break;

                case Tile.TileType.Exit:
                    tile = new ExitTile(position);
                    break;

                case Tile.TileType.Enemy:
                    tile = CreateEnemyTile(position);
                    break;

                case Tile.TileType.Pickup:
                    tile = CreatePickupTile(position);
                    break;

                default:
                    tile = new EmptyTile(position);
                    break;
            }

            tiles[position.X, position.Y] = tile;
            return tile;
        }

        private EnemyTile CreateEnemyTile(Position position)
        {
            int roll = random.Next(100);

            if (roll < 50)
            {
                return new GruntTile(position, this);
            }

            if (roll < 80)
            {
                return new WarlockTile(position, this);
            }

            return new TyrantTile(position, this);
        }

        private PickupTile CreatePickupTile(Position position)
        {
            if (random.Next(3) < 2)
            {
                return new HealthPickupTile(position);
            }

            return new AttackBuffPickupTile(position);
        }

        private void InitialiseTiles()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool boundary =
                        x == 0 || y == 0 ||
                        x == width - 1 || y == height - 1;

                    CreateTile(
                        boundary
                            ? Tile.TileType.Wall
                            : Tile.TileType.Empty,
                        new Position(x, y));
                }
            }
        }

        public Position GetRandomEmptyPosition()
        {
            List<Position> emptyPositions = new List<Position>();

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (tiles[x, y] is EmptyTile)
                    {
                        emptyPositions.Add(new Position(x, y));
                    }
                }
            }

            if (emptyPositions.Count == 0)
            {
                throw new InvalidOperationException(
                    "There are no empty positions.");
            }

            return emptyPositions[random.Next(emptyPositions.Count)];
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
            {
                return null;
            }

            return tiles[x, y];
        }

        public void SwapTiles(Tile first, Tile second)
        {
            Position firstPosition = first.Position;
            Position secondPosition = second.Position;

            first.Position = secondPosition;
            second.Position = firstPosition;

            tiles[first.X, first.Y] = first;
            tiles[second.X, second.Y] = second;
        }

        public void CollectPickup(PickupTile pickup)
        {
            pickup.ApplyEffect(hero);

            for (int i = 0; i < pickups.Length; i++)
            {
                if (pickups[i] == pickup)
                {
                    pickups[i] = null;
                    break;
                }
            }

            Position position = pickup.Position;

            tiles[position.X, position.Y] =
                new EmptyTile(position);
        }

        public void UpdateVision()
        {
            hero.UpdateVision(this);

            foreach (EnemyTile enemy in enemies)
            {
                enemy.UpdateVision(this);
            }
        }

        public void UpdateExit()
        {
            if (enemies.All(enemy => enemy.IsDead))
            {
                exit.Unlock();
            }
        }

        // Called explicitly after the full saved object graph is loaded.
        public void RebuildGrid()
        {
            tiles = new Tile[width, height];
            InitialiseTiles();

            tiles[hero.X, hero.Y] = hero;
            tiles[exit.X, exit.Y] = exit;

            foreach (EnemyTile enemy in enemies)
            {
                tiles[enemy.X, enemy.Y] = enemy;
            }

            foreach (PickupTile pickup in pickups)
            {
                if (pickup != null)
                {
                    tiles[pickup.X, pickup.Y] = pickup;
                }
            }

            UpdateVision();
            UpdateExit();
        }

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    display.Append(tiles[x, y].Display);
                    display.Append(' ');
                }

                display.AppendLine();
            }

            return display.ToString();
        }
    }
}
