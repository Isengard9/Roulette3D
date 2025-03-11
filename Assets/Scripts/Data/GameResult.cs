using System;
using System.Collections.Generic;
using NCGames.Services;

namespace NCGames.Data
{
    /// <summary>
    /// Represents the result of a single game round.
    /// </summary>
    [Serializable]
    public class GameResult
    {
        public DateTime timestamp;
        public int winningNumber;
        public int balanceBefore;
        public int balanceAfter;
        public int totalBetAmount;
        public int totalWinAmount;
        public List<BetRecord> bets = new List<BetRecord>();
    }
}