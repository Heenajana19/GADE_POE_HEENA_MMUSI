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

            // Prevent moving outside the grid.
            if (targetPos.X < 0 ||
                targetPos.X >= CurrentLevel.Width ||
                targetPos.Y < 0 ||
                targetPos.Y >= CurrentLevel.Height)
            {
                return;
            }

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

                // Unlock the exit if all enemies are defeated.
                CurrentLevel.UpdateExit();
            }
            else if (targetTile is ExitTile exit)
            {
                if (!exit.IsLocked)
                {
                    // The exit is unlocked.
                    // Next-level progression must be handled here
                    // if your project implements multiple levels.
                    return;
                }

                // The exit is locked, so the hero cannot enter.
                return;
            }

            ProcessEnemyTurns();

            // Also check after enemy turns.
            CurrentLevel.UpdateExit();
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
                    Tile nextTile;

                    if (enemy.GetMove(out nextTile) &&
                        nextTile != null &&
                        nextTile is EmptyTile)
                    {
                        CurrentLevel.MoveTile(
                            enemy, nextTile.Position);
                    }
                }

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

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace GADE_POE_HEENA_JANA
{
    public class GameEngine
    {
        private const int MIN_SIZE = 10;
        private const int MAX_SIZE = 20;

        private Level currentLevel;
        private int numberOfLevels;
        private int currentLevelNumber;
        private int successfulMoves;
        private GameState gameState;

        private readonly Random random = new Random();

        public Level CurrentLevel
        {
            get { return currentLevel; }
        }

        public int CurrentLevelNumber
        {
            get { return currentLevelNumber; }
        }

        public int NumberOfLevels
        {
            get { return numberOfLevels; }
        }

        public GameState State
        {
            get { return gameState; }
        }

        public string HeroStats
        {
            get
            {
                return currentLevel.Hero.CurrentHealth + "/" +
                    currentLevel.Hero.MaxHealth;
            }
        }

        public string SaveFilePath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "GADE_POE_HEENA_JANA",
                    "savegame.bin");
            }
        }

        public GameEngine(int numberOfLevels)
        {
            if (numberOfLevels < 1 || numberOfLevels > 10)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfLevels),
                    "Choose between 1 and 10 levels.");
            }

            this.numberOfLevels = numberOfLevels;
            currentLevelNumber = 1;
            gameState = GameState.InProgress;

            currentLevel = CreateLevel(null);
        }

        private Level CreateLevel(HeroTile hero)
        {
            return new Level(
                random.Next(MIN_SIZE, MAX_SIZE + 1),
                random.Next(MIN_SIZE, MAX_SIZE + 1),
                currentLevelNumber,
                1,
                hero);
        }

        private void NextLevel()
        {
            HeroTile hero = currentLevel.Hero;

            currentLevelNumber++;
            successfulMoves = 0;
            currentLevel = CreateLevel(hero);
        }

        private bool MoveHero(Direction direction)
        {
            Tile target = currentLevel.Hero.GetVisibleTile(direction);

            if (target == null)
            {
                return false;
            }

            if (target is ExitTile exit)
            {
                if (exit.IsLocked)
                {
                    return false;
                }

                if (currentLevelNumber == numberOfLevels)
                {
                    gameState = GameState.Complete;
                    return false;
                }

                NextLevel();
                return true;
            }

            if (target is PickupTile pickup)
            {
                Position position = pickup.Position;

                currentLevel.CollectPickup(pickup);

                target = currentLevel.GetTile(
                    position.X, position.Y);
            }

            if (!(target is EmptyTile))
            {
                return false;
            }

            currentLevel.SwapTiles(currentLevel.Hero, target);
            currentLevel.UpdateVision();

            return true;
        }

        public void TriggerMovement(Direction direction)
        {
            if (gameState != GameState.InProgress)
            {
                return;
            }

            int previousLevel = currentLevelNumber;

            if (!MoveHero(direction))
            {
                return;
            }

            // Entering a fresh level starts a fresh movement counter.
            if (previousLevel != currentLevelNumber)
            {
                return;
            }

            successfulMoves++;

            if (successfulMoves >= 2)
            {
                successfulMoves = 0;
                MoveEnemies();
            }
        }

        private void MoveEnemies()
        {
            foreach (EnemyTile enemy in currentLevel.Enemies)
            {
                if (enemy.IsDead)
                {
                    continue;
                }

                if (enemy.GetMove(out Tile target) &&
                    target is EmptyTile)
                {
                    currentLevel.SwapTiles(enemy, target);
                    currentLevel.UpdateVision();
                }
            }
        }

        private bool HeroAttack(Direction direction)
        {
            CharacterTile target =
                currentLevel.Hero.GetVisibleTile(direction)
                as CharacterTile;

            if (target == null || target.IsDead)
            {
                return false;
            }

            currentLevel.Hero.Attack(target);
            return true;
        }

        private void EnemiesAttack()
        {
            foreach (EnemyTile enemy in currentLevel.Enemies)
            {
                if (currentLevel.Hero.IsDead)
                {
                    break;
                }

                if (enemy.IsDead)
                {
                    continue;
                }

                foreach (CharacterTile target in enemy.GetTargets())
                {
                    if (!target.IsDead)
                    {
                        enemy.Attack(target);
                    }
                }
            }
        }

        public void TriggerAttack(Direction direction)
        {
            if (gameState != GameState.InProgress)
            {
                return;
            }

            if (HeroAttack(direction))
            {
                EnemiesAttack();

                if (currentLevel.Hero.IsDead)
                {
                    gameState = GameState.GameOver;
                }
            }

            currentLevel.UpdateExit();
        }

        private DataContractSerializer CreateSerializer()
        {
            Type[] knownTypes =
            {
                typeof(Level),
                typeof(Position),
                typeof(EmptyTile),
                typeof(WallTile),
                typeof(HeroTile),
                typeof(GruntTile),
                typeof(WarlockTile),
                typeof(TyrantTile),
                typeof(HealthPickupTile),
                typeof(AttackBuffPickupTile),
                typeof(ExitTile)
            };

            return new DataContractSerializer(
                typeof(GameSaveData),
                new DataContractSerializerSettings
                {
                    KnownTypes = knownTypes,
                    PreserveObjectReferences = true,
                    MaxItemsInObjectGraph = 100000
                });
        }

        public void SaveGame()
        {
            GameSaveData data = new GameSaveData(
                numberOfLevels,
                currentLevelNumber,
                successfulMoves,
                gameState,
                currentLevel);

            string folder = Path.GetDirectoryName(SaveFilePath);
            Directory.CreateDirectory(folder);

            string temporaryPath = SaveFilePath + ".tmp";

            try
            {
                using (FileStream stream = File.Create(temporaryPath))
                using (XmlDictionaryWriter writer =
                    XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    CreateSerializer().WriteObject(writer, data);
                    writer.Flush();
                }

                File.Move(temporaryPath, SaveFilePath, true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        public void LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                throw new FileNotFoundException(
                    "No saved game was found. Save a game first.");
            }

            GameSaveData data;

            using (FileStream stream = File.OpenRead(SaveFilePath))
            using (XmlDictionaryReader reader =
                XmlDictionaryReader.CreateBinaryReader(
                    stream,
                    XmlDictionaryReaderQuotas.Max))
            {
                data = CreateSerializer().ReadObject(reader)
                    as GameSaveData;
            }

            if (data == null ||
                data.Version != 1 ||
                data.CurrentLevel == null ||
                data.NumberOfLevels < 1 ||
                data.NumberOfLevels > 10 ||
                data.CurrentLevelNumber < 1 ||
                data.CurrentLevelNumber > data.NumberOfLevels ||
                data.SuccessfulMoves < 0 ||
                data.SuccessfulMoves > 1 ||
                !Enum.IsDefined(typeof(GameState), data.State))
            {
                throw new InvalidDataException(
                    "The saved game is invalid or incompatible.");
            }

            // Rebuild before replacing the current game.
            data.CurrentLevel.RebuildGrid();

            numberOfLevels = data.NumberOfLevels;
            currentLevelNumber = data.CurrentLevelNumber;
            successfulMoves = data.SuccessfulMoves;
            gameState = data.State;
            currentLevel = data.CurrentLevel;
        }

        public override string ToString()
        {
            if (gameState == GameState.Complete)
            {
                return "YOU WIN! You completed all " +
                    numberOfLevels + " levels.";
            }

            if (gameState == GameState.GameOver)
            {
                return "GAME OVER!" +
                    Environment.NewLine +
                    Environment.NewLine +
                    currentLevel;
            }

            return currentLevel.ToString();
        }
    }
}
