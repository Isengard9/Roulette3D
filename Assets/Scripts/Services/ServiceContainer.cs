using System;
using NCGames.Helpers;
using UnityEngine;

namespace NCGames.Services
{
    public class ServiceContainer : Singleton<ServiceContainer>
    {
        private BetService _betService;
        public BetService BetService => _betService;

        private EventPublisherService _eventPublisherService;
        public EventPublisherService EventPublisherService => _eventPublisherService;

        private void Start()
        {
            _betService = new BetService();
            _eventPublisherService = new EventPublisherService();
        }
    }
}