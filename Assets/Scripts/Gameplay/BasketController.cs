using System;
using Core;
using Data;
using Services;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Represents a controller for managing basket-related logic during gameplay.
    /// Handles interactions, logic, or updates specific to the basket object in the game.
    /// </summary>
    public class BasketController : MonoBehaviour
    {
        private IGameplayEventService _gameplayEventService;

        private void Start()
        {
            _gameplayEventService = ServiceLocator.Get<IGameplayEventService>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IBallController>(out var ball))
            {
                _gameplayEventService.Broadcast(GameplayTags.Event_Score);
            }
        }
    }
}
