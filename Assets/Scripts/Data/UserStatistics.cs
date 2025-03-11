using System;
using System.Collections.Generic;
using NCGames.Services;

namespace NCGames.Data
{
    /// <summary>
    /// Container for all user statistics and game history.
    /// </summary>
    [Serializable]
    public class UserStatistics
    {
        public int totalGamesPlayed;
        public int totalBetAmount;
        public int totalWinAmount;
        public List<GameResult> gameHistory = new List<GameResult>();
    }
}