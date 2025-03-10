using NCGames.Data;
using NCGames.Events.Popup;
using NCGames.Managers;
using NCGames.Services;
using UnityEngine;

namespace NCGames.Controllers.UI
{
    /// <summary>
    /// Controls bet buttons in the roulette UI, handling bet-related actions when clicked.
    /// </summary>
    [RequireComponent(typeof(CustomButtonController))]
    public class BetButtonController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CustomButtonController buttonController;

        [Header("Configuration")]
        [SerializeField] private BetData _data;
        
        /// <summary>
        /// The betting data associated with this button.
        /// </summary>
        public BetData Data => _data;

        #region Unity Lifecycle Methods

        private void Awake()
        {
            Initialize();
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
        /// Initializes the button controller reference if not already assigned.
        /// </summary>
        private void Initialize()
        {
            if (buttonController == null)
            {
                buttonController = GetComponent<CustomButtonController>();
            }
        }

        /// <summary>
        /// Subscribes to button click events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (buttonController != null)
            {
                buttonController.OnButtonClicked += HandleButtonClick;
            }
        }

        /// <summary>
        /// Unsubscribes from button click events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (buttonController != null)
            {
                buttonController.OnButtonClicked -= HandleButtonClick;
            }
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Handles the button click event by opening the token popup for placing bets.
        /// </summary>
        private void HandleButtonClick()
        {
            LogManager.Log($"Bet button clicked: {_data.betType}", LogManager.LogLevel.Development, gameObject);
            
            // Publish an event to open the token popup with the current bet data
            ServiceContainer.Instance.EventPublisherService.Publish(new OnTokenPopupOpenEvent
            {
                BetData = _data
            });
        }

        #endregion
    }
}