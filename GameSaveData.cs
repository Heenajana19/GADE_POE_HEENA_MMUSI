using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class GameSaveData
    {
        public int Version;
        public int NumberOfLevels;
        public int CurrentLevelNumber;
        public int SuccessfulMoves;
        public GameState State;
        public Level CurrentLevel;

        public GameSaveData(
            int numberOfLevels,
            int currentLevelNumber,
            int successfulMoves,
            GameState state,
            Level currentLevel)
        {
            Version = 1;
            NumberOfLevels = numberOfLevels;
            CurrentLevelNumber = currentLevelNumber;
            SuccessfulMoves = successfulMoves;
            State = state;
            CurrentLevel = currentLevel;
        }
    }
}
