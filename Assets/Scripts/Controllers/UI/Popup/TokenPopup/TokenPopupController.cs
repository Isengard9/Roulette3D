using System;
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
    public class TokenPopupController : MonoBehaviour
    {
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private Animator _tokenPopupAnimator;
        [SerializeField] private TMP_Text _tokenText;
        
        [SerializeField] string _preAmountText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private int totalAmount;
        
        [SerializeField] private CustomButtonController _closeButton;
        [SerializeField] private BetData _currentBetData;

        private BetService.Bet? _currentBet = null;
        
        [SerializeField] private List<TokenUpgradeController> _tokenUpgradeControllers = new List<TokenUpgradeController>();
        private void OnEnable()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnTokenPopupOpenEvent>(OnTokenPopupOpen);
            _closeButton.OnButtonClicked+= OnCloseButtonClicked;
        }
        
        private void OnDisable()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnTokenPopupOpenEvent>(OnTokenPopupOpen);
            _closeButton.OnButtonClicked-= OnCloseButtonClicked;
        }

        public void AddTokenUpgradeController(TokenUpgradeController controller)
        {
            if(!_tokenUpgradeControllers.Contains(controller))
                _tokenUpgradeControllers.Add(controller);
        }
        
        private void OnTokenPopupOpen(OnTokenPopupOpenEvent e)
        {
           LogManager.Log("Token Popup Opened", LogManager.LogLevel.Development, gameObject);
            _tokenPopupAnimator.SetTrigger(Open);
            _currentBetData = e.BetData;
            
            
            foreach (var tokenUpgradeController in _tokenUpgradeControllers)
            {
                tokenUpgradeController.ResetController();
            }

           _currentBet = ServiceContainer.Instance.BetService.GetBet(_currentBetData);

            if (_currentBet != null)
            {
                var tokenUpgradeController = _tokenUpgradeControllers.Find(controller => controller.TokenType ==TokenType.X1);
                tokenUpgradeController.InitializeController(_currentBet.Value.amount);
            }
        }
        
        private void OnCloseButtonClicked()
        {
            if (totalAmount > 0)
            {
                if (_currentBet != null)
                {
                    ServiceContainer.Instance.BetService.UpdateBet(_currentBet.Value, totalAmount);
                }
                else
                {
                    ServiceContainer.Instance.BetService.PlaceBet(_currentBetData.betType, _currentBetData.numbersOfBet,
                        totalAmount);
                }
            }

            if (_currentBet != null && totalAmount <= 0)
            {
                ServiceContainer.Instance.BetService.RemoveBet(_currentBet);
            } 

            _tokenPopupAnimator.SetTrigger(Close);
            
            LogManager.Log("Token Popup Closed", LogManager.LogLevel.Development, gameObject);
        }

        public void UpdateAmount()
        {
            totalAmount = 0;
            foreach (var tokenUpgradeController in _tokenUpgradeControllers)
            {
                totalAmount += tokenUpgradeController.TokenAmount * (int)tokenUpgradeController.TokenType;
            }
            UpdateAmountText();
        }
        
        private void UpdateAmountText()
        {
            _amountText.text = _preAmountText+": " + totalAmount;
        }
    }
}