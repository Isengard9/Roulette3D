using System;
using NCGames.Helpers;
using UnityEngine;

namespace NCGames.Services
{
    public class ServiceContainer : Singleton<ServiceContainer>
    {
        [SerializeField] private BetService _betService;
        public BetService BetService => _betService;

        [SerializeField] private EventPublisherService _eventPublisherService;
        public EventPublisherService EventPublisherService => _eventPublisherService;
        
        [SerializeField] private GameService _gameService;
        public GameService GameService => _gameService;

        private void Awake()
        {
            _betService = new BetService();
            _eventPublisherService = new EventPublisherService();
        }
    }
}