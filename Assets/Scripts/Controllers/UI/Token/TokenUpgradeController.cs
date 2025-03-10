using System;
using NCGames.Controllers.UI.Popup.TokenPopup;
using NCGames.Enums;
using TMPro;
using UnityEngine;

namespace NCGames.Controllers.UI.Token
{
    public class TokenUpgradeController: MonoBehaviour
    {
        [SerializeField] private TokenType _tokenType;
        public TokenType TokenType => _tokenType;
        [SerializeField] private int _tokenAmount;
        public int TokenAmount => _tokenAmount;
        [SerializeField] private TMP_Text _tokenTitleText;
        [SerializeField] private TMP_Text _tokenAmountText;

        [SerializeField] private CustomButtonController _upgradeButton;
        [SerializeField] private CustomButtonController _downgradeButton;
        
        private TokenPopupController _tokenPopupController;

        private void Start()
        {
            _tokenPopupController = FindObjectOfType<TokenPopupController>();
            _tokenPopupController.AddTokenUpgradeController(this);
        }

        private void OnValidate()
        {
            _tokenTitleText.text = _tokenType.ToString();   
        }

        public void InitializeController(int tokenAmount = 0)
        {
            _tokenAmount = tokenAmount;
            UpdateAmountText();
        }

        public void ResetController()
        {
            _tokenAmount = 0;
            UpdateAmountText();
        }
        
        
        private void OnEnable()
        {
            _upgradeButton.OnButtonClicked += HandleUpgradeButtonClick;
            _downgradeButton.OnButtonClicked += HandleDowngradeButtonClick;
        }
        
        private void OnDisable()
        {
            _upgradeButton.OnButtonClicked -= HandleUpgradeButtonClick;
            _downgradeButton.OnButtonClicked -= HandleDowngradeButtonClick;
        }
        
        private void HandleUpgradeButtonClick()
        {
            _tokenAmount++;
            _tokenPopupController.UpdateAmount();
            UpdateAmountText();
        }
        
        private void HandleDowngradeButtonClick()
        {
            _tokenAmount--;
            if (_tokenAmount <= 0)
            {
                _tokenAmount = 0;
            }
            _tokenPopupController.UpdateAmount();
            UpdateAmountText();
        }
        
        private void UpdateAmountText()
        {
            _tokenAmountText.text = _tokenAmount.ToString();
        }
    }
}