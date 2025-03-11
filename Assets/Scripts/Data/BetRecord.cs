using System;
using System.Collections.Generic;

namespace NCGames.Data
{
    /// <summary>
    /// Records information about a single bet.
    /// </summary>
    [Serializable]
    public class BetRecord
    {
        public string betType;
        public int amount;
        public List<int> numbers;
        public bool isWin;
        public int winAmount;
    }
}