using System;
using NCGames.Events.UI;
using NCGames.Managers;
using NCGames.Services;
using UnityEngine.UI;

namespace NCGames.Controllers.UI
{
    /// <summary>
    /// Custom button controller that extends Unity's Button class,
    /// providing additional event functionality when clicked.
    /// </summary>
    public class CustomButtonController : Button
    {
        private CustomButtonClickedEvent _buttonClickedEvent;

        /// <summary>
        /// Action that gets invoked when the button is clicked.
        /// Subscribe to this event to receive button click notifications.
        /// </summary>
        public Action OnButtonClicked;

        #region Unity Lifecycle Methods

        protected override void Start()
        {
            base.Start();
            InitializeButtonEvent();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterClickListener();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnregisterClickListener();
        }

        #endregion

        #region Button Event Handling

        /// <summary>
        /// Initializes the button click event object with a reference to this button.
        /// </summary>
        private void InitializeButtonEvent()
        {
            _buttonClickedEvent = new CustomButtonClickedEvent
            {
                ClickedButton = this
            };
        }

        /// <summary>
        /// Registers the button click listener to handle click events.
        /// </summary>
        private void RegisterClickListener()
        {
            onClick.AddListener(HandleButtonClick);
        }

        /// <summary>
        /// Unregisters the button click listener to prevent memory leaks.
        /// </summary>
        private void UnregisterClickListener()
        {
            onClick.RemoveListener(HandleButtonClick);
        }

        /// <summary>
        /// Handles the button click event by invoking the OnButtonClicked action
        /// and publishing the button clicked event through the event service.
        /// </summary>
        private void HandleButtonClick()
        {
            // Invoke the direct callback if subscribed
            OnButtonClicked?.Invoke();
            
            // Publish the event through the event system
            ServiceContainer.Instance?.EventPublisherService.Publish(_buttonClickedEvent);
        }

        #endregion
    }
}