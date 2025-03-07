using System;
using NCGames.Events.UI;
using NCGames.Managers;
using NCGames.Services;
using UnityEngine.UI;

namespace NCGames.Controllers.UI
{
    /// <summary>
    /// Custom button controller that extends Unity's Button class and publishes events when clicked.
    /// </summary>
    public class CustomButtonController : Button
    {
        private CustomButtonClickedEvent _buttonClickedEvent;
        public Action OnButtonClicked;
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

        private void InitializeButtonEvent()
        {
            _buttonClickedEvent = new CustomButtonClickedEvent()
            {
                ClickedButton = this
            };
        }

        private void RegisterClickListener()
        {
            onClick.AddListener(HandleButtonClick);
        }

        private void UnregisterClickListener()
        {
            onClick.RemoveListener(HandleButtonClick);
        }

        private void HandleButtonClick()
        {
            LogManager.Log($"Button clicked: {name}", LogManager.LogLevel.Development, gameObject);
            OnButtonClicked?.Invoke();
            ServiceContainer.Instance.EventPublisherService.Publish(_buttonClickedEvent);
        }
    }
}