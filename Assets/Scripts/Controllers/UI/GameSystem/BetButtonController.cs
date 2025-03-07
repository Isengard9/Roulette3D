using System;
using NCGames.Enums;
using NCGames.Managers;
using UnityEngine;

namespace NCGames.Controllers.UI
{
    [RequireComponent(typeof(CustomButtonController))]
    public class BetButtonController : MonoBehaviour
    {
        [SerializeField] private BetType betType;
        [SerializeField] private CustomButtonController buttonController;
        [SerializeField] private int[] numbersOfBet;
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
            LogManager.Log($"Bet button clicked: {betType}", LogManager.LogLevel.Development, gameObject);
        }
    }
}