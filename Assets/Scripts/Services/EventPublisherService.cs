using System;
using System.Collections.Generic;
using NCGames.Interfaces;
using NCGames.Managers;
using UnityEngine;

namespace NCGames.Services
{
     public class EventPublisherService : IEventPublisher
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        private readonly object _lock = new object();

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            List<Delegate> handlersCopy;

            lock (_lock)
            {
                if (!_handlers.ContainsKey(eventType))
                {
                    return;
                }

                // handlers listesinin bir kopyasını al
                handlersCopy = new List<Delegate>(_handlers[eventType]);
            }

            for (int i = 0; i < handlersCopy.Count; i++)
            {
                var handler = handlersCopy[i];
                if (handler == null) continue;

                try
                {
                    ((Action<TEvent>)handler)(@event);
                }
                catch (Exception ex)
                {
                    LogManager.Log("Error while publishing event: " + ex.Message+ $"\nHandler: {handler.Target}", LogManager.LogLevel.Error);
                }
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            
            var eventType = typeof(TEvent);
            lock (_lock)
            {
                if (!_handlers.ContainsKey(eventType))
                {
                    _handlers[eventType] = new List<Delegate>();
                }

                _handlers[eventType].Add(handler);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            lock (_lock)
            {
                if (!_handlers.ContainsKey(eventType))
                {
                    return;
                }
                //Debug.Log($"Unsubscribed handler : {handler}");
                _handlers[eventType].Remove(handler);

                if (_handlers[eventType].Count == 0)
                {
                    _handlers.Remove(eventType);
                }
            }
        }
    }
}