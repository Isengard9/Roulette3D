using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NCGames.Data;
using NCGames.Events.Game;
using NCGames.Managers;
using UnityEngine;

namespace NCGames.Services
{
    /// <summary>
    /// Manages saving and loading of user statistics and game history.
    /// </summary>
    public class SaveLoadService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string _saveFileName = "user_statistics.json";
        
        // In-memory store of the user's game history
        [SerializeField] private UserStatistics _userStatistics = new UserStatistics();

        #region Unity Lifecycle
        
        private void Awake()
        {
            LoadStatistics();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        
        #endregion

        #region Event Management

        private void SubscribeToEvents()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnGameEndedEvent>(OnGameEnded);
        }

        private void UnsubscribeFromEvents()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnGameEndedEvent>(OnGameEnded);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles game ended event by saving game results to statistics.
        /// </summary>
        private void OnGameEnded(OnGameEndedEvent e)
        {
            // Create a new game result record
            GameResult result = new GameResult
            {
                timestamp = DateTime.Now,
                winningNumber = e.WinningNumber,
                bets = new List<BetRecord>()
            };

            // Record all active bets for this game
            BetService betService = ServiceContainer.Instance.BetService;
            int previousBalance = betService.CurrentBalance;
            
            
            foreach (var bet in betService.currentBets)
            {
                // Determine if this bet won
                bool isWinningBet = (Array.Exists(bet.numbers, number => number == e.WinningNumber));
                int payoutMultiplier = betService.GetPayoutMultiplier(bet.betType);
                int betPayout = bet.amount * payoutMultiplier;
                // Create and add bet record
                BetRecord betRecord = new BetRecord
                {
                    betType = bet.betType.ToString(),
                    amount = bet.amount,
                    numbers = new List<int>(bet.numbers),
                    isWin = isWinningBet,
                    winAmount = betPayout
                };
                
                result.bets.Add(betRecord);
            }
            
            // Calculate total results
            result.totalBetAmount = CalculateTotalBetAmount(result.bets);
            result.totalWinAmount = CalculateTotalWinAmount(result.bets);
            result.balanceAfter = previousBalance + betService.CurrentBalance;
            result.balanceBefore = previousBalance;
            
            // Add this result to the history
            _userStatistics.gameHistory.Add(result);
            
            // Update lifetime statistics
            _userStatistics.totalGamesPlayed++;
            _userStatistics.totalBetAmount += result.totalBetAmount;
            _userStatistics.totalWinAmount += result.totalWinAmount;
            betService.UpdateBalance(result.balanceAfter);
            // Save updated statistics
            SaveStatistics();
            
            LogManager.Log($"Game result saved. Winning number: {e.WinningNumber}, Balance: {result.balanceAfter}");
        }
        
        #endregion

        #region Save/Load Methods

        /// <summary>
        /// Saves current statistics to a JSON file.
        /// </summary>
        public void SaveStatistics()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, _saveFileName);
                string jsonData = JsonUtility.ToJson(_userStatistics, true);
                File.WriteAllText(path, jsonData);
                Debug.Log($"Statistics saved to: {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save statistics: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads statistics from JSON file if it exists.
        /// </summary>
        public void LoadStatistics()
        {
            string path = Path.Combine(Application.persistentDataPath, _saveFileName);
            
            if (File.Exists(path))
            {
                try
                {
                    string jsonData = File.ReadAllText(path);
                    _userStatistics = JsonUtility.FromJson<UserStatistics>(jsonData);
                    Debug.Log($"Statistics loaded from: {path}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load statistics: {ex.Message}");
                    // Create new statistics if loading fails
                    _userStatistics = new UserStatistics();
                }
            }
            else
            {
                Debug.Log("No saved statistics found. Creating new statistics.");
                _userStatistics = new UserStatistics();
            }
        }
        
        #endregion
        
        #region Helper Methods

        private int CalculateTotalBetAmount(List<BetRecord> bets)
        {
            int total = 0;
            foreach (var bet in bets)
            {
                total += bet.amount;
            }
            return total;
        }

        private int CalculateTotalWinAmount(List<BetRecord> bets)
        {
            int total = 0;
            foreach (var bet in bets)
            {
                total += bet.winAmount;
            }
            return total;
        }
        
        /// <summary>
        /// Returns a copy of the current user statistics.
        /// </summary>
        public UserStatistics GetStatistics()
        {
            return _userStatistics;
        }
        
        #endregion
    }
}