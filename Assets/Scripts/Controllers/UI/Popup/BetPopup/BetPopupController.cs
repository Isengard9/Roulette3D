using System;
using NCGames.Events.Game;
using NCGames.Events.Popup;
using NCGames.Services;
using UnityEngine;

namespace NCGames.Controllers.UI.Popup.BetPopup
{
    /// <summary>
    /// Controls the bet popup UI, handling opening and closing animations
    /// and responding to related events.
    /// </summary>
    public class BetPopupController : MonoBehaviour
    {
        // Animation parameter hashes for better performance
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");
        
        [Header("References")]
        [SerializeField] private BetButtonController[] _betButtons;
        [SerializeField] private Animator _animator;

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

        #region Initialization

        /// <summary>
        /// Validates required component references and logs errors if missing.
        /// </summary>
        private void ValidateReferences()
        {
            if (_animator == null)
            {
                Debug.LogError($"Animator reference is missing on {gameObject.name}.", this);
            }
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Subscribes to popup-related events.
        /// </summary>
        private void SubscribeToEvents()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnBetPopupOpenEvent>(OnBetPopupOpen);
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnBetPopupCloseEvent>(OnBetPopupClose);
            ServiceContainer.Instance.EventPublisherService.Subscribe<ResetGameEvent>(ResetGame);
        }

        /// <summary>
        /// Unsubscribes from popup-related events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnBetPopupOpenEvent>(OnBetPopupOpen);
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnBetPopupCloseEvent>(OnBetPopupClose);
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<ResetGameEvent>(ResetGame);
        }

        /// <summary>
        /// Handles the popup open event by triggering the open animation.
        /// </summary>
        /// <param name="e">The open event data</param>
        private void OnBetPopupOpen(OnBetPopupOpenEvent e)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(Open);
            }
        }

        /// <summary>
        /// Handles the popup close event by triggering the close animation.
        /// </summary>
        /// <param name="e">The close event data</param>
        private void OnBetPopupClose(OnBetPopupCloseEvent e)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(Close);
            }
        }
        
        private void ResetGame(ResetGameEvent e)
        {
            _animator.SetTrigger(Open);
        }

        #endregion
    }
}