using NCGames.Data;
using NCGames.Events.Popup;
using NCGames.Managers;
using NCGames.Services;
using UnityEngine;

namespace NCGames.Controllers.UI
{
    
    
    [RequireComponent(typeof(CustomButtonController))]
    public class BetButtonController : MonoBehaviour
    {
        [SerializeField] private CustomButtonController buttonController;

        [SerializeField] private BetData _data;
        public BetData Data => _data;
        private void Awake()
        {
            Initialize();
        }


        private void Initialize()
        {
            buttonController = GetComponent<CustomButtonController>();
        }


        private void OnEnable()
        {
            buttonController.OnButtonClicked += HandleButtonClick;
        }

        private void OnDisable()
        {
            buttonController.OnButtonClicked -= HandleButtonClick;
        }


        private void HandleButtonClick()
        {
            LogManager.Log($"Bet button clicked: {_data.betType}", LogManager.LogLevel.Development, gameObject);
            ServiceContainer.Instance.EventPublisherService.Publish(new OnTokenPopupOpenEvent()
            {
                BetData = _data
            });
        }
    }
}