using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Services
{
    public class GameplayEventService : IGameplayEventService
    {
        private Dictionary<GameplayTag, List<Action<object>>> _eventListeners = new Dictionary<GameplayTag, List<Action<object>>>();
        private Dictionary<GameplayTag, List<Action>> _eventListenersNoParam = new Dictionary<GameplayTag, List<Action>>();
        public void Initialize()
        {
            ClearAllEvents();
        }

        public void PostInitialize()
        {
        }

        public void Shutdown()
        {
            ClearAllEvents();
        }

        public string GetServiceName()
        {
            return nameof(GameplayEventService);
        }

        public void Subscribe(GameplayTag eventTag, Action<object> listener)
        {
            if (!_eventListeners.ContainsKey(eventTag))
            {
                _eventListeners[eventTag] = new List<Action<object>>();
            }
        
            if (!_eventListeners[eventTag].Contains(listener))
            {
                _eventListeners[eventTag].Add(listener);
            }
        }

        public void Subscribe(GameplayTag eventTag, Action listener)
        {
            if (!_eventListenersNoParam.ContainsKey(eventTag))
            {
                _eventListenersNoParam[eventTag] = new List<Action>();
            }
        
            if (!_eventListenersNoParam[eventTag].Contains(listener))
            {
                _eventListenersNoParam[eventTag].Add(listener);
            }
        }

        public void Unsubscribe(GameplayTag eventTag, Action<object> listener)
        {
            _eventListeners.Remove(eventTag);
        }

        public void Unsubscribe(GameplayTag eventTag, Action listener)
        {
            _eventListenersNoParam.Remove(eventTag);
        }

        public void Broadcast(GameplayTag eventTag, object data = null)
        {
            if (_eventListeners.TryGetValue(eventTag, out var eventListener))
            {
                foreach (var listener in eventListener.ToArray()) // ToArray to prevent modification during iteration
                {
                    try
                    {
                        listener?.Invoke(data);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error in event listener for {eventTag}: {ex.Message}");
                    }
                }
            }

            // Broadcast to listeners without parameters
            if (_eventListenersNoParam.ContainsKey(eventTag))
            {
                foreach (var listener in _eventListenersNoParam[eventTag].ToArray()) // ToArray to prevent modification during iteration
                {
                    try
                    {
                        listener?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error in event listener for {eventTag}: {ex.Message}");
                    }
                }
            }
        }
        
        private void ClearEvent(GameplayTag eventTag)
        {
            if (_eventListeners.ContainsKey(eventTag))
            {
                _eventListeners[eventTag].Clear();
            }
        
            if (_eventListenersNoParam.ContainsKey(eventTag))
            {
                _eventListenersNoParam[eventTag].Clear();
            }
        }
        
        private void ClearAllEvents()
        {
            _eventListeners.Clear();
            _eventListenersNoParam.Clear();
        }
    }
}