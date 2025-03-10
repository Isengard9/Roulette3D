using System;
using NCGames.Events.Popup;
using NCGames.Services;
using UnityEngine;

namespace NCGames.Controllers.UI.Popup.BetPopup
{
    public class BetPopupController : MonoBehaviour
    {
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");
        
        [SerializeField] private BetButtonController[] _betButtons;
        [SerializeField] private Animator _animator;


        private void OnEnable()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnBetPopupOpenEvent>(OnBetPopupOpen);
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnBetPopupCloseEvent>(OnBetPopupClose);
        }
        
        private void OnDisable()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnBetPopupOpenEvent>(OnBetPopupOpen);
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnBetPopupCloseEvent>(OnBetPopupClose);
        }
        
        private void OnBetPopupOpen(OnBetPopupOpenEvent e)
        {
            _animator.SetTrigger(Open);
        }
        
        private void OnBetPopupClose(OnBetPopupCloseEvent e)
        {
            _animator.SetTrigger(Close);
        }
        
        
    }
}