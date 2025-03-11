using NCGames.Events.Game;
using NCGames.Services;
using TMPro;
using UnityEngine;

namespace NCGames.Controllers.UI.Popup.ScorePopup
{
    public class ScorePopupController: MonoBehaviour
    {
        // Animation parameter hashes for better performance
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");

        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _titleText;

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
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnGameEndedEvent>(OnGameEnded);
        }
        
        /// <summary>
        /// Unsubscribe to popup-related events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnGameEndedEvent>(OnGameEnded);
        }
        
        #endregion
        
        
        #region Event Handlers

        private void OnGameEnded(OnGameEndedEvent e)
        {
            _animator.SetTrigger(Open);
            Invoke(nameof(ClosePopup), 5f);

            var amount = ServiceContainer.Instance.BetService.EvaluateBets(e.WinningNumber);
            _scoreText.text = amount.ToString();

            _titleText.text = amount > 0 ? "You won!" : "You lost!";
        }
        
        #endregion
        
        private void ClosePopup()
        {
            _animator.SetTrigger(Close);
            ServiceContainer.Instance.EventPublisherService.Publish(new ResetGameEvent());
        }
    }
}