using NCGames.Controllers.UI.Popup.TokenPopup;
using NCGames.Enums;
using TMPro;
using UnityEngine;

namespace NCGames.Controllers.UI.Token
{
    /// <summary>
    /// Controls the UI for upgrading and downgrading token amounts for a specific token type in betting.
    /// </summary>
    public class TokenUpgradeController : MonoBehaviour
    {
        [Header("Token Configuration")]
        [SerializeField] private TokenType _tokenType;
        [SerializeField] private int _tokenAmount;

        [Header("UI References")]
        [SerializeField] private TMP_Text _tokenTitleText;
        [SerializeField] private TMP_Text _tokenAmountText;
        [SerializeField] private CustomButtonController _upgradeButton;
        [SerializeField] private CustomButtonController _downgradeButton;

        private TokenPopupController _tokenPopupController;

        /// <summary>
        /// Gets the type of token this controller manages.
        /// </summary>
        public TokenType TokenType => _tokenType;

        /// <summary>
        /// Gets the current amount of tokens.
        /// </summary>
        public int TokenAmount => _tokenAmount;

        #region Unity Lifecycle Methods

        private void Start()
        {
            InitializeReferences();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void OnValidate()
        {
            UpdateTitleText();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes references and registers this controller with the token popup.
        /// </summary>
        private void InitializeReferences()
        {
            _tokenPopupController = FindObjectOfType<TokenPopupController>();
            
            if (_tokenPopupController == null)
            {
                Debug.LogError($"TokenPopupController not found in scene for {gameObject.name}", this);
                return;
            }
            
            _tokenPopupController.AddTokenUpgradeController(this);
        }

        /// <summary>
        /// Updates the UI title text to match the token type.
        /// </summary>
        private void UpdateTitleText()
        {
            if (_tokenTitleText != null)
            {
                _tokenTitleText.text = _tokenType.ToString();
            }
        }

        #endregion

        #region Event Management

        /// <summary>
        /// Subscribes to button click events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_upgradeButton != null)
                _upgradeButton.OnButtonClicked += HandleUpgradeButtonClick;
            
            if (_downgradeButton != null)
                _downgradeButton.OnButtonClicked += HandleDowngradeButtonClick;
        }

        /// <summary>
        /// Unsubscribes from button click events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_upgradeButton != null)
                _upgradeButton.OnButtonClicked -= HandleUpgradeButtonClick;
            
            if (_downgradeButton != null)
                _downgradeButton.OnButtonClicked -= HandleDowngradeButtonClick;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the controller with a specific token amount.
        /// </summary>
        /// <param name="tokenAmount">Initial token amount (defaults to 0)</param>
        public void InitializeController(int tokenAmount = 0)
        {
            _tokenAmount = tokenAmount;
            UpdateAmountText();
        }

        /// <summary>
        /// Resets the controller to zero tokens.
        /// </summary>
        public void ResetController()
        {
            _tokenAmount = 0;
            UpdateAmountText();
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Handles the upgrade button click by incrementing token amount.
        /// </summary>
        private void HandleUpgradeButtonClick()
        {
            _tokenAmount++;
            UpdateUI();
        }

        /// <summary>
        /// Handles the downgrade button click by decrementing token amount.
        /// </summary>
        private void HandleDowngradeButtonClick()
        {
            _tokenAmount--;
            if (_tokenAmount < 0)
            {
                _tokenAmount = 0;
            }
            UpdateUI();
        }

        #endregion

        #region UI Updates

        /// <summary>
        /// Updates the UI display of the token amount.
        /// </summary>
        private void UpdateAmountText()
        {
            if (_tokenAmountText != null)
            {
                _tokenAmountText.text = _tokenAmount.ToString();
            }
        }

        /// <summary>
        /// Updates both the token popup's total amount and this controller's amount display.
        /// </summary>
        private void UpdateUI()
        {
            if (_tokenPopupController != null)
            {
                _tokenPopupController.UpdateAmount();
            }
            UpdateAmountText();
        }

        #endregion
    }
}