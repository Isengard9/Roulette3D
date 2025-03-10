using System;
using System.Collections.Generic;
using System.Linq;
using NCGames.Data;
using NCGames.Enums;
using NCGames.Events.Bet;
using NCGames.Managers;
using UnityEngine;

namespace NCGames.Services
{
    /// <summary>
    /// Manages all bet-related functionality including placing, updating, 
    /// removing, and evaluating bets in the roulette game.
    /// </summary>
    [Serializable]
    public class BetService
    {
        /// <summary>
        /// Represents a single bet placed on the roulette table.
        /// </summary>
        [Serializable]
        public struct Bet
        {
            [SerializeField] public BetType betType;  // Type of the bet (e.g., Straight, Split, etc.)
            [SerializeField] public int[] numbers;    // Numbers covered by this bet
            [SerializeField] public int amount;       // Bet amount in chips
        }

        [SerializeField] public List<Bet> currentBets;

        /// <summary>
        /// Initializes a new instance of the BetService.
        /// </summary>
        public BetService()
        {
            currentBets = new List<Bet>();
        }

        #region Bet Management

        /// <summary>
        /// Places a new bet on the table.
        /// </summary>
        /// <param name="betType">Type of bet to place</param>
        /// <param name="numbers">Numbers covered by the bet</param>
        /// <param name="amount">Amount to bet</param>
        public void PlaceBet(BetType betType, int[] numbers, int amount)
        {
            Bet newBet = new Bet
            {
                betType = betType,
                numbers = numbers,
                amount = amount
            };

            currentBets.Add(newBet);
            NotifyBetUpdated();
        }

        /// <summary>
        /// Retrieves a bet matching the provided bet data.
        /// </summary>
        /// <param name="currentBetData">Data to search for</param>
        /// <returns>Matching bet if found, null otherwise</returns>
        public Bet? GetBet(BetData currentBetData)
        {
            foreach (var bet in currentBets)
            {
                if (bet.betType == currentBetData.betType && 
                    bet.numbers.SequenceEqual(currentBetData.numbersOfBet))
                {
                    return bet;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates an existing bet with a new amount.
        /// </summary>
        /// <param name="updateBet">Bet to update</param>
        /// <param name="newAmount">New bet amount</param>
        public void UpdateBet(Bet updateBet, int newAmount)
        {
            for (int i = 0; i < currentBets.Count; i++)
            {
                if (currentBets[i].betType == updateBet.betType &&
                    currentBets[i].numbers.SequenceEqual(updateBet.numbers))
                {
                    currentBets[i] = new Bet
                    {
                        betType = updateBet.betType,
                        numbers = updateBet.numbers,
                        amount = newAmount
                    };
                    break;
                }
            }
            
            NotifyBetUpdated();
        }

        /// <summary>
        /// Removes a bet from the current bets.
        /// </summary>
        /// <param name="currentBet">Bet to remove</param>
        public void RemoveBet(Bet? currentBet)
        {
            if (currentBet.HasValue)
            {
                currentBets.RemoveAll(bet => 
                    bet.betType == currentBet.Value.betType &&
                    bet.numbers.SequenceEqual(currentBet.Value.numbers));
            }
            
            NotifyBetUpdated();
        }

        /// <summary>
        /// Notifies the system that bet data has been updated.
        /// </summary>
        private void NotifyBetUpdated()
        {
            ServiceContainer.Instance.EventPublisherService.Publish(new OnBetUpdatedEvent());
        }

        #endregion

        #region Bet Evaluation

        /// <summary>
        /// Evaluates all current bets against the winning number and calculates payouts.
        /// </summary>
        /// <param name="winningNumber">The winning number from the roulette spin</param>
        /// <returns>Total payout amount</returns>
        public float EvaluateBets(int winningNumber)
        {
            float totalPayout = 0;
            
            foreach (Bet bet in currentBets)
            {
                if (Array.Exists(bet.numbers, number => number == winningNumber))
                {
                    float payoutMultiplier = GetPayoutMultiplier(bet.betType);
                    float betPayout = bet.amount * payoutMultiplier;
                    totalPayout += betPayout;
                    
                    LogManager.Log($"Won bet: {bet.betType} with {bet.amount} chips, payout: {betPayout}",
                        LogManager.LogLevel.Development);
                }
            }
            
            LogManager.Log($"Total payout: {totalPayout}", LogManager.LogLevel.Development);
            return totalPayout;
        }

        /// <summary>
        /// Returns the payout multiplier for a specific bet type.
        /// </summary>
        /// <param name="betType">Type of bet</param>
        /// <returns>The multiplier value for calculating winnings</returns>
        private float GetPayoutMultiplier(BetType betType)
        {
            switch (betType)
            {
                // Inside bets (higher risk/reward)
                case BetType.Straight: return 35f;  // Single number
                case BetType.Split: return 17f;     // Two adjacent numbers
                case BetType.Street: return 11f;    // Three numbers in a row
                case BetType.Corner: return 8f;     // Four numbers in a square
                case BetType.SixLine: return 5f;    // Six numbers (two rows)
                
                // Outside bets (lower risk/reward)
                case BetType.Red:
                case BetType.Black:
                case BetType.Even:
                case BetType.Odd:
                case BetType.High:
                case BetType.Low: return 1f;        // Even money bets
                
                case BetType.Dozens:                // 1-12, 13-24, 25-36
                case BetType.Columns: return 2f;    // Column bets
                
                default:
                    LogManager.Log($"Unknown bet type: {betType}", LogManager.LogLevel.Error);
                    return 0f;
            }
        }
        
        #endregion
    }
}