using System.Collections.Generic;
using NCGames.Controllers.UI.Token;
using NCGames.Data;
using NCGames.Enums;
using NCGames.Events.Popup;
using NCGames.Managers;
using NCGames.Services;
using TMPro;
using UnityEngine;

namespace NCGames.Controllers.UI.Popup.TokenPopup
{
    /// <summary>
    /// Controls the token popup UI for placing and managing bets with different chip values.
    /// </summary>
    public class TokenPopupController : MonoBehaviour
    {
        // Animation parameter hashes for better performance
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int Open = Animator.StringToHash("Open");
        
        [Header("UI References")]
        [SerializeField] private Animator _tokenPopupAnimator;
        [SerializeField] private TMP_Text _tokenText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private CustomButtonController _closeButton;
        
        [Header("Configuration")]
        [SerializeField] private string _preAmountText = "Total";
        [SerializeField] private List<TokenUpgradeController> _tokenUpgradeControllers = new List<TokenUpgradeController>();
        
        [Header("Runtime Data")]
        [SerializeField] private BetData _currentBetData;
        [SerializeField] private int _totalAmount;
        
        private BetService.Bet? _currentBet;

        #region Unity Lifecycle Methods
        
        private void Awake()
        {
            ValidateReferences();
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

        #region Initialization and Event Management
        
        /// <summary>
        /// Validates required component references and logs errors if missing.
        /// </summary>
        private void ValidateReferences()
        {
            if (_tokenPopupAnimator == null)
                Debug.LogError($"Animator reference is missing on {gameObject.name}", this);
                
            if (_closeButton == null)
                Debug.LogError($"Close button reference is missing on {gameObject.name}", this);
        }
        
        /// <summary>
        /// Subscribes to relevant events.
        /// </summary>
        private void SubscribeToEvents()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnTokenPopupOpenEvent>(OnTokenPopupOpen);
            
            if (_closeButton != null)
                _closeButton.OnButtonClicked += OnCloseButtonClicked;
        }

        /// <summary>
        /// Unsubscribes from events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnTokenPopupOpenEvent>(OnTokenPopupOpen);
            
            if (_closeButton != null)
                _closeButton.OnButtonClicked -= OnCloseButtonClicked;
        }
        
        #endregion
        
        #region Token Controllers Management
        
        /// <summary>
        /// Registers a token upgrade controller with this popup.
        /// </summary>
        /// <param name="controller">The controller to register</param>
        public void AddTokenUpgradeController(TokenUpgradeController controller)
        {
            if (!_tokenUpgradeControllers.Contains(controller))
                _tokenUpgradeControllers.Add(controller);
        }
        
        /// <summary>
        /// Updates the total bet amount based on all token controllers.
        /// </summary>
        public void UpdateAmount()
        {
            _totalAmount = 0;
            foreach (var controller in _tokenUpgradeControllers)
            {
                _totalAmount += controller.TokenAmount * (int)controller.TokenType;
            }
            UpdateAmountText();
        }
        
        /// <summary>
        /// Updates the UI text displaying the current bet amount.
        /// </summary>
        private void UpdateAmountText()
        {
            if (_amountText != null)
                _amountText.text = $"{_preAmountText}: {_totalAmount}";
        }
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Handles the token popup open event, initializing the UI with bet data.
        /// </summary>
        /// <param name="e">The open event containing bet data</param>
        private void OnTokenPopupOpen(OnTokenPopupOpenEvent e)
        {
            LogManager.Log("Token Popup Opened", LogManager.LogLevel.Development, gameObject);
            _totalAmount = 0;
            if (_tokenPopupAnimator != null)
                _tokenPopupAnimator.SetTrigger(Open);
                
            _currentBetData = e.BetData;

            // Reset all token controllers
            foreach (var controller in _tokenUpgradeControllers)
            {
                controller.ResetController();
            }

            // Check if there's an existing bet for these numbers
            _currentBet = ServiceContainer.Instance.BetService.GetBet(_currentBetData);

            // If existing bet found, initialize controller with current amount
            if (_currentBet != null)
            {
                var baseTokenController = _tokenUpgradeControllers.Find(controller => controller.TokenType == TokenType.X1);
                if (baseTokenController != null)
                {
                    baseTokenController.InitializeController(_currentBet.Value.amount);
                    _totalAmount = _currentBet.Value.amount;
                }
            }
            UpdateAmountText();
        }

        /// <summary>
        /// Handles the close button click, updating or removing the bet as needed.
        /// </summary>
        private void OnCloseButtonClicked()
        {
            // Process bet based on total amount
            if (_totalAmount > 0)
            {
                if (_currentBet != null)
                {
                    // Update existing bet
                    ServiceContainer.Instance.BetService.UpdateBet(_currentBet.Value, _totalAmount);
                }
                else
                {
                    // Place new bet
                    ServiceContainer.Instance.BetService.PlaceBet(
                        _currentBetData.betType, 
                        _currentBetData.numbersOfBet,
                        _totalAmount);
                }
            }
            else if (_currentBet != null)
            {
                // Remove bet if amount is zero
                ServiceContainer.Instance.BetService.RemoveBet(_currentBet);
            }

            // Close the popup
            if (_tokenPopupAnimator != null)
                _tokenPopupAnimator.SetTrigger(Close);

            LogManager.Log("Token Popup Closed", LogManager.LogLevel.Development, gameObject);
        }
        
        #endregion
    }
}