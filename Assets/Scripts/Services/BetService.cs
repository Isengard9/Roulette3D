using System.Collections.Generic;
using NCGames.Enums;

namespace NCGames.Services
{
    public class BetService
    {
        public struct Bet
        {
            public BetType betType;
            public int[] numbers;
            public float amount;
        }

        public List<Bet> currentBets;

        public void PlaceBet(BetType betType, int[] numbers, float amount)
        {
            Bet newBet = new Bet
            {
                betType = betType,
                numbers = numbers,
                amount = amount
            };

            currentBets.Add(newBet);
        }

        public float EvaluateBets(int winningNumber)
        {
            float totalPayout = 0;
            foreach (Bet bet in currentBets)
            {
                if (System.Array.Exists(bet.numbers, number => number == winningNumber))
                {
                    totalPayout += bet.amount * GetPayoutMultiplier(bet.betType);
                }
            }

            return totalPayout;
        }

        private float GetPayoutMultiplier(BetType betType)
        {
            switch (betType)
            {
                case BetType.Straight: return 35f;
                case BetType.Split: return 17f;
                case BetType.Street: return 11f;
                case BetType.Corner: return 8f;
                case BetType.SixLine: return 5f;
                case BetType.Red:
                case BetType.Black:
                case BetType.Even:
                case BetType.Odd:
                case BetType.High:
                case BetType.Low: return 1f;
                case BetType.Dozens:
                case BetType.Columns: return 2f;
                default: return 0f;
            }
        }
    }
}