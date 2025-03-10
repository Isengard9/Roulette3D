using System;
using System.Collections.Generic;
using NCGames.Controllers.UI;
using NCGames.Events.Bet;
using NCGames.Events.Game;
using NCGames.Events.Popup;
using NCGames.GameSystems;
using UnityEngine;

namespace NCGames.Services
{
    /// <summary>
    /// Manages the game flow, controlling the roulette game mechanics and outcome evaluation.
    /// </summary>
    public class GameService : MonoBehaviour
    {
        [SerializeField] private CustomButtonController playButton;
        [SerializeField] private List<Pocket> pockets = new List<Pocket>();

        // Game state tracking
        private bool _isRouletteSpinning = false;
        private bool _isBallRolling = false;

        public CustomButtonController PlayButton => playButton;
        public IReadOnlyList<Pocket> Pockets => pockets;

        #region Event Management

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            var eventService = ServiceContainer.Instance.EventPublisherService;
            eventService.Subscribe<OnBetUpdatedEvent>(OnBetUpdated);
            eventService.Subscribe<OnRouletteStoppedEvent>(OnRouletteStopped);
            eventService.Subscribe<OnBallStoppedEvent>(OnBallStopped);
            playButton.OnButtonClicked += OnPlayButtonClicked;
        }

        private void UnsubscribeFromEvents()
        {
            if (ServiceContainer.Instance?.EventPublisherService != null)
            {
                var eventService = ServiceContainer.Instance.EventPublisherService;
                eventService.Unsubscribe<OnBetUpdatedEvent>(OnBetUpdated);
                eventService.Unsubscribe<OnRouletteStoppedEvent>(OnRouletteStopped);
                eventService.Unsubscribe<OnBallStoppedEvent>(OnBallStopped);
            }
            
            if (playButton != null)
            {
                playButton.OnButtonClicked -= OnPlayButtonClicked;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when bet data is updated. Enables/disables play button based on bet status.
        /// </summary>
        private void OnBetUpdated(OnBetUpdatedEvent e)
        {
            playButton.interactable = (ServiceContainer.Instance.BetService.currentBets.Count > 0);
        }

        /// <summary>
        /// Handles play button click to start the game round.
        /// </summary>
        private void OnPlayButtonClicked()
        {
            // Set game state
            _isRouletteSpinning = true;
            _isBallRolling = true;
            
            // Close bet UI and trigger game start
            ServiceContainer.Instance.EventPublisherService.Publish(new OnBetPopupCloseEvent());
            ServiceContainer.Instance.EventPublisherService.Publish(new OnGameStartEvent());
            
            // Disable play button during gameplay
            playButton.interactable = false;
        }

        /// <summary>
        /// Called when the roulette wheel completes spinning.
        /// </summary>
        private void OnRouletteStopped(OnRouletteStoppedEvent e)
        {
            _isRouletteSpinning = false;
            CheckGameCompletion();
        }

        /// <summary>
        /// Called when the ball stops rolling.
        /// </summary>
        private void OnBallStopped(OnBallStoppedEvent e)
        {
            _isBallRolling = false;
            CheckGameCompletion();
        }

        #endregion

        #region Game Flow Management

        /// <summary>
        /// Checks if both the roulette and ball have stopped, then evaluates the game outcome.
        /// </summary>
        private void CheckGameCompletion()
        {
            if (_isRouletteSpinning || _isBallRolling)
                return;

            EvaluateGameOutcome();
        }

        /// <summary>
        /// Determines the winning pocket and evaluates bets.
        /// </summary>
        private void EvaluateGameOutcome()
        {
            foreach (var pocket in pockets)
            {
                if (pocket.IsPocketActive)
                {
                    int winningNumber = pocket.Data.pocketNumber;
                    
                    // Publish game ended event with the winning number
                    ServiceContainer.Instance.EventPublisherService.Publish(new OnGameEndedEvent
                    {
                        WinningNumber = winningNumber
                    });

                    // Evaluate bets based on winning number
                    ServiceContainer.Instance.BetService.EvaluateBets(winningNumber);
                    break;
                }
            }
        }

        #endregion
    }
}