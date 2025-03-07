using System;

namespace NCGames.Interfaces
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
    }
}